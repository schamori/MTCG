namespace UnitTestMITCG
{
    public class TestPackageService
    {

        private IPackagesService package;
        [SetUp]
        public void Setup()
        {
            package = new PackageService();

        }
        static object[] CreatePackagesCases =
        {
            new object[] 
            {
                new List<Dictionary<string, string>>
                    {
                        new Dictionary<string, string>
                        {
                            { "Id", "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
                            { "Name", "WaterGoblin" },
                            { "Damage", "55" }
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
                            { "Name", "WaterSpell" },
                            { "Damage", "75" }
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
                            { "Name", "Knight" },
                            { "Damage", "100" }
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
                            { "Name", "RegularGoblin" },
                            { "Damage", "50" }
                        },
                        new Dictionary<string, string>
                        {
                            { "Id", "3fa85f64-5717-4562-b3fc-2c963f66afa6" },
                            { "Name", "WaterTroll" },
                            { "Damage", "40" }
                        }

                    },
                new List<Card>
                {
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "WaterGoblin",
                        55,
                        CardType.Monster,
                        Element.Water,
                        Species.Goblin
                        ),
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
                        CardType.Monster,
                        Element.Regular,
                        Species.Knight
                    ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "RegularGoblin",
                        50,
                        CardType.Monster,
                        Element.Regular,
                        Species.Goblin
                    ),
                    new Card(
                        "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                        "WaterTroll",
                        40,
                        CardType.Monster,
                        Element.Water,
                        Species.Troll
                    )

                }
             
            }
        };

        [Test]
        [TestCaseSource(nameof(CreatePackagesCases))]
        public void TestCreatePackages(List<Dictionary<string, string>> Cards, List<Card> expectedCard)
        {
            List<Card> newPackages = package.CreateNewPackage(Cards);
            for (int i = 0; i < Cards.Count; i++)
                Assert.That(expectedCard[i], Is.EqualTo(newPackages[i]));
        }
    }
}