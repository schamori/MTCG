using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using MTCG.Services;
using MTCG.Interface;


namespace MTCG.Controller
{
    public class PackageController: ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;

        public PackageController(IPackageService packageService, IUserService userService, 
            IAuthenticationService authenticationService)
        {
            _packageService = packageService;
            _authenticationService = authenticationService;
            _userService = userService;
        }

        private HttpResponse createPackage(List<RawRequestCard> Packages, string? token)
        {
            _authenticationService.AuthenticateUser(token, true);

            List<Card> cards = _packageService.CreateNewPackage(Packages);
            return _packageService.SavePackage(cards) ? new HttpResponse(StatusCode.Created, "Package and cards successfully created") : new HttpResponse(StatusCode.Conflict, "At least one card in the packages already exists");
        }

        private HttpResponse assignPackage(string? authToken)
        {
            User user = _authenticationService.AuthenticateUser(authToken, false);
            if (!_packageService.isPackageAvaliabe()) 
                return new HttpResponse(StatusCode.NotFound, "No card package available for buying");
            if (!_userService.PayPackage(user.Id, 5))
                return new HttpResponse(StatusCode.Forbidden, "Not enough money for buying a card package");
            return new HttpResponse(StatusCode.Ok, _packageService.AssignUserToPackage(user.Id));



        }
        public override HttpResponse HandleRequest(HttpRequest httpRequest)
        {
            return httpRequest switch
            {
                { route: "/packages"} => createPackage((List<RawRequestCard>)ParseContent(httpRequest.content, typeof(List<RawRequestCard>)), httpRequest.authentication),
                { route: "/transactions/packages"} => assignPackage(httpRequest.authentication),
               
                _ => new HttpResponse(StatusCode.NotFound)
            };
        }
    }
    }

