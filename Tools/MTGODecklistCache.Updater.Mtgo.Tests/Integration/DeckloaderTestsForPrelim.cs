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
            return 3;
        }

        protected override DateTime GetEventDate()
        {
            return new DateTime(2022, 10, 25, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return new DateTime(2022, 10, 25, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2022-10-2512488091");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = new DateTime(2022, 10, 25, 00, 00, 00, DateTimeKind.Utc),
                Player = "Aeolic",
                Result = "4-0",
                AnchorUri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2022-10-2512488091#deck_Aeolic"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Relic of Progenitus",            Count=4 },
                    new DeckItem(){ CardName="Bonecrusher Giant",              Count=3 },
                    new DeckItem(){ CardName="Squee, Dubious Monarch",         Count=1 },
                    new DeckItem(){ CardName="Fury",                           Count=4 },
                    new DeckItem(){ CardName="Ragavan, Nimble Pilferer",       Count=4 },
                    new DeckItem(){ CardName="Seasoned Pyromancer",            Count=4 },
                    new DeckItem(){ CardName="Blood Moon",                     Count=3 },
                    new DeckItem(){ CardName="Fable of the Mirror-Breaker",    Count=4 },
                    new DeckItem(){ CardName="Spikefield Hazard",              Count=2 },
                    new DeckItem(){ CardName="Lightning Bolt",                 Count=4 },
                    new DeckItem(){ CardName="Inspiring Vantage",              Count=3 },
                    new DeckItem(){ CardName="Sokenzan, Crucible of Defiance", Count=1 },
                    new DeckItem(){ CardName="Prismatic Vista",                Count=1 },
                    new DeckItem(){ CardName="Den of the Bugbear",             Count=2 },
                    new DeckItem(){ CardName="Sacred Foundry",                 Count=2 },
                    new DeckItem(){ CardName="Plains",                         Count=1 },
                    new DeckItem(){ CardName="Arid Mesa",                      Count=4 },
                    new DeckItem(){ CardName="Blast Zone",                     Count=2 },
                    new DeckItem(){ CardName="Jetmir's Garden",                Count=1 },
                    new DeckItem(){ CardName="Shinka, the Bloodsoaked Keep",   Count=1 },
                    new DeckItem(){ CardName="Needle Spires",                  Count=1 },
                    new DeckItem(){ CardName="Mountain",                       Count=2 },
                    new DeckItem(){ CardName="Flame Slash",                    Count=2 },
                    new DeckItem(){ CardName="Prismatic Ending",               Count=4 },
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Obosh, the Preypiercer", Count=1 },
                    new DeckItem(){ CardName="Hallowed Moonlight",     Count=4 },
                    new DeckItem(){ CardName="Burrenton Forge-Tender", Count=2 },
                    new DeckItem(){ CardName="Chained to the Rocks",   Count=2 },
                    new DeckItem(){ CardName="Magus of the Moon",      Count=3 },
                    new DeckItem(){ CardName="Temporary Lockdown",     Count=1 },
                    new DeckItem(){ CardName="Wear // Tear",           Count=2 },
                },
            };
        }
    }
}
