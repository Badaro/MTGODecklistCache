using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class DeckloaderTestsForChallenge : DeckLoaderTests
    {
        public DeckloaderTestsForChallenge()
        {
        }

        protected override int GetDeckCount()
        {
            return 32;
        }

        protected override DateTime GetEventDate()
        {
            return new DateTime(2022, 10, 23, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return new DateTime(2022, 10, 23, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2022-10-2312488075");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = new DateTime(2022, 10, 23, 00, 00, 00, DateTimeKind.Utc),
                Player = "Baku_91",
                Result = "1st Place",
                AnchorUri = new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2022-10-2312488075#deck_Baku_91"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Mishra's Bauble",         Count=3 },
                    new DeckItem(){ CardName="Dragon's Rage Channeler", Count=4 },
                    new DeckItem(){ CardName="Murktide Regent",         Count=4 },
                    new DeckItem(){ CardName="Delver of Secrets",       Count=1 },
                    new DeckItem(){ CardName="Brazen Borrower",         Count=1 },
                    new DeckItem(){ CardName="Counterbalance",          Count=2 },
                    new DeckItem(){ CardName="Force of Negation",       Count=1 },
                    new DeckItem(){ CardName="Daze",                    Count=3 },
                    new DeckItem(){ CardName="Lightning Bolt",          Count=4 },
                    new DeckItem(){ CardName="Force of Will",           Count=4 },
                    new DeckItem(){ CardName="Brainstorm",              Count=4 },
                    new DeckItem(){ CardName="Pyroblast",               Count=2 },
                    new DeckItem(){ CardName="Polluted Delta",          Count=2 },
                    new DeckItem(){ CardName="Island",                  Count=1 },
                    new DeckItem(){ CardName="Wasteland",               Count=4 },
                    new DeckItem(){ CardName="Misty Rainforest",        Count=2 },
                    new DeckItem(){ CardName="Steam Vents",             Count=1 },
                    new DeckItem(){ CardName="Flooded Strand",          Count=2 },
                    new DeckItem(){ CardName="Volcanic Island",         Count=4 },
                    new DeckItem(){ CardName="Mystic Sanctuary",        Count=1 },
                    new DeckItem(){ CardName="Scalding Tarn",           Count=2 },
                    new DeckItem(){ CardName="Expressive Iteration",    Count=4 },
                    new DeckItem(){ CardName="Ponder",                  Count=4 },
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Tropical Island",              Count=1 },
                    new DeckItem(){ CardName="End the Festivities",          Count=1 },
                    new DeckItem(){ CardName="Meltdown",                     Count=2 },
                    new DeckItem(){ CardName="Red Elemental Blast",          Count=1 },
                    new DeckItem(){ CardName="Pyroblast",                    Count=2 },
                    new DeckItem(){ CardName="Minsc & Boo, Timeless Heroes", Count=2 },
                    new DeckItem(){ CardName="Hydroblast",                   Count=1 },
                    new DeckItem(){ CardName="Submerge",                     Count=2 },
                    new DeckItem(){ CardName="Surgical Extraction",          Count=2 },
                    new DeckItem(){ CardName="Force of Negation",            Count=1 },
                },
            };
        }
    }
}
