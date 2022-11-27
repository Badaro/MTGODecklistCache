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
    public class BracketWithoutExtraMatchesLoaderTests
    {
        private Round[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/15/"),
                Date = new DateTime(2021, 04, 30, 00, 00, 00, DateTimeKind.Utc)
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
            _testData.Skip(1).First().RoundName.Should().Be("Semifinals");
            _testData.Skip(2).First().RoundName.Should().Be("Finals");
        }

        [Test]
        public void ShouldNotContainExtraBrackets()
        {
            _testData.Length.Should().Be(3);
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
                        new RoundItem(){ Player1 = "sandoiche",   Player2 = "MentalMisstep", Result= "2-0-0" },
                        new RoundItem(){ Player1 = "stefanogs",   Player2 = "Paradise_lost", Result= "2-0-0" },
                        new RoundItem(){ Player1 = "Darthkid",    Player2 = "Promidnightz",  Result= "2-0-0" },
                        new RoundItem(){ Player1 = "LynnChalice", Player2 = "joaofelipen72", Result= "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Semifinals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem(){ Player1 = "sandoiche",   Player2 = "stefanogs", Result= "2-1-0" },
                        new RoundItem(){ Player1 = "LynnChalice", Player2 = "Darthkid",  Result= "2-0-0" }
                    }
                },
                new Round()
                {
                    RoundName = "Finals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "sandoiche", Player2 = "LynnChalice", Result = "2-0-0" }
                    }
                }
            };

            _testData.Should().BeEquivalentTo(expected);
        }
    }
}
