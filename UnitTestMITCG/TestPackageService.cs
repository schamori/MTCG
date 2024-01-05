  namespace UnitTestMITCG
{
    public class TestPackageService
    {

        private IPackageService _package;
        [SetUp]
        public void Setup()
        {
            var mockController = new Mock<ICardManager>();

            _package = new PackageService(mockController.Object);

        }
        static object[] CreatePackagesCases =
        {
            new object[] 
            {
                new List<RawRequestCard>
                    {
                        new RawRequestCard("3fa85f64-5717-4562-b3fc-2c963f66afa6", "WaterGoblin", 55),
                        new RawRequestCard("3fa85f64-5717-4562-b3fc-2c963f66afa6", "WaterSpell", 75),
                        new RawRequestCard("3fa85f64-5717-4562-b3fc-2c963f66afa6", "Knight", 100),
                        new RawRequestCard("3fa85f64-5717-4562-b3fc-2c963f66afa6", "RegularGoblin", 50),
                        new RawRequestCard("3fa85f64-5717-4562-b3fc-2c963f66afa6", "WaterTroll", 40)

                    },
                new List<Card>
                {
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "WaterGoblin",
                        55,
                        CardType.Goblin,
                        Element.Water                        ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "WaterSpell",
                        75,
                        CardType.Spell,
                        Element.Water                    
                        ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "Knight",
                        100,
                        CardType.Knight,
                        Element.Regular                    
                        ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "RegularGoblin",
                        50,
                        CardType.Goblin,
                        Element.Regular
                    ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "WaterTroll",
                        40,
                        CardType.Troll,
                        Element.Water
                    )

                }
             
            }
        };

        [Test]
        [TestCaseSource(nameof(CreatePackagesCases))]
        public void TestCreatePackages(List<RawRequestCard> Cards, List<Card> expectedCard)
        {
            List<Card> newPackages = _package.CreateNewPackage(Cards);
            for (int i = 0; i < Cards.Count; i++)
                Assert.That(expectedCard[i], Is.EqualTo(newPackages[i]));
        }
    }
}