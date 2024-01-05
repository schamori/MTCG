using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.DAL;
using MTCG.Models;
using MTCG.Interface;


namespace MTCG.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserManager _userDao;

        public AuthenticationService(IUserManager userDao)
        {
            _userDao = userDao;
        }
        public User AuthenticateUser(string? authtoken, bool isAdmin)
        {
            if (authtoken == null)
                throw new AccessTokenException();

            User? user = _userDao.GetUserByAuthToken(authtoken);
            if (user == null)
                throw new AccessTokenException();
            if (isAdmin && !user.isAdmin)
                throw new AccessTokenException(true); 

            return user;
        }
    }
    public class AccessTokenException : Exception
    {
        public bool NeedsAdmin { get; }

        public AccessTokenException(bool needsAdmin = false)
        {
            NeedsAdmin = needsAdmin;
        }
    }
}
