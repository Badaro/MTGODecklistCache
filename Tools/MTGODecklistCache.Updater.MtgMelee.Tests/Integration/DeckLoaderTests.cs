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
    public class DeckLoaderTests
    {
        private Deck[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentLoader.GetTournamentDetails(new MtgMeleeTournament()
            {
                Uri = new Uri("https://melee.gg/Tournament/View/12867"),
                Date = new DateTime(2022, 11, 19, 00, 00, 00, DateTimeKind.Utc)
            }).Decks;
        }

        [Test]
        public void DeckCountIsCorrect()
        {
            _testData.Length.Should().Be(6);
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
            Deck testDeck = _testData.First();
            testDeck.Should().BeEquivalentTo(new Deck()
            {
                Player = "リヒト ＿蝦夷決闘者",
                AnchorUri = new Uri("https://melee.gg/Decklist/View/257085"),
                Date = null,
                Result = "1st Place",
                Mainboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Teferi, Temporal Pilgrim", Count=2 },
                    new DeckItem() { CardName= "Consider",                 Count=4 },
                    new DeckItem() { CardName= "Narset, Parter of Veils",  Count=3 },
                    new DeckItem() { CardName= "Sorin the Mirthless",      Count=1 },
                    new DeckItem() { CardName= "Fatal Push",               Count=4 },
                    new DeckItem() { CardName= "Search for Azcanta",       Count=1 },
                    new DeckItem() { CardName= "Extinction Event",         Count=2 },
                    new DeckItem() { CardName= "Commit // Memory",         Count=1 },
                    new DeckItem() { CardName= "Sinister Sabotage",        Count=3 },
                    new DeckItem() { CardName= "Censor",                   Count=4 },
                    new DeckItem() { CardName= "Ashiok, Nightmare Muse",   Count=1 },
                    new DeckItem() { CardName= "Cling to Dust",            Count=1 },
                    new DeckItem() { CardName= "Memory Deluge",            Count=1 },
                    new DeckItem() { CardName= "Behold the Multiverse",    Count=1 },
                    new DeckItem() { CardName= "Shark Typhoon",            Count=2 },
                    new DeckItem() { CardName= "Watery Grave",             Count=4 },
                    new DeckItem() { CardName= "Clearwater Pathway",       Count=4 },
                    new DeckItem() { CardName= "Negate",                   Count=1 },
                    new DeckItem() { CardName= "Heartless Act",            Count=2 },
                    new DeckItem() { CardName= "Drowned Catacomb",         Count=4 },
                    new DeckItem() { CardName= "Fabled Passage",           Count=2 },
                    new DeckItem() { CardName= "Castle Locthwain",         Count=1 },
                    new DeckItem() { CardName= "Castle Vantress",          Count=1 },
                    new DeckItem() { CardName= "Hive of the Eye Tyrant",   Count=1 },
                    new DeckItem() { CardName= "Hall of Storm Giants",     Count=1 },
                    new DeckItem() { CardName= "Field of Ruin",            Count=2 },
                    new DeckItem() { CardName= "Island",                   Count=2 },
                    new DeckItem() { CardName= "Fetid Pools",              Count=1 },
                    new DeckItem() { CardName= "Swamp",                    Count=2 },
                    new DeckItem() { CardName= "Shipwreck Marsh",          Count=1 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem() { CardName= "Aether Gust",               Count=2 },
                    new DeckItem() { CardName= "Cling to Dust",             Count=1 },
                    new DeckItem() { CardName= "Thoughtseize",              Count=2 },
                    new DeckItem() { CardName= "Bloodchief's Thirst",       Count=1 },
                    new DeckItem() { CardName= "Noxious Grasp",             Count=1 },
                    new DeckItem() { CardName= "Sheoldred, the Apocalypse", Count=2 },
                    new DeckItem() { CardName= "Kalitas, Traitor of Ghet",  Count=1 },
                    new DeckItem() { CardName= "Mystical Dispute",          Count=2 },
                    new DeckItem() { CardName= "Witch's Vengeance",         Count=1 },
                    new DeckItem() { CardName= "Thought Distortion",        Count=1 },
                    new DeckItem() { CardName= "Hullbreaker Horror",        Count=1 },
                },
            });
        }
    }
}
