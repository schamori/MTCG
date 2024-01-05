using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public record User(int Id, string username, string Token, int Coins, int Elo, bool isAdmin)
    {
        public override string ToString()
        {
            return username;
        }
    }


}
