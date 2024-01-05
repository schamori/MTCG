using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record class Trade(string Id, string CardToTrade, MonsterOrSpell Type, int MinimumDamage);

}
