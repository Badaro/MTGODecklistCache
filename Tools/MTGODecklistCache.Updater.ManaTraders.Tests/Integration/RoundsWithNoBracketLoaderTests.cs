using FluentAssertions;
using MTGODecklistCache.Updater.ManaTraders;
using MTGODecklistCache.Updater.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MagicGG.Tests
{
    public class RoundsWithNoBracketLoaderTests
    {
        private Round[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/34/"),
                Date = new DateTime(2022, 12, 31, 00, 00, 00, DateTimeKind.Utc)
            }).Rounds;
        }

        [Test]
        public void RoundCountIsCorrect()
        {
            _testData.Length.Should().Be(9);
        }

        [Test]
        public void RoundsHaveNumber()
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
                Player1 = "sneakymisato",
                Player2 = "Mogged",
                Result = "0-2-0"
            });
        }
    }
}
