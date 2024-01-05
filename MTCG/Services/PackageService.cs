 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;
using System.Text.RegularExpressions;
using MTCG.DAL;


namespace MTCG.Services
{
    public class PackageService: IPackageService
    {
        private const string NamesPattern = @"(?=[A-Z])";
        private readonly ICardManager _cardsManager;

        public PackageService(ICardManager cardsManager)
        {
            _cardsManager = cardsManager;
        }

        public List<Card> CreateNewPackage(List<RawRequestCard> Cards)
        {
            List<Card> Package = new List<Card>();
            foreach (var card in Cards)
            {
                string[] splittedNames = Regex.Split(card.Name, NamesPattern);
                if (splittedNames.Length == 2)
                {
                    Element element;
                    switch (splittedNames[1])
                    {
                        case "Knight":
                            element = Element.Regular;
                            break;
                        case "Dragon":
                            element = Element.Fire;
                            break;

                        case "Ork":
                            element = Element.Regular;
                            break;

                        case "Kraken":
                            element = Element.Water;
                            break;

                        case "Wizzard":
                            element = Element.Fire;
                            break;
                        default:
                            throw new ArgumentException("Card Name not known!");
                    }

                    // Already checked if the Names are valid above
                    Enum.TryParse(splittedNames[1], out CardType type);

                    Package.Add(new Card(card.Id, card.Name, card.Damage, type, element));
                }
                else if (splittedNames.Length == 3)
                {
                    if (!Enum.TryParse(splittedNames[1], out Element element))
                    {
                        throw new ArgumentException("Element not known!");
                    }
                    if (splittedNames[2] == "Spell")
                    {
                        Package.Add(new Card(card.Id, card.Name, card.Damage, CardType.Spell, element));
                        continue;
                    }
                    if (!Enum.TryParse(splittedNames[2], out CardType type))
                    {
                        throw new ArgumentException("Species not known!");
                    }
                    Package.Add(new Card(card.Id, card.Name, card.Damage, type, element));
                }
                else
                {
                    throw new ArgumentException("Card Name not known!");
                }
            }
            return Package;
        }


        public bool SavePackage(List<Card> Cards)
        {
            return _cardsManager.InsertCards(Cards);
        }
        public bool isPackageAvaliabe()
        {
            return _cardsManager.GetFreePackage() != null;
        }

        public string AssignUserToPackage(int userId)
        {
            List<Card> cards = _cardsManager.GetFreePackage(userId)!;
            return CardHelper.MapCardsToResponse(cards)!;

        }
    }
}
