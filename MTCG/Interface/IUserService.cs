using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
namespace MTCG.Interface
{
    public interface IUserService
    {
        bool CreateUser(Credentials credentials);
        bool UpdateUser(string username, string password);
        bool GetUser(string username);
        string? LoginUser(Credentials credentials);
        bool PayPackage(int userId, int amount);

        void UpdateUserData(string username, UserData userData);

        string? GetUserData(string username);


    }
}
