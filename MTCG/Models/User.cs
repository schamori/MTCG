using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{



    public class User
    {
        public Stack Cards { get; }
        public int Coints { get; set; }
        public string Name { get; }

        public string Password { get; }


        public User(string Username, string Password, int coints)
        {
            Name = Username;
            this.Password = Password;
            Coints = coints;
            Cards = new Stack();
        }

    }
}
