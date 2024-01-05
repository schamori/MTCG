using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestMITCG
{
    internal class CardHelperTest
    {

        static object[] CardMapJsonResponse = {
            new object[] {
                new List<Card>{
                new Card(
                    "123",
                    "WaterTroll",
                    50,
                    CardType.Troll,
                    Element.Water
                    ),
                new Card(
                    "124",
                    "RegularGoblin",
                    50,
                    CardType.Goblin,
                    Element.Regular)
                },
                  @"[
  {
    ""Id"": ""123"",
    ""Name"": ""WaterTroll"",
    ""Type"": 7,
    ""Element"": 1,
    ""Damage"": 50.0
  },
  {
    ""Id"": ""124"",
    ""Name"": ""RegularGoblin"",
    ""Type"": 0,
    ""Element"": 2,
    ""Damage"": 50.0
  }
]"
            }, 
            new object?[] { null, null }
           };

        [Test]
        [TestCaseSource(nameof(CardMapJsonResponse))]
        public void TestCardToJsonResponse(List<Card>? cards, string? response)
        {
            Assert.That(CardHelper.MapCardsToResponse(cards), Is.EqualTo(response), $"Expected: {CardHelper.MapCardsToResponse(cards)}, But was: {response}");
        }

        static object[] CardMapPlainResponse = {
            new object[] {
                new List<Card>{
                    new Card(
                        "123",
                        "WaterTroll",
                        50,
                        CardType.Troll,
                        Element.Water
                        ),
                    new Card(
                        "124",
                        "RegularGoblin",
                        50,
                        CardType.Goblin,
                        Element.Regular),
                },
                @"|||||---------------------------------------------------------------|||||
     Id = 123
     Name = WaterTroll
     Damage = 50
     ---------------------------------------------------------------      
     Id = 124
     Name = RegularGoblin
     Damage = 50
|||||---------------------------------------------------------------|||||
"
            }
            };
        [Test]
        [TestCaseSource(nameof(CardMapPlainResponse))]
        public void TestCardToPlainResponse(List<Card> cards, string response)
        {
            Assert.That(CardHelper.MapCardsToResponse(cards, true), Is.EqualTo(response.Replace("\r", "")), $"Expected: {CardHelper.MapCardsToResponse(cards, true)}, But was: {response.Replace("\r", "")}");
        }
    }
}
