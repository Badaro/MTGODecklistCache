using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class DeckloaderTestsForPrelim : DeckLoaderTests
    {
        public DeckloaderTestsForPrelim()
        {
        }

        protected override int GetDeckCount()
        {
            return 16;
        }

        protected override DateTime GetEventDate()
        {
            return new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc),
                Player = "Wartico1",
                Result = "5-0",
                AnchorUri = new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02#wartico_-"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Abbot of Keral Keep",  Count=4 },
                    new DeckItem(){ CardName="Kiln Fiend",           Count=2 },
                    new DeckItem(){ CardName="Monastery Swiftspear", Count=4 },
                    new DeckItem(){ CardName="Soul-Scar Mage",       Count=4 },
                    new DeckItem(){ CardName="Thoughtseize",         Count=4 },
                    new DeckItem(){ CardName="Cling to Dust",        Count=3 },
                    new DeckItem(){ CardName="Fatal Push",           Count=3 },
                    new DeckItem(){ CardName="Kolaghan's Command",   Count=1 },
                    new DeckItem(){ CardName="Lava Dart",            Count=2 },
                    new DeckItem(){ CardName="Lightning Bolt",       Count=4 },
                    new DeckItem(){ CardName="Manamorphose",         Count=4 },
                    new DeckItem(){ CardName="Mishra's Bauble",      Count=4 },
                    new DeckItem(){ CardName="Seal of Fire",         Count=2 },
                    new DeckItem(){ CardName="Arid Mesa",            Count=1 },
                    new DeckItem(){ CardName="Blackcleave Cliffs",   Count=4 },
                    new DeckItem(){ CardName="Blood Crypt",          Count=2 },
                    new DeckItem(){ CardName="Bloodstained Mire",    Count=4 },
                    new DeckItem(){ CardName="Marsh Flats",          Count=1 },
                    new DeckItem(){ CardName="Mountain",             Count=3 },
                    new DeckItem(){ CardName="Sacred Foundry",       Count=1 },
                    new DeckItem(){ CardName="Sunbaked Canyon",      Count=2 },
                    new DeckItem(){ CardName="Swamp",                Count=1 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Fatal Push",              Count=1 },
                    new DeckItem(){ CardName="Alpine Moon",             Count=2 },
                    new DeckItem(){ CardName="Engineered Explosives",   Count=2 },
                    new DeckItem(){ CardName="Lurrus of the Dream-Den", Count=1 },
                    new DeckItem(){ CardName="Unearth",                 Count=1 },
                    new DeckItem(){ CardName="Kolaghan's Command",      Count=1 },
                    new DeckItem(){ CardName="Collective Brutality",    Count=2 },
                    new DeckItem(){ CardName="Goblin Cratermaker",      Count=1 },
                    new DeckItem(){ CardName="Nihil Spellbomb",         Count=2 },
                    new DeckItem(){ CardName="Wear // Tear",            Count=2 }
                },
            };
        }
    }
}
