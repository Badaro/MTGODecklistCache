using FluentAssertions;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Mtgo;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    abstract class BracketLoaderTests
    {
        private Round[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = this.GetEventUri()
            }).Rounds;
        }


        [Test]
        public void BracketDataPresentWhenExpected()
        {
            if (this.GetBracket() != null && _testData == null) Assert.Fail();
        }


        [Test]
        public void BracketItemCountIsCorrect()
        {
            if (_testData != null)
            {
                _testData.FirstOrDefault(r => r.RoundName == "Quarterfinals").Matches.Length.Should().Be(4);
                _testData.FirstOrDefault(r => r.RoundName == "Semifinals").Matches.Length.Should().Be(2);
                _testData.FirstOrDefault(r => r.RoundName == "Finals").Matches.Length.Should().Be(1);
            }
        }

        [Test]
        public void BracketItemsHaveWinningPlayer()
        {
            if (_testData != null)
            {
                foreach (var round in _testData)
                {
                    foreach (var match in round.Matches) match.Player1.Should().NotBeNullOrEmpty();
                }
            }
        }

        [Test]
        public void BracketItemsHaveLosingPlayer()
        {
            if (_testData != null)
            {
                foreach (var round in _testData)
                {
                    foreach (var match in round.Matches) match.Player2.Should().NotBeNullOrEmpty();
                }
            }
        }

        [Test]
        public void BracketItemsHaveResult()
        {
            if (_testData != null)
            {
                foreach (var round in _testData)
                {
                    foreach (var match in round.Matches) match.Result.Should().NotBeNullOrEmpty();
                }
            }
        }

        [Test]
        public void BracketItemsDataIsCorrect()
        {
            if (_testData != null) _testData.Should().BeEquivalentTo(this.GetBracket());
        }

        [Test]
        public void BracketRoundsShouldBeInCorrectOrder()
        {
            if (_testData != null)
            {
                _testData.First().RoundName.Should().Be("Quarterfinals");
                _testData.Skip(1).First().RoundName.Should().Be("Semifinals");
                _testData.Skip(2).First().RoundName.Should().Be("Finals");
            }
        }

        protected abstract Uri GetEventUri();
        protected abstract Round[] GetBracket();
    }
}
