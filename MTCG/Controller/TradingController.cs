using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Interface;
namespace MTCG.Controller
{
    internal class TradingController : ControllerBase
    {
        private ITradingService _tradingService;
        private ICardService _cardService;
        private IAuthenticationService _authenticationService;

        public TradingController(ITradingService tradingService, ICardService cardService, IAuthenticationService authenticationService)
        {
            _tradingService = tradingService;
            _cardService = cardService;
            _authenticationService = authenticationService;
        }

        private HttpResponse CreateTrade(Trade trade, string? authToken)
        {
            User user = _authenticationService.AuthenticateUser(authToken, false);
            List<String> cardToTrade = new();
            cardToTrade.Append(trade.Id);
            if (!_cardService.areCardsAvaliable(cardToTrade, user.Id)) return new HttpResponse(StatusCode.Forbidden, "The deal contains a card that is not owned by the user, locked in the deck or already in a Trade.");
            if (!_tradingService.CreateTrade(trade)) return new HttpResponse(StatusCode.Conflict, "A deal with this deal ID already exists.");
            return new HttpResponse(StatusCode.Created, "	Trading deal successfully created");
        }

        private HttpResponse ShowallTrades(string? authToken)
        {
            _authenticationService.AuthenticateUser(authToken, false);
            string? allTrades = _tradingService.GetallTrades();
            if (allTrades == null) return new HttpResponse(StatusCode.NoContent, "The request was fine, but there are no trading deals available");
            return new HttpResponse(StatusCode.Ok, "There are trading deals available, the response contains these " + allTrades);

        }
        private HttpResponse DeleteTrade(string? tradeId,  string? authToken)
        {
            if (tradeId == null) throw new InvalidDataException();
            User user = _authenticationService.AuthenticateUser(authToken, false);

            Trade? trade = _tradingService.GetTrade(tradeId);
            if (trade == null) return new HttpResponse(StatusCode.NotFound, "The provided deal ID was not found.");
            if (!_cardService.isCardfromUser(trade.CardToTrade, user.Id)) return new HttpResponse(StatusCode.Forbidden, "The deal contains a card that is not owned by the user.");
            // Already check all errors so no need for futher error handeling
            _tradingService.DeleteTrade(tradeId);
            return new HttpResponse(StatusCode.Ok, "Trading deal successfully deleted");
        }

        private HttpResponse AcceptTrade(string? tradeId, string? cardToTradeId, string? authToken)
        {


            if (tradeId == null || cardToTradeId == null ) throw new InvalidDataException();

            cardToTradeId = cardToTradeId.Trim('\\').Trim('\"');



            User user = _authenticationService.AuthenticateUser(authToken, false);
            Trade? trade = _tradingService.GetTrade(tradeId);
            if (trade == null) return new HttpResponse(StatusCode.NotFound, "The provided deal ID was not found.");
            if (_cardService.isCardfromUser(trade.CardToTrade, user.Id) || !_cardService.isCardfromUser(cardToTradeId, user.Id) 
                || _cardService.isCardinDeck(cardToTradeId, user.Id) || !_tradingService.isCardMeetingRequirements(trade, cardToTradeId)) 
                return new HttpResponse(StatusCode.Forbidden, "The offered card is not owned by the user, or the requirements are not met (Type, MinimumDamage), or the offered card is locked in the deck, or the user tries to trade with self.");

            _tradingService.ExecuteTrade(trade, cardToTradeId);
            return new HttpResponse(StatusCode.Ok, "Trading deal successfully executed.");

        }


        public override HttpResponse HandleRequest(HttpRequest httpRequest)
        {
            return httpRequest switch
            {
                { route: "/tradings", method: HttpMethod.Post, subRouteParameter: null } => CreateTrade((Trade)ParseContent(httpRequest.content, typeof(Trade)), httpRequest.authentication),
                { route: "/tradings", method: HttpMethod.Get } => ShowallTrades(httpRequest.authentication),
                { route: "/tradings", method: HttpMethod.Delete } => DeleteTrade(httpRequest.subRouteParameter, httpRequest.authentication),
                { route: "/tradings", method: HttpMethod.Post } => AcceptTrade(httpRequest.subRouteParameter, httpRequest.content, httpRequest.authentication),

                _ => new HttpResponse(StatusCode.NotFound)
            };
        }
    }
}
