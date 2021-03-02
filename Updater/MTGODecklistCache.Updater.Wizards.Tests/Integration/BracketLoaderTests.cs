using FluentAssertions;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Wizards;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    abstract class BracketLoaderTests
    {
        private Bracket _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentDetailsLoader.GetTournamentDetails(this.GetEventUri()).Bracket;
        }

        [Test]
        public void BracketItemCountIsCorrect()
        {
            if (_testData != null) _testData.Quarterfinals.Length.Should().Be(4);
            if (_testData != null) _testData.Semifinals.Length.Should().Be(2);
        }

        [Test]
        public void BracketItemsHaveWinningPlayer()
        {
            if (_testData != null) foreach (var match in _testData.Quarterfinals) match.Player1.Should().NotBeNullOrEmpty();
            if (_testData != null) foreach (var match in _testData.Semifinals) match.Player1.Should().NotBeNullOrEmpty();
            if (_testData != null) _testData.Finals.Player1.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void BracketItemsHaveLosingPlayer()
        {
            if (_testData != null) foreach (var match in _testData.Quarterfinals) match.Player2.Should().NotBeNullOrEmpty();
            if (_testData != null) foreach (var match in _testData.Semifinals) match.Player2.Should().NotBeNullOrEmpty();
            if (_testData != null) _testData.Finals.Player2.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void BracketItemsHaveResult()
        {
            if (_testData != null) foreach (var match in _testData.Quarterfinals) match.Result.Should().NotBeNullOrEmpty();
            if (_testData != null) foreach (var match in _testData.Semifinals) match.Result.Should().NotBeNullOrEmpty();
            if (_testData != null) _testData.Finals.Result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void BracketItemsDataIsCorrect()
        {
            if (_testData != null) _testData.Should().BeEquivalentTo(this.GetBracket());
        }

        protected abstract Uri GetEventUri();
        protected abstract Bracket GetBracket();
    }
}
