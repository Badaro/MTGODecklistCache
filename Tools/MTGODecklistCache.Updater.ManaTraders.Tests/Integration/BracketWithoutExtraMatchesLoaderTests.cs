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
        private Bracket _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/15/"),
                Date = new DateTime(2021, 04, 30, 00, 00, 00, DateTimeKind.Utc)
            }).Bracket;
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
            if (_testData != null) _testData.Should().BeEquivalentTo(new Bracket()
            {
                Quarterfinals = new BracketItem[]
                {
                    new BracketItem(){ Player1 = "sandoiche",    Player2 = "MentalMisstep",     Result= "2-0" },
                    new BracketItem(){ Player1 = "stefanogs",        Player2 = "Paradise_lost",    Result= "2-0" },
                    new BracketItem(){ Player1 = "Darthkid", Player2 = "Promidnightz", Result= "2-0" },
                    new BracketItem(){ Player1 = "LynnChalice",      Player2 = "joaofelipen72", Result= "2-0" }
                },
                Semifinals = new BracketItem[]
                {
                    new BracketItem(){ Player1 = "sandoiche",        Player2 = "stefanogs", Result= "2-1" },
                    new BracketItem(){ Player1 = "LynnChalice", Player2 = "Darthkid",   Result= "2-0" }
                },
                Finals = new BracketItem() { Player1 = "sandoiche", Player2 = "LynnChalice", Result = "2-0" }
            });
        }
    }
}
