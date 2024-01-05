using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.DAL;
using MTCG.Models;

namespace MTCG.Services
{
    public class CardService: ICardService
    {
        private readonly ICardManager _cardsManager;

        public CardService(ICardManager cardsManager)
        {
            _cardsManager = cardsManager;
        }

        public bool areCardsAvaliable(List<string> cardIds, int userId)
        {
            List<Card>? cards = _cardsManager.GetAvaliableUserCards(userId);
            if (cards == null) return false;
            foreach (string cardId in cardIds)
            {
                if (!cards.Any(card => card.Id == cardId)) return false;
            }
            return true;
        }

        public string? GetUserCards(int userId)
        {
            return CardHelper.MapCardsToResponse(_cardsManager.GetUserCards(userId));
        }

         

        public string? GetUserDeck(int userId, bool plain)
        {

            return CardHelper.MapCardsToResponse(_cardsManager.GetUserCards(userId, true), plain);

        }

        public bool isCardfromUser(string cardId, int userId)
        {
            return _cardsManager.GetUserCards(userId)!.Any(card => card.Id == cardId);
        }

        public bool isCardinDeck(string cardId, int userId)
        {
            return _cardsManager.GetUserCards(userId, true)!.Any(card => card.Id == cardId);
        }

        public bool SetUserDeck(int userId, List<string> cardIds)
        {
            if (!areCardsAvaliable(cardIds, userId)) return false;
            return _cardsManager.SetCardsToDeck(cardIds);
        }
    }
}
