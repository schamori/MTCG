using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using MTCG.Models;
using MTCG.Interface;
using MTCG.Controller;
using MTCG.Services;
using Npgsql;


namespace MTCG.Server
{
    public class HttpService: IHttpService
    {
        private ControllerBase _userController;
        private ControllerBase _packageController;
        private ControllerBase _cardController;
        private ControllerBase _gameController;
        private ControllerBase _tradingController;
        private Dictionary<string, RouteEnum> routeDictionary;


        public HttpService(ControllerBase userController, ControllerBase packageController, 
            ControllerBase cardController, ControllerBase gameController, ControllerBase tradingController)
        {
            _userController = userController;
            _packageController = packageController;
            _cardController = cardController;
            _gameController = gameController;
            _tradingController = tradingController;


            routeDictionary = new Dictionary<string, RouteEnum>
            {
            { "/users", RouteEnum.Users },
            { "/sessions", RouteEnum.Users },
            { "/packages", RouteEnum.Packages },
            { "/transactions/packages", RouteEnum.Packages },
            { "/cards", RouteEnum.Cards },
            { "/deck", RouteEnum.Cards },
            { "/stats", RouteEnum.Game },
            { "/scoreboard", RouteEnum.Game },
            { "/battles", RouteEnum.Game },
            { "/tradings", RouteEnum.Trading },
            };
        }



        public HttpRequest? Parse(string requestString)
        {
            string? method = null;
            string? route = null;
            bool contentPart = false;
            string? authToken = null;
            StringBuilder content = new StringBuilder();

            int i = 0;
            foreach (string httpLine in requestString.Split('\n'))
            {
                string[] httpWords = httpLine.Trim().Split(' ');

                if (contentPart)
                {
                    content.Append(httpLine);
                }
                else if (i == 0)
                {
                    method = httpWords[0];
                    route = httpWords[1];
                }
                else if (httpWords[0] == "Authorization:")
                {
                    authToken = httpWords[2];
                }
                else if (httpLine == "\r")
                {
                    contentPart = true;
                }
                i++;
            }


            string? httpContent = content.Length > 0 ? content.ToString().Trim() : null;

            if (method == null || route == null) return null;

            // The only route that has two routes
            string? subRouteParameter = null;
            if (!route.StartsWith("/transactions"))
            {
                Match routeMatch = Regex.Match(route, @"^(/[^/]+)/([^/]+)/?$");

                if (routeMatch.Success)
                {
                    route = routeMatch.Groups[1].Value;
                    if (routeMatch.Groups.Count >= 2)
                    {
                        subRouteParameter = routeMatch.Groups[2].Value;

                    }
                }
            }
            Regex regex = new Regex(@"\?format=(\w+)");
            Match match = regex.Match(route);

            bool plain = false;
            if (match.Success)
            {
                plain = match.Groups[1].Value == "plain";
            }
            try
            {
                GetMethod(method);
            }
            catch (InvalidDataException)
            {
                return null; 
            }

            int questionMarkIndex = route.IndexOf('?');

            if (questionMarkIndex != -1) // Check if the '?' exists
            {
                route = route.Substring(0, questionMarkIndex);
            }

            return new HttpRequest(GetMethod(method), route, subRouteParameter, authToken, httpContent, plain);
        }

        public string Route(HttpRequest? httpRequest)
        {
            if (httpRequest == null)
            {
                return SendResponse(new HttpResponse(StatusCode.BadRequest));
            }
            try
            {
                if (routeDictionary.ContainsKey(httpRequest.route))
                {
                    switch (routeDictionary[httpRequest.route])
                    {
                        case RouteEnum.Users:
                            // Todo GET PUT
                            return SendResponse(_userController.HandleRequest(httpRequest));
                        case RouteEnum.Packages:
                            return SendResponse(_packageController.HandleRequest(httpRequest));

                        case RouteEnum.Cards:
                            return SendResponse(_cardController.HandleRequest(httpRequest));

                        case RouteEnum.Game:
                            return SendResponse(_gameController.HandleRequest(httpRequest));

                        case RouteEnum.Trading:
                            return SendResponse(_tradingController.HandleRequest(httpRequest));

                        default:
                            return SendResponse(new HttpResponse(StatusCode.NotFound));

                    }
                }
                else
                {
                    return SendResponse(new HttpResponse(StatusCode.NotFound));
                }

            }
            catch (InvalidDataException)
            {
                return SendResponse(new HttpResponse(StatusCode.BadRequest));
            }
            catch (PostgresException ex)
            {
                Console.WriteLine("PostgresException: " + ex.Message);

                return SendResponse(new HttpResponse(StatusCode.InternalServerError, ex.Message));
            }
            catch (AccessTokenException ex)
            {

                return ex.NeedsAdmin? SendResponse(new HttpResponse(StatusCode.Forbidden, @"Provided user is not ""admin""")) 
                    : SendResponse(new HttpResponse(StatusCode.Unauthorized, "Access token is missing or invalid"));
            } catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Console.WriteLine("Route: " + httpRequest.route);
                return SendResponse(new HttpResponse(StatusCode.InternalServerError));


            }
        }

        public HttpMethod GetMethod(string method)
        {
            return method.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                "patch" => HttpMethod.Patch,
                _ => throw new InvalidDataException()
            };
        }

        private string SendResponse(HttpResponse response)
        {
            StringBuilder reponse = new StringBuilder();

            reponse.Append($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}\r\n");
            if (!string.IsNullOrEmpty(response.Payload))
            {
                reponse.Append($"Content-Length: {response.Payload.Length}\r\n");
                reponse.Append("\r\n");
                reponse.Append(response.Payload);
            }
            else
            {
                reponse.Append("\r\n");
            }
            return reponse.ToString();
        }
    }

}
