using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.DAL
{
    public interface IUserManager
    {
        string? GetTokenByCredentials(Credentials credentials);
        bool InsertUser(Credentials credentials, string authToken);
        User? GetUserByAuthToken(string authToken);

        bool DecreaseCoinsAmound(int userId, int amount);

        string GetGameScoreboard();
        string GetUserScore(int userId);

        void changeUserElo(int userId, int amount);

        void changeWinLosses(int userId, bool win);
        void UpdateUserData(string username, UserData userData);

        string? GetUserData(string username);

        int GetUserElo(int userId);
    }
}

