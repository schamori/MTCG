using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Services;
using MTCG.Models;
using MTCG.Interface;
using MTCG.DAL;


namespace MTCG.Controller
{
    public class UserController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IAuthenticationService _authenticationService;

        public UserController(IUserService userservice, IAuthenticationService authenticationService)
        {
            _userservice = userservice;
            _authenticationService = authenticationService;
        }

        private HttpResponse login(Credentials credentials)
        {
            string? authToken = _userservice.LoginUser(credentials);
            if (authToken == null)
                return new HttpResponse(StatusCode.Unauthorized, "invalid username/password provided");
            return new HttpResponse(StatusCode.Ok, authToken);
        }
        private HttpResponse register(Credentials credentials)
        {     
            return _userservice.CreateUser(credentials) ? new HttpResponse(StatusCode.Created): new HttpResponse(StatusCode.Conflict, "User with same username already registered");
        }

        private HttpResponse getUserData(string? authToken, string? username)
        {
            if (username == null)
            {
                return new HttpResponse(StatusCode.NotFound, "User not found.");
            }
            User user = _authenticationService.AuthenticateUser(authToken, false);
            if (!user.isAdmin && username != user.username)
            {
                throw new AccessTokenException();
            }

            try
            {
                string? userData = _userservice.GetUserData(username);
                if (userData == null)
                {
                    return new HttpResponse(StatusCode.NotFound, "User found but Userdata was not set");
                } 
                return new HttpResponse(StatusCode.Ok, "Data successfully retrieved: \n" + userData);
            } catch (UserNotfoundException)
            {
                return new HttpResponse(StatusCode.NotFound, "User not found.");
            }
        }

        private HttpResponse setUserData(string? authToken, string? username, UserData userData)
        {
            if (username == null)
            {
                return new HttpResponse(StatusCode.NotFound, "User not found.");
            }
            User user = _authenticationService.AuthenticateUser(authToken, false);
            if (!user.isAdmin && username != user.username)
            {
                throw new AccessTokenException();
            }
            try
            {
                _userservice.UpdateUserData(username, userData);
            }
            catch (UserNotfoundException)
            {
                return new HttpResponse(StatusCode.NotFound, "User not found.");
            }
            return new HttpResponse(StatusCode.Created, "User sucessfully updated.");
        }


        public override HttpResponse HandleRequest(HttpRequest httpRequest)
        {
            return httpRequest switch
            {
                {route: "/users", method: HttpMethod.Post} => register((Credentials)ParseContent(httpRequest.content, typeof(Credentials))),
                { route: "/sessions", method: HttpMethod.Post } => login((Credentials)ParseContent(httpRequest.content, typeof(Credentials))),
                { route: "/users", method: HttpMethod.Get } => getUserData(httpRequest.authentication, httpRequest.subRouteParameter),
                { route: "/users", method: HttpMethod.Put } => setUserData(httpRequest.authentication, httpRequest.subRouteParameter, (UserData)ParseContent(httpRequest.content, typeof(UserData))),
                _ => new HttpResponse(StatusCode.NotFound)
            };
        }


    }
}
