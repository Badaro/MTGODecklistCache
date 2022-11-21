using FluentAssertions;
using MTGODecklistCache.Updater.ManaTraders;
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

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://mtgmelee.com/Tournament/View/12867"),
                Date = new DateTime(2022, 11, 19, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds;
        }

        [Test]
        public void RoundCountIsCorrect()
        {
            _testData.Length.Should().Be(5);
        }

        [Test]
        public void RoundsHaveNumber()
        {
            foreach (var round in _testData) round.RoundNumber.Should().BeGreaterThan(0);
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
            testRound.RoundNumber.Should().Be(1);
            testRound.Matches.First().Should().BeEquivalentTo(new RoundItem()
            {
                Player1 = "リヒト ＿蝦夷決闘者",
                Player2 = "agesZ #84443",
                Result = "2-0-0"
            });
        }
    }
}
