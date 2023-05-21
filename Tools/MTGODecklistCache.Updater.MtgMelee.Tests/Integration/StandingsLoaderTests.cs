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
    public class StandingsLoaderTests
    {
        private Standing[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://melee.gg/Tournament/View/12867"),
                Date = new DateTime(2022, 11, 19, 00, 00, 00, DateTimeKind.Utc)
            }).Standings;
        }

        [Test]
        public void StandingsCountIsCorrect()
        {
            _testData.Length.Should().Be(6);
        }

        [Test]
        public void StandingsHavePlayers()
        {
            foreach (var standing in _testData) standing.Player.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void StandingsHaveRank()
        {
            foreach (var standing in _testData) standing.Rank.Should().BeGreaterThan(0);
        }

        [Test]
        public void StandingsHavePoints()
        {
            foreach (var standing in _testData) standing.Points.Should().BeGreaterThan(0);
        }

        [Test]
        public void StandingsHaveOMWP()
        {
            foreach (var standing in _testData) standing.OMWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void DecksHaveGWP()
        {
            foreach (var standing in _testData) standing.GWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void DecksHaveOGWP()
        {
            foreach (var standing in _testData) standing.OGWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void StandingDataIsCorrect()
        {
            Standing testStanding = _testData.FirstOrDefault();
            testStanding.Should().BeEquivalentTo(new Standing()
            {
                Rank = 1,
                Player = "リヒト ＿蝦夷決闘者",
                Points = 15,
                OMWP = 0.453333,
                GWP = 0.909091,
                OGWP = 0.45
            });
        }
    }
}
