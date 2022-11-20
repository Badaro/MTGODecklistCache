using FluentAssertions;
using MTGODecklistCache.Updater.ManaTraders;
using MTGODecklistCache.Updater.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    public class StandingsLoaderTests
    {
        private Standing[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/30/"),
                Date = new DateTime(2022, 08, 31, 00, 00, 00, DateTimeKind.Utc)
            }).Standings;
        }

        [Test]
        public void StandingsCountIsCorrect()
        {
            _testData.Length.Should().Be(195);
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
            foreach (var standing in _testData.Take(32)) standing.Points.Should().BeGreaterThan(0);
        }

        [Test]
        public void StandingsHaveOMWP()
        {
            foreach (var standing in _testData.Take(32)) standing.OMWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void DecksHaveGWP()
        {
            foreach (var standing in _testData.Take(32)) standing.GWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void DecksHaveOGWP()
        {
            foreach (var standing in _testData.Take(32)) standing.OGWP.Should().BeGreaterThan(0);
        }

        [Test]
        public void StandingDataIsCorrect()
        {
            Standing testStanding = _testData.FirstOrDefault();
            testStanding.Should().BeEquivalentTo(new Standing()
            {
                Rank = 1,
                Player = "Fink64",
                Points = 21,
                OMWP = 0.659,
                GWP = 0.75,
                OGWP = 0.584
            });
        }
    }
}
