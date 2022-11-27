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
    public class DeckLoaderTests
    {
        private Deck[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.manatraders.com/tournaments/30/"),
                Date = new DateTime(2022, 08, 31, 00, 00, 00, DateTimeKind.Utc)
            }).Decks;
        }

        [Test]
        public void DeckCountIsCorrect()
        {
            _testData.Length.Should().Be(194);
        }

        [Test]
        public void DecksDontHaveDate()
        {
            foreach (var deck in _testData) deck.Date.Should().BeNull();
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
            Deck testDeck = _testData.Skip(7).First();
            testDeck.Should().BeEquivalentTo(new Deck()
            {
                Player = "Fink64",
                AnchorUri = new Uri("https://www.manatraders.com/webshop/personal/874208"),
                Date = null,
                Result = "8th Place",
                Mainboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Mausoleum Wanderer",          Count=4 },
                    new DeckItem() { CardName= "Spectral Sailor",             Count=4 },
                    new DeckItem() { CardName= "Rattlechains",                Count=4 },
                    new DeckItem() { CardName= "Selfless Spirit",             Count=1 },
                    new DeckItem() { CardName= "Shacklegeist",                Count=4 },
                    new DeckItem() { CardName= "Supreme Phantom",             Count=4 },
                    new DeckItem() { CardName= "Empyrean Eagle",              Count=3 },
                    new DeckItem() { CardName= "Katilda, Dawnhart Martyr",    Count=1 },
                    new DeckItem() { CardName= "Skyclave Apparition",         Count=4 },
                    new DeckItem() { CardName= "Spell Queller",               Count=4 },
                    new DeckItem() { CardName= "Collected Company",           Count=4 },
                    new DeckItem() { CardName= "Botanical Sanctum",           Count=4 },
                    new DeckItem() { CardName= "Branchloft Pathway",          Count=4 },
                    new DeckItem() { CardName= "Breeding Pool",               Count=1 },
                    new DeckItem() { CardName= "Eiganjo, Seat of the Empire", Count=1 },
                    new DeckItem() { CardName= "Hallowed Fountain",           Count=4 },
                    new DeckItem() { CardName= "Hengegate Pathway",           Count=4 },
                    new DeckItem() { CardName= "Island",                      Count=1 },
                    new DeckItem() { CardName= "Mana Confluence",             Count=1 },
                    new DeckItem() { CardName= "Otawara, Soaring City",       Count=1 },
                    new DeckItem() { CardName= "Secluded Courtyard",          Count=1 },
                    new DeckItem() { CardName= "Temple Garden",               Count=1 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Portable Hole",         Count=2 },
                    new DeckItem() { CardName= "Shapers' Sanctuary",    Count=2 },
                    new DeckItem() { CardName= "Lofty Denial",          Count=4 },
                    new DeckItem() { CardName= "Rest in Peace",         Count=2 },
                    new DeckItem() { CardName= "Selfless Spirit",       Count=1 },
                    new DeckItem() { CardName= "Extraction Specialist", Count=4 }
                },
            });
        }


        [Test]
        public void ShouldApplyTop8OrderingToDecks()
        {
            string[] top8 = new string[]
            {
                "ModiSapiras",
                "kvza",
                "Cinciu",
                "zuri1988",
                "Daking3603",
                "Harry13",
                "ScouterTF2",
                "Fink64"
            };

            _testData.Take(8).Select(s => s.Player).Should().BeEquivalentTo(top8, o => o.WithStrictOrdering());
        }
    }
}
