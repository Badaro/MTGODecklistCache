using FluentAssertions;
using MTGODecklistCache.Updater.Mtgo;
using MTGODecklistCache.Updater.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    abstract class DeckLoaderTests
    {
        private Deck[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = this.GetEventUri()
            }).Decks;
        }

        [Test]
        public void DeckCountIsCorrect()
        {
            _testData.Length.Should().Be(this.GetDeckCount());
        }

        [Test]
        public void DecksHaveDate()
        {
            foreach (var deck in _testData) deck.Date.Should().Be(this.GetDeckDate());
        }

        [Test]
        public void DecksHavePlayers()
        {
            foreach (var deck in _testData) deck.Player.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void DecksHaveMainboards()
        {
            foreach (var deck in _testData) deck.Mainboard.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public void DecksHaveSideboards()
        {
            foreach (var deck in _testData) deck.Sideboard.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public void DecksHaveValidMainboards()
        {
            foreach (var deck in _testData) deck.Mainboard.Sum(i => i.Count).Should().BeGreaterOrEqualTo(60); ;
        }

        [Test]
        public void DecksHaveValidSideboards()
        {
            foreach (var deck in _testData) deck.Sideboard.Sum(i => i.Count).Should().BeLessOrEqualTo(15);
        }

        [Test]
        public void DeckDataIsCorrect()
        {
            Deck testDeck = _testData.First();
            testDeck.Should().BeEquivalentTo(this.GetFirstDeck());
        }

        protected abstract Uri GetEventUri();
        protected abstract DateTime GetEventDate();
        protected abstract DateTime? GetDeckDate();
        protected abstract int GetDeckCount();
        protected abstract Deck GetFirstDeck();
    }
}
