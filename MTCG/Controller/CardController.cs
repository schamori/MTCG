using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Services;
using MTCG.Interface;


namespace MTCG.Controller
{
    public class CardController : ControllerBase

    {
        private ICardService _cardService;
        private IAuthenticationService _authenticationService;

        public CardController(ICardService cardService, 
            IAuthenticationService authenticationService)
        {
            _cardService = cardService;
            _authenticationService = authenticationService;
        }
        private HttpResponse ShowUserCards(string? token)
        {
            User user = _authenticationService.AuthenticateUser(token, false);
            string? responseCards = _cardService.GetUserCards(user.Id);
            return responseCards != null ? new HttpResponse(StatusCode.Ok, responseCards) :
                new HttpResponse(StatusCode.NoContent, "The request was fine, but the user doesn't have any cards");
        }
        private HttpResponse ShowUserDeck(string? token, bool plain)
        {

            User user = _authenticationService.AuthenticateUser(token, false);
            string? responseCards = _cardService.GetUserDeck(user.Id, plain);
            return responseCards != null ? new HttpResponse(StatusCode.Ok, responseCards) :
                new HttpResponse(StatusCode.NoContent, "The request was fine, but the user doesn't have any cards");
        }


        private HttpResponse ConfigureUserDeck(List<String> cardIds, string? token)
        {
            if (cardIds.Count != 4)
                return new HttpResponse(StatusCode.NoContent, "The request was fine, but the deck doesn't have any cards");

            User user = _authenticationService.AuthenticateUser(token, false);

            
            return _cardService.SetUserDeck(user.Id, cardIds) ? new HttpResponse(StatusCode.Ok, "The deck has been successfully configured") :
                new HttpResponse(StatusCode.Forbidden, "At least one of the provided cards does not belong to the user or is not available.");






        }

        public override HttpResponse HandleRequest(HttpRequest httpRequest)
        {
            return httpRequest switch
            {
                { route: "/cards" , method: HttpMethod.Get} => ShowUserCards(httpRequest.authentication),
                { route: "/deck",  method: HttpMethod.Get } => ShowUserDeck(httpRequest.authentication, httpRequest.plain),
                { route: "/deck", method: HttpMethod.Put } => ConfigureUserDeck((List<String>)ParseContent(httpRequest.content, typeof(List<String>)), httpRequest.authentication),

                _ => new HttpResponse(StatusCode.NotFound)
            };
        }
    }
}
