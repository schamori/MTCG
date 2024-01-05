using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interface
{
    public interface ICardService
    {
        string? GetUserCards(int userId);
        string? GetUserDeck(int userId, bool plain);

        bool areCardsAvaliable(List<string> cardIds, int userId);
        bool SetUserDeck(int userId, List<string> cardIds);

        bool isCardfromUser(string cardId, int userId);

        bool isCardinDeck(string cardId, int userId);

    }
}
