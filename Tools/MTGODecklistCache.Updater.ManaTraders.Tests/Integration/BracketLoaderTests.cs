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
    public class BracketLoaderTests
    {
        private Bracket _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/30/"),
                Date = new DateTime(2022, 08, 31, 00, 00, 00, DateTimeKind.Utc)
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
                    new BracketItem(){ Player1 = "zuri1988",    Player2 = "Fink64",     Result= "2-1" },
                    new BracketItem(){ Player1 = "kvza",        Player2 = "Harry13",    Result= "2-0" },
                    new BracketItem(){ Player1 = "ModiSapiras", Player2 = "Daking3603", Result= "2-0" },
                    new BracketItem(){ Player1 = "Cinciu",      Player2 = "ScouterTF2", Result= "2-0" }
                },
                Semifinals = new BracketItem[]
                {
                    new BracketItem(){ Player1 = "kvza",        Player2 = "zuri1988", Result= "2-1" },
                    new BracketItem(){ Player1 = "ModiSapiras", Player2 = "Cinciu",   Result= "2-0" }
                },
                Finals = new BracketItem() { Player1 = "ModiSapiras", Player2 = "kvza", Result = "2-0" }
            });
        }
    }
}
