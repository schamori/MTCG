using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;
using MTCG.DAL;



namespace MTCG.Services
{
    public class UserService : IUserService
    {
        private readonly IUserManager _userDao;

        public UserService(IUserManager userDao)
        {
            _userDao = userDao;
        }
        private string GenerateToken(int length)
        {
            Random random = new Random();
            // Dont allow special characters since this can mess with the curl request
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            char[] token = new char[length];

            for (int i = 0; i < length; i++)
            {
                token[i] = validChars[random.Next(validChars.Length)];
            }

            return new string(token);
        }
        
        public bool CreateUser(Credentials credentials)
        {
            return _userDao.InsertUser(credentials, credentials.Username + "-mtcgToken");
        }

        public bool GetUser(string authToken)
        {
            throw new NotImplementedException();
        }

        public string? LoginUser(Credentials credentials)
        {
            return _userDao.GetTokenByCredentials(credentials);
        }


        public bool UpdateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public bool PayPackage(int userId, int amount)
        {
            return _userDao.DecreaseCoinsAmound(userId, amount);
        }

        public void UpdateUserData(string username, UserData userData)
        {
            _userDao.UpdateUserData(username, userData);
        }

        public string? GetUserData(string username)
        {
            return _userDao.GetUserData(username);
        }
    }
}
