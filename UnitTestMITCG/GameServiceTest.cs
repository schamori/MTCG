using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMITCG
{
    internal class GameServiceTest
    {
        private IGameService game;
        [SetUp]
        public void Setup()
        {
            game = new GameService();
        }
        // Id is irrelavant in this test case
        private const string Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6";

        static object[] BattleCardsCases = {
            new object[] {
                new Card(
                    Id,
                    "WaterTroll",
                    50,
                    CardType.Monster,
                    Element.Water,
                    Species.Troll
                    ),
                new Card(
                    Id,
                    "RegularGoblin",
                    50,
                    CardType.Monster,
                    Element.Regular,
                    Species.Goblin),
                2
            },
            new object[] {
                new Card(
                    Id,
                    "FireGoblin",
                    0,
                    CardType.Monster,
                    Element.Fire,
                    Species.Goblin
                    ),
                new Card(
                    Id,
                    "Dragon",
                    50,
                    CardType.Monster,
                    Element.Fire,
                    Species.Dragon),
                1
            },
            new object[] {
                new Card(
                    Id,
                    "Wizzard",
                    0,
                    CardType.Monster,
                    Element.Fire,
                    Species.Wizzard
                    ),
                new Card(
                    Id,
                    "FireOrk",
                    50,
                    CardType.Monster,
                    Element.Fire,
                    Species.Ork),
                0
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
                    "Knights",
                    50,
                    CardType.Monster,
                    Element.Regular,
                    Species.Knight),
                0
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
                    CardType.Monster,
                    Element.Water,
                    Species.Kraken),
                1
            },
             new object[] {
                new Card(
                    Id,
                    "FireElf",
                    0,
                    CardType.Monster,
                    Element.Fire,
                    Species.Elf
                    ),
                new Card(
                    Id,
                    "Dragon",
                    100,
                    CardType.Monster,
                    Element.Fire,
                    Species.Dragon
                    ),
                0
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
                    CardType.Monster,
                    Element.Fire,
                    Species.Dragon),
                0
             },
             new object[] {
                new Card(
                    Id,
                    "Knight",
                    10,
                    CardType.Monster,
                    Element.Regular,
                    Species.Knight
                    ),
                new Card(
                    Id,
                    "WaterSpell",
                    30,
                    CardType.Spell,
                    Element.Water),
                0
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
                    CardType.Monster,
                    Element.Regular,
                    Species.Knight
                    ),

                2
             }
           };

        [Test]
        [TestCaseSource(nameof(BattleCardsCases))]
        public void TestBattleCards(Card card1, Card card2, int winner)
        {
            Assert.That(winner, Is.EqualTo(game.BattleCards(card1, card2)));
        }
    }
}
