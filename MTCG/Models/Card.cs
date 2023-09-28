using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record class Card
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public float Damage { get; init; }
        public CardType Type { get; init; }
        public Element Element { get; init; }
        public Species? Species { get; init; }

        public Card(string id, string name, float damage,
            CardType type, Element element, Species? species = null)
        {
            Id = id;
            Name = name;
            Damage = damage;
            Type = type;
            Element = element;
            Species = species;
        }
    }
}
