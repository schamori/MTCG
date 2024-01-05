using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMITCG
{
    internal class TestGameService
    {
        private IGameService game;
        [SetUp]
        public void Setup()
        {
            var mockController = new Mock<ControllerBase>();

            game = new GameService(new Mock<IUserManager>().Object, new Mock<ICardManager>().Object);
        }
        // Id is irrelavant in this test case
        private const string Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

        static object[] BattleCardsCases = {
            new object[] {
                new Card(
                    Id,
                    "WaterTroll",
                    50,
                    CardType.Troll,
                    Element.Water
                    ),
                new Card(
                    Id,
                    "RegularGoblin",
                    50,
                    CardType.Goblin,
                    Element.Regular),
                Winner.Draw
            },
            new object[] {
                new Card(
                    Id,
                    "FireGoblin",
                    0,
                    CardType.Goblin,
                    Element.Fire
                    ),
                new Card(
                    Id,
                    "Dragon",
                    50,
                    CardType.Dragon,
                    Element.Fire),
                Winner.Second
            },
            new object[] {
                new Card(
                    Id,
                    "Wizzard",
                    0,
                    CardType.Wizzard,
                    Element.Fire
                    ),
                new Card(
                    Id,
                    "FireOrk",
                    50,
                    CardType.Ork,
                    Element.Fire),
                Winner.First
            },
            new object[] {
                new Card(
                    Id,
                    "WaterSpell",
                    0,
                    CardType.Spell,
                    Element.Water
                    ),
                new Card(
                    Id,
                    "Knight",
                    50,
                    CardType.Knight,
                    Element.Regular),
                Winner.First
            },
             new object[] {
                new Card(
                    Id,
                    "WaterSpell",
                    40,
                    CardType.Spell,
                    Element.Water
                    ),
                new Card(
                    Id,
                    "Kraken",
                    0,
                    CardType.Kraken,
                    Element.Water),
                Winner.Second
            },
             new object[] {
                new Card(
                    Id,
                    "FireElf",
                    0,
                    CardType.Elf,
                    Element.Fire),
                new Card(
                    Id,
                    "Dragon",
                    100,
                    CardType.Dragon,
                    Element.Fire),
                Winner.First
             },
             new object[] {
                new Card(
                    Id,
                    "WaterSpell",
                    10,
                    CardType.Spell,
                    Element.Water
                    ),
                new Card(
                    Id,
                    "Dragon",
                    30,
                    CardType.Dragon,
                    Element.Fire),
                Winner.First
             },
             new object[] {
                new Card(
                    Id,
                    "Knight",
                    10,
                    CardType.Knight,
                    Element.Regular
                    ),
                new Card(
                    Id,
                    "WaterSpell",
                    30,
                    CardType.Spell,
                    Element.Water),
                Winner.Second
             },
             new object[] {
                new Card(
                    Id,
                    "FireSpell",
                    20,
                    CardType.Spell,
                    Element.Fire
                    ),
                new Card(
                    Id,
                    "Knight",
                    80,
                    CardType.Knight,
                    Element.Regular
                    ),

                Winner.Draw
             },
             new object[] {
                new Card(
                    Id,
                    "Knight",
                    20,
                    CardType.Knight,
                    Element.Regular
                    ),
                new Card(
                    Id,
                    "Knight",
                    80,
                    CardType.Knight,
                    Element.Regular
                    ),

                Winner.Second
             },
             new object[] {
               new Card(
                    Id,
                    "Knight",
                    80,
                    CardType.Knight,
                    Element.Regular
                   ),
                new Card(
                    Id,
                    "FireSpell",
                    20,
                    CardType.Spell,
                    Element.Fire
                    ),
               Winner.Draw
             }

           };

        [Test]
        [TestCaseSource(nameof(BattleCardsCases))]
        public void TestBattleCards(Card card1, Card card2, Winner winner)
        {
            Assert.That(game.BattleCards(card1, card2), Is.EqualTo(winner));
        }
    }
}
