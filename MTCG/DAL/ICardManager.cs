using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL
{
    public interface ICardManager
    {
        public bool InsertCards(List<Card> cards);
        public List<Card>? GetFreePackage(int? userId = null);

        public List<Card>? GetUserCards(int userId, bool onlyDeck = false);

        public bool SetCardsToDeck(List<string> cardIds);

        public List<Card>? GetAvaliableUserCards(int userId);

        Card GetCard(string cardId);

        public bool UpdateCardOwner(string cardId, int newUserId);

        int GetCardOwner(string cardId);


    }
}
