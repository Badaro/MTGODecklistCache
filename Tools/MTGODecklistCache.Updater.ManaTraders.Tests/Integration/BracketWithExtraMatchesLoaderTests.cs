using FluentAssertions;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.ManaTraders;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.ManaTraders.Tests
{
    public class BracketWithExtraMatchesLoaderTests
    {
        private Round[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/30/"),
                Date = new DateTime(2022, 08, 31, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds.Where(r => !r.RoundName.StartsWith("Round")).ToArray();
        }

        [Test]
        public void BracketItemCountIsCorrect()
        {
            _testData.First(r => r.RoundName == "Quarterfinals").Matches.Length.Should().Be(4);
            _testData.First(r => r.RoundName == "Semifinals").Matches.Length.Should().Be(2);
            _testData.First(r => r.RoundName == "Finals").Matches.Length.Should().Be(1);
        }

        [Test]
        public void BracketItemsHaveWinningPlayer()
        {
            foreach (var round in _testData)
            {
                foreach (var match in round.Matches) match.Player1.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void BracketItemsHaveLosingPlayer()
        {
            foreach (var round in _testData)
            {
                foreach (var match in round.Matches) match.Player2.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void BracketItemsHaveResult()
        {
            foreach (var round in _testData)
            {
                foreach (var match in round.Matches) match.Result.Should().NotBeNullOrEmpty();
            }
        }

        [Test]
        public void BracketRoundsShouldBeInCorrectOrder()
        {
            _testData.First().RoundName.Should().Be("Quarterfinals");
            _testData.Skip(1).First().RoundName.Should().Be("Loser Semifinals");
            _testData.Skip(2).First().RoundName.Should().Be("Semifinals");
            _testData.Skip(3).First().RoundName.Should().Be("Match for 7th and 8th places");
            _testData.Skip(4).First().RoundName.Should().Be("Match for 5th and 6th places");
            _testData.Skip(5).First().RoundName.Should().Be("Match for 3rd and 4th places");
            _testData.Skip(6).First().RoundName.Should().Be("Finals");
        }


        [Test]
        public void ShouldContainExtraBrackets()
        {
            _testData.Length.Should().Be(7);
        }

        [Test]
        public void BracketItemsDataIsCorrect()
        {
            var expected = new Round[]
            {
                new Round()
                {
                    RoundName = "Quarterfinals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem(){ Player1 = "zuri1988",    Player2 = "Fink64",     Result= "2-1-0" },
                        new RoundItem(){ Player1 = "kvza",        Player2 = "Harry13",    Result= "2-0-0" },
                        new RoundItem(){ Player1 = "ModiSapiras", Player2 = "Daking3603", Result= "2-0-0" },
                        new RoundItem(){ Player1 = "Cinciu",      Player2 = "ScouterTF2", Result= "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Loser Semifinals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem(){ Player1 = "Harry13",    Player2 = "Fink64", Result= "2-0-0" },
                        new RoundItem(){ Player1 = "Daking3603", Player2 = "ScouterTF2",   Result= "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Semifinals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem(){ Player1 = "kvza",        Player2 = "zuri1988", Result= "2-1-0" },
                        new RoundItem(){ Player1 = "ModiSapiras", Player2 = "Cinciu",   Result= "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Match for 7th and 8th places",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "ScouterTF2", Player2 = "Fink64", Result = "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Match for 5th and 6th places",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "Daking3603", Player2 = "Harry13", Result = "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Match for 3rd and 4th places",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "Cinciu", Player2 = "zuri1988", Result = "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Finals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "ModiSapiras", Player2 = "kvza", Result = "2-0-0" }
                    }
                },
            };

            _testData.Should().BeEquivalentTo(expected);
        }
    }
}
