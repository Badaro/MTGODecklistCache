using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    class DeckloaderTestsForLeague : DeckLoaderTests
    {
        public DeckloaderTestsForLeague()
        {
        }

        protected override int GetDeckCount()
        {
            return 76;
        }

        protected override DateTime GetEventDate()
        {
            return new DateTime(2020, 08, 04, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return null;
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-08-04");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = null,
                Player = "CordoTwin",
                Result = "5-0",
                AnchorUri = new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-08-04#cordotwin_-"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Gravecrawler",                 Count=3 },
                    new DeckItem(){ CardName="Hedron Crab",                  Count=4 },
                    new DeckItem(){ CardName="Merfolk Secretkeeper",         Count=4 },
                    new DeckItem(){ CardName="Narcomoeba",                   Count=4 },
                    new DeckItem(){ CardName="Prized Amalgam",               Count=3 },
                    new DeckItem(){ CardName="Silversmote Ghoul",            Count=3 },
                    new DeckItem(){ CardName="Stitcher's Supplier",          Count=4 },
                    new DeckItem(){ CardName="Uro, Titan of Nature's Wrath", Count=3 },
                    new DeckItem(){ CardName="Vengevine",                    Count=4 },
                    new DeckItem(){ CardName="Creeping Chill",               Count=4 },
                    new DeckItem(){ CardName="Glimpse the Unthinkable",      Count=4 },
                    new DeckItem(){ CardName="Breeding Pool",                Count=2},
                    new DeckItem(){ CardName="Island",                       Count=1 },
                    new DeckItem(){ CardName="Misty Rainforest",             Count=4 },
                    new DeckItem(){ CardName="Overgrown Tomb",               Count=2 },
                    new DeckItem(){ CardName="Polluted Delta",               Count=4 },
                    new DeckItem(){ CardName="Swamp",                        Count=1 },
                    new DeckItem(){ CardName="Verdant Catacombs",            Count=4 },
                    new DeckItem(){ CardName="Watery Grave",                 Count=2 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Assassin's Trophy", Count=2 },
                    new DeckItem(){ CardName="Force of Vigor",    Count=2 },
                    new DeckItem(){ CardName="Spell Pierce",      Count=2 },
                    new DeckItem(){ CardName="Fatal Push",        Count=4 },
                    new DeckItem(){ CardName="Nature's Claim",    Count=2 },
                    new DeckItem(){ CardName="Thoughtseize",      Count=3 }
                },
            };
        }
    }
}
