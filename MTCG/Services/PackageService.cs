using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;
using System.Text.RegularExpressions;



namespace MTCG.Services
{
    public class PackageService: IPackagesService
    {
        private const string NamesPattern = @"(?=[A-Z])";

        public PackageService()
        {

        }

        public List<Card> CreateNewPackage(List<Dictionary<string, string>> Cards)
        {
            List<Card> Package = new List<Card>();
            foreach (Dictionary<string, string> card in Cards)
            {
                string[] splittedNames = Regex.Split(card["Name"], NamesPattern);
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
                    Enum.TryParse(splittedNames[1], out Species species);
                    Package.Add(new Card(card["Id"], card["Name"],
                            float.Parse(card["Damage"]), CardType.Monster, 
                            element, species));

                } else if (splittedNames.Length == 3)
                {
                    if (!Enum.TryParse(splittedNames[1], out Element element))
                    {
                        throw new ArgumentException("Element not known!");
                    }
                    if (splittedNames[2] == "Spell")
                    {
                        Package.Add(new Card(card["Id"], card["Name"], 
                            float.Parse(card["Damage"]), CardType.Spell, element));
                        continue;
                    }
                    if (!Enum.TryParse(splittedNames[2], out Species species))
                    {
                        throw new ArgumentException("Species not known!");
                    }
                    Package.Add(new Card(card["Id"], card["Name"],
                            float.Parse(card["Damage"]), CardType.Monster,
                            element, species));
                } else
                {
                    throw new ArgumentException("Card Name not known!"); 
                }

            }
            return Package;
        }
    }
}
