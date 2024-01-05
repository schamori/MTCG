using MTCG.Services;
using MTCG.Models;
using MTCG.Interface;
using MTCG.Server;
using MTCG.DAL;
using MTCG.Controller;

namespace MTCG
{
    internal class Program
    {
        /* private IServiceProvider GetService()
        {
            ServiceCollection services = new();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<IPackageService, PackageService>();
            services.AddTransient<IUserManager, UserManager>();
            services.AddTransient<UserController>();
            return services.BuildServiceProvider();
        } */
        static async Task Main(string[] args)
        {
            // psql -h localhost -U postgres -d mtcg

            var connectionString = "Host=localhost;Username=postgres;Password=password;Database=mtcg;Include Error Detail=True";


            IUserManager userManager = new UserManager(connectionString);
            ICardManager cardsManager = new CardsManager(connectionString);
            ITradingManager tradingManager = new TradingManager(connectionString);


            IAuthenticationService authenticationService = new AuthenticationService(userManager);
            IUserService userService = new UserService(userManager);
            IPackageService packageService = new PackageService(cardsManager);
            ICardService cardService = new CardService(cardsManager);
            IGameService gameService = new GameService(userManager, cardsManager);
            ITradingService tradingService = new TradingService(tradingManager, cardsManager);

            ControllerBase packageController = new PackageController(packageService, userService, authenticationService);
            ControllerBase userController = new UserController(userService, authenticationService);
            ControllerBase cardController = new CardController(cardService, authenticationService);
            ControllerBase gameController = new GameController(gameService, authenticationService);
            ControllerBase tradingController = new TradingController(tradingService, cardService, authenticationService);


            IHttpService httpService = new HttpService(userController, packageController, cardController, gameController, tradingController);

            TCPServer server = new TCPServer(10001, httpService);

            await server.Listen();

        }
    }
}