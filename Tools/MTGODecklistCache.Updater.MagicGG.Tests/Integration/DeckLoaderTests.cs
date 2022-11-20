using FluentAssertions;
using MTGODecklistCache.Updater.MagicGG;
using MTGODecklistCache.Updater.Model;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MagicGG.Tests
{
    public class DeckLoaderTests
    {
        private Deck[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://magic.gg/decklists/2021-magic-online-champions-showcase-season-1-modern-decklists")
            }).Decks;
        }

        [Test]
        public void DeckCountIsCorrect()
        {
            _testData.Length.Should().Be(8);
        }

        [Test]
        public void DecksHaveDate()
        {
            foreach (var deck in _testData) deck.Date.Should().Be(new DateTime(2021, 06, 26, 00, 00, 00, DateTimeKind.Utc));
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
            testDeck.Should().BeEquivalentTo(new Deck()
            {
                Player = "Ryan Haddad",
                AnchorUri = new Uri("https://magic.gg/decklists/2021-magic-online-champions-showcase-season-1-modern-decklists#Izzet_Blitz_Ryan_Haddad_2021_MOCS_Season_1_6_26_2021_53092e3d-2799-4ab3-a690-fa020cf528eb"),
                Date = new DateTime(2021, 06, 26, 00, 00, 00, DateTimeKind.Utc),
                Result = null,
                Mainboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Mishra's Bauble",         Count=4 },
                    new DeckItem() { CardName= "Steam Vents",             Count=2 },
                    new DeckItem() { CardName= "Scalding Tarn",           Count=2 },
                    new DeckItem() { CardName= "Bloodstained Mire",       Count=2 },
                    new DeckItem() { CardName= "Dragon's Rage Channeler", Count=4 },
                    new DeckItem() { CardName= "Lightning Bolt",          Count=4 },
                    new DeckItem() { CardName= "Arid Mesa",               Count=1 },
                    new DeckItem() { CardName= "Mountain",                Count=4 },
                    new DeckItem() { CardName= "Expressive Iteration",    Count=4 },
                    new DeckItem() { CardName= "Lava Dart",               Count=4 },
                    new DeckItem() { CardName= "Stormwing Entity",        Count=4 },
                    new DeckItem() { CardName= "Manamorphose",            Count=4 },
                    new DeckItem() { CardName= "Unholy Heat",             Count=2 },
                    new DeckItem() { CardName= "Fiery Islet",             Count=4 },
                    new DeckItem() { CardName= "Spirebluff Canal",        Count=4 },
                    new DeckItem() { CardName= "Monastery Swiftspear",    Count=4 },
                    new DeckItem() { CardName= "Mutagenic Growth",        Count=3 },
                    new DeckItem() { CardName= "Soul-Scar Mage",          Count=4 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Tormod's Crypt", Count=3 },
                    new DeckItem() { CardName= "Kozilek's Return", Count=2 },
                    new DeckItem() { CardName= "Alpine Moon", Count=1 },
                    new DeckItem() { CardName= "Spell Pierce", Count=3 },
                    new DeckItem() { CardName= "Abrade", Count=3 },
                    new DeckItem() { CardName= "Blood Moon", Count=3 }
                },
            });
        }
    }
}
