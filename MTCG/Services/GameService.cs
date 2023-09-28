using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;

namespace MTCG.Services
{

    public class GameService : IGameService
    {
        public GameService()
        {
        }

        


        public int BattleCards(Card card1, Card card2) {
            /* 
             * Returns 0: for the card1 as the winner 
             * 1: for card2 as the winner.
             * 2: for a draw
            */

            Dictionary<Species, Species?> attackMap = new Dictionary<Species, Species?>
            {
                {Species.Dragon, Species.Goblin},
                {Species.Wizzard, Species.Ork },
            };

            // Goblins are too afraid of Dragons to attack. 
            if ((card1.Name == "WaterSpell" && card2.Species == Species.Knight) ||
                (card2.Name == "WaterSpell" && card1.Species == Species.Knight))
                return Convert.ToInt32(card2.Name == "WaterSpells");

            // The Kraken is immune against spells. 
            if ((card1.Type == CardType.Spell && card2.Species == Species.Kraken) ||
                (card2.Type == CardType.Spell && card1.Species == Species.Kraken))
                return Convert.ToInt32(card2.Species == Species.Kraken);

            // The FireElves know Dragons since they were little and can evade their attacks. 
            if ((card1.Name == "FireElf" && card2.Species == Species.Dragon) ||
                (card2.Name == "FireElf" && card1.Species == Species.Dragon))
                    return Convert.ToInt32(card2.Name == "FireElf");


            if ((card1.Type == CardType.Monster && card2.Type == CardType.Monster)||
                card1.Element == card2.Element)
            {
                if(card1.Type == CardType.Monster && card2.Type == CardType.Monster) { 
                    // Species cannot be null, because both cards are Monster
                    if ((attackMap.ContainsKey((Species)card1.Species) && attackMap[(Species)card1.Species] == card2.Species) ||
                        attackMap.ContainsKey((Species)card2.Species) && attackMap[(Species)card2.Species] == card1.Species)
                        return Convert.ToInt32(attackMap.ContainsKey((Species)card2.Species));
                }
                if (card1.Damage == card2.Damage)
                    return 2;
                return Convert.ToInt32(card2.Damage > card1.Damage);
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
                return 2;
            return Convert.ToInt32(damage2 > damage1);


        }
#pragma warning restore CS8629 // Nullable value type may be null.

        }
    
}



/*
public static string GenerateRandomToken(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder token = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                token.Append(chars[index]);
            }

            return token.ToString();
        }
public Credentials Login(string Username, string Password)
{
    // Check if Username and Password are valid
    int coints = 20;
    Users.Add(new User(Username, Password, coints));
    string randomToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    // Save Token in database
    return new Credentials(Username, randomToken);
}

private User GetUserByUsername(string username)
{
    foreach (User user in Users)
    {
        if (user.Name == username)
        {
            return user;
        }
    }
    throw new Exception("Username not found");
}

/* public string BuyCards(string Username, string randomToken){
    GetUserByUsername(Username).Cards.Push(NewCard());


} */