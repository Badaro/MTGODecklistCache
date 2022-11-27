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
        private RoundV2[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/30/"),
                Date = new DateTime(2022, 08, 31, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds.TakeLast(3).ToArray();
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
        public void BracketItemsDataIsCorrect()
        {
            _testData.Should().BeEquivalentTo(new RoundV2[]
            {
                new RoundV2()
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
                new RoundV2()
                {
                    RoundName = "Semifinals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem(){ Player1 = "kvza",        Player2 = "zuri1988", Result= "2-1-0" },
                        new RoundItem(){ Player1 = "ModiSapiras", Player2 = "Cinciu",   Result= "2-0-0" }
                    }
                },
                new RoundV2()
                {
                    RoundName = "Finals",
                    Matches = new RoundItem[]
                    {
                        new RoundItem() { Player1 = "ModiSapiras", Player2 = "kvza", Result = "2-0-0" }
                    }
                },
            });
        }
    }
}
