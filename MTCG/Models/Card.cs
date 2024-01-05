using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record Card(string Id, string Name, CardType Type, Element Element)
    {
        private float _damage;

        public float Damage
        {
            get => _damage;
            private init
            {
                if (value < 0)
                {
                    throw new ArgumentException("Damage cannot be less than 0.");
                }
                _damage = value;
            }
        }

        public Card(string id, string name, float damage, CardType type, Element element)
            : this(id, name, type, element)
        {
            Damage = damage;
        }

        public override string ToString()
        {
            return $"Card(Name: {Name}, Damage: {Damage})";
        }
}
}
