using FluentAssertions;
using MTGODecklistCache.Updater.MtgMelee;
using MTGODecklistCache.Updater.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MtgMelee.Tests
{
    public class RoundsLoaderTests
    {
        private Round[] _testData = null;
        private Round[] _testData2 = null;
        private Round[] _testData3 = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://melee.gg/Tournament/View/12867"),
                Date = new DateTime(2022, 11, 19, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds;

            _testData2 = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://melee.gg/Tournament/View/7708"),
                Date = new DateTime(2021, 11, 09, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds;

            _testData3 = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://melee.gg/Tournament/View/12946"),
                Date = new DateTime(2022, 11, 20, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds;
        }

        [Test]
        public void RoundCountIsCorrect()
        {
            _testData.Length.Should().Be(5);
        }

        [Test]
        public void RoundsHaveName()
        {
            foreach (var round in _testData) round.RoundName.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void RoundsHaveMatches()
        {
            foreach (var round in _testData) round.Matches.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public void RoundDataIsCorrect()
        {
            Round testRound = _testData.First();
            testRound.RoundName.Should().Be("Round 1");
            testRound.Matches.First().Should().BeEquivalentTo(new RoundItem()
            {
                Player1 = "リヒト ＿蝦夷決闘者",
                Player2 = "agesZ #84443",
                Result = "2-0-0"
            });
        }

        [Test]
        public void ShouldParseByesCorrectly()
        {
            _testData2
                .Where(r => r.RoundName=="Round 3")
                .SelectMany(r => r.Matches)
                .First(r => r.Player1== "Er_gitta")
                .Should().BeEquivalentTo(new RoundItem()
                {
                    Player1 = "Er_gitta",
                    Player2 = "-",
                    Result = "2-0-0"
                });
        }
        [Test]
        public void ShouldParseDrawsCorrectly()
        {
            _testData3
                .Where(r => r.RoundName == "Round 5")
                .SelectMany(r => r.Matches)
                .First(r => r.Player1 == "Arthur Rodrigues")
                .Should().BeEquivalentTo(new RoundItem()
                {
                    Player1 = "Arthur Rodrigues",
                    Player2 = "RudsonC",
                    Result = "0-0-3"
                });
        }

        [Test]
        public void ShouldParseMissingOpponentCorrectly()
        {
            _testData2
                .Where(r => r.RoundName == "Round 4")
                .SelectMany(r => r.Matches)
                .First(r => r.Player1 == "Taerian van Rensburg")
                .Should().BeEquivalentTo(new RoundItem()
                {
                    Player1 = "Taerian van Rensburg",
                    Player2 = "-",
                    Result = "2-0-0"
                });
        }
    }
}
