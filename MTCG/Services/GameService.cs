using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;
using MTCG.DAL;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace MTCG.Services
{

    public class GameService : IGameService
    {
        private IUserManager _userDao;
        private ConcurrentQueue<User> playerBattleQueue;
        private Dictionary<int, string> playerLogsBattle;
        private Mutex _readMutex;

        private ICardManager _cardDao;

        public GameService(IUserManager userDao, ICardManager cardDao)
        {
            _userDao = userDao;
            playerBattleQueue  = new();
            playerLogsBattle = new();
            _cardDao = cardDao;
            _readMutex = new Mutex();
        }

        public Winner BattleCards(Card card1, Card card2) {

            Dictionary<CardType, CardType?> attackMap = new Dictionary<CardType, CardType?>
            {
                {CardType.Dragon, CardType.Goblin},
                {CardType.Wizzard, CardType.Ork },
            };

            if ((card1.Name == "WaterSpell" && card2.Type == CardType.Knight) ||
                (card2.Name == "WaterSpell" && card1.Type == CardType.Knight))
                return card2.Name == "WaterSpell" ? Winner.Second : Winner.First;

            // The Kraken is immune against spells. 
            if ((card1.Type == CardType.Spell && card2.Type == CardType.Kraken) ||
                (card2.Type == CardType.Spell && card1.Type == CardType.Kraken))
                return card2.Type == CardType.Kraken ? Winner.Second : Winner.First;

            // The FireElves know Dragons since they were little and can evade their attacks. 
            if ((card1.Name == "FireElf" && card2.Type == CardType.Dragon) ||
                (card2.Name == "FireElf" && card1.Type == CardType.Dragon))
                return card2.Name == "FireElf" ? Winner.Second : Winner.First;


            if ((card1.Type != CardType.Spell && card2.Type != CardType.Spell) ||
                card1.Element == card2.Element)
            {
                    // Species cannot be null, because both cards are Monster ( Monster species cannot be null)
                    if ((attackMap.ContainsKey(card1.Type) && attackMap[card1.Type] == card2.Type) ||
                        attackMap.ContainsKey(card2.Type) && attackMap[card2.Type] == card1.Type)
                        return attackMap.ContainsKey(card2.Type) ? Winner.Second : Winner.First;
                
                if (card1.Damage == card2.Damage)
                    return Winner.Draw;
                return card2.Damage > card1.Damage ? Winner.Second : Winner.First;
            }

            Dictionary<Element, Element> elementAttack = new Dictionary<Element, Element>
            {
                {Element.Water, Element.Fire},
                {Element.Fire, Element.Regular},
                {Element.Regular, Element.Water}
            };
            float damage1;
            float damage2;
            if (elementAttack[card1.Element] == card2.Element)
            {
                damage1 = card1.Damage * 2;
                damage2 = card2.Damage / 2;
            } else
            {
                damage1 = card1.Damage / 2;
                damage2 = card2.Damage * 2;
            }
            if (damage1 == damage2)
                return Winner.Draw;
            return damage2 > damage1 ? Winner.Second : Winner.First;
        }

        public string GetScoreboard()
        {
            return _userDao.GetGameScoreboard();
        }

        public string GetUserScore(int userId)
        {
            return _userDao.GetUserScore(userId);
        }
        private Random rand = new Random();

        private T RandomElement<T>(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                throw new ArgumentException("The list is empty or null.");
            }

            int index = rand.Next(list.Count);
            return list[index];
        }

        private const int standardEloChanges = 10;

        public string? WaitOrStartBattle(User user)
        {
            // Check if User has Cards
            if (_cardDao.GetUserCards(user.Id, true) == null) return null;

            _readMutex.WaitOne();
            if (playerBattleQueue.Count == 0)
            {
                playerBattleQueue.Enqueue(user);
                _readMutex.ReleaseMutex();
                while (!playerLogsBattle.ContainsKey(user.Id))
                {
                    Thread.Sleep(30);
                }
                string thisUserBattle = playerLogsBattle[user.Id];
                playerLogsBattle.Remove(user.Id);
                return thisUserBattle;
            }
            _readMutex.ReleaseMutex();
            User userInQueue;
            playerBattleQueue.TryDequeue(out userInQueue!);
            StringBuilder battleLog = new();

            for (int i = 0; i < 100; i++)
            {
                List<Card>? thisUserCards = _cardDao.GetUserCards(user.Id, true);
                List<Card>? otherUserCards = _cardDao.GetUserCards(userInQueue.Id, true);
                if (thisUserCards == null) {
                    battleLog.Append($"{user} ran out of cards\n");
                    playerLogsBattle[userInQueue.Id] = battleLog.ToString();
                    return battleLog.ToString();
                } else if(otherUserCards == null)
                {
                    battleLog.Append($"{userInQueue} ran out of cards\n");
                    playerLogsBattle[userInQueue.Id] = battleLog.ToString();
                    return battleLog.ToString();
                }
                Card thisUserCard = RandomElement(thisUserCards);
                Card otherUserCard = RandomElement(otherUserCards);
                int thisUserElo = _userDao.GetUserElo(user.Id);
                int otherUserElo = _userDao.GetUserElo(userInQueue.Id);


                int eloCalculation = Math.Abs(thisUserElo - otherUserElo) / 10;
                int firstUserWinElo;
                int secondUserWinElo;
                if (thisUserElo >= otherUserElo) { 
                    firstUserWinElo = standardEloChanges - eloCalculation > 1 ? standardEloChanges - eloCalculation: 1;
                    secondUserWinElo = 2 * standardEloChanges - firstUserWinElo;
                }
                else
                {
                    secondUserWinElo = standardEloChanges - eloCalculation > 1 ? standardEloChanges - eloCalculation : 1;
                    firstUserWinElo = 2 * standardEloChanges - secondUserWinElo;

                }
                switch (BattleCards(thisUserCard, otherUserCard))
                {
                    case Winner.First:
                        _userDao.changeUserElo(user.Id, firstUserWinElo);
                        _userDao.changeUserElo(userInQueue.Id, -firstUserWinElo);
                        _userDao.changeWinLosses(user.Id, win: true);
                        _userDao.changeWinLosses(userInQueue.Id, win: false);
                        _cardDao.UpdateCardOwner(otherUserCard.Id, user.Id);

                        battleLog.AppendLine($"{user}'s {thisUserCard} (ELO: {thisUserElo} to {_userDao.GetUserElo(user.Id)}) defeated {userInQueue}'s {otherUserCard} (ELO: {otherUserElo} to {_userDao.GetUserElo(userInQueue.Id)}).");
                        break;

                    case Winner.Second:
                        _userDao.changeUserElo(user.Id, -secondUserWinElo);
                        _userDao.changeUserElo(userInQueue.Id, secondUserWinElo);
                        _userDao.changeWinLosses(user.Id, win: false);
                        _userDao.changeWinLosses(userInQueue.Id, win: true);
                        _cardDao.UpdateCardOwner(thisUserCard.Id, userInQueue.Id);

                        battleLog.AppendLine($"{user}'s {thisUserCard} (ELO: {thisUserElo} to {_userDao.GetUserElo(user.Id)}) was defeated by {userInQueue}'s {otherUserCard} (ELO: {otherUserElo} to {_userDao.GetUserElo(userInQueue.Id)}).");
                        break;

                    case Winner.Draw:
                        _userDao.changeUserElo(user.Id, -(thisUserElo - otherUserElo) / 10);
                        _userDao.changeUserElo(userInQueue.Id, (thisUserElo - otherUserElo) / 10);

                        battleLog.AppendLine($"It's a draw! {user}'s {thisUserCard} (ELO: {thisUserElo} to {_userDao.GetUserElo(user.Id)}) and {userInQueue}'s {otherUserCard} (ELO: {otherUserElo} to {_userDao.GetUserElo(userInQueue.Id)}).");
                        break;
                }


            }
            battleLog.Append("Battle reached max round (100)\n");
            playerLogsBattle[userInQueue.Id] = battleLog.ToString();
            return battleLog.ToString();


        }
    }
    
}

