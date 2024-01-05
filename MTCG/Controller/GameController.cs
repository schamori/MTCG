using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
using MTCG.Models;

namespace MTCG.Controller
{
    public class GameController: ControllerBase
    {
        private IGameService _gameService;
        private IAuthenticationService _authenticationService;

        public GameController(IGameService gameService,
            IAuthenticationService authenticationService)
        {
            _gameService = gameService;
            _authenticationService = authenticationService;
        }

        private HttpResponse GetUserStats(string? authToken)
        {
            User user = _authenticationService.AuthenticateUser(authToken, false);
            return new HttpResponse(StatusCode.Ok, "The stats could be retrieved successfully.\n" + _gameService.GetUserScore(user.Id));
        }

        private HttpResponse GetScoreboard(string? authToken)
        {
            _authenticationService.AuthenticateUser(authToken, false);
            return new HttpResponse(StatusCode.Ok, "The scoreboard could be retrieved successfully.\n" + _gameService.GetScoreboard());
        }

        private HttpResponse Battle(string? authToken)
        {
            User user = _authenticationService.AuthenticateUser(authToken, false);
            string? battleLog = _gameService.WaitOrStartBattle(user);
            if (battleLog == null) return new HttpResponse(StatusCode.NotFound, "The User deck was not set.\n");
            return new HttpResponse(StatusCode.Ok, "The battle has been carried out successfully.\n" + battleLog);
        }


        public override HttpResponse HandleRequest(HttpRequest httpRequest)
        {
            return httpRequest switch
            {
                { route: "/stats" } => GetUserStats(httpRequest.authentication),
                { route: "/scoreboard"} => GetScoreboard(httpRequest.authentication),
                { route: "/battles", method: HttpMethod.Post } => Battle(httpRequest.authentication),
                _ => new HttpResponse(StatusCode.NotFound)
            };
        }
    }
}
