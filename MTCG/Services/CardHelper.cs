using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MTCG.Services
{
    public static class CardHelper
    {
        public static string? MapCardsToResponse(List<Card>? cards, bool plain = false)
        {
            if (cards == null) return null;

            if (plain)
            {
                var responseCards = new StringBuilder();
                responseCards.Append("|||||---------------------------------------------------------------|||||\n");
                for (int i = 0; i < cards.Count; i++)
                {
                    responseCards.Append($"     Id = {cards[i].Id}\n");
                    responseCards.Append($"     Name = {cards[i].Name}\n");
                    responseCards.Append($"     Damage = {cards[i].Damage}\n");
                    if (i < cards.Count - 1)
                    {
                        responseCards.Append("     ---------------------------------------------------------------      \n");
                    }
                }
                responseCards.Append("|||||---------------------------------------------------------------|||||\n");
                return responseCards.ToString();
            }
            else
            {
                return JsonConvert.SerializeObject(cards, Formatting.Indented);
            }
        }
    }
}