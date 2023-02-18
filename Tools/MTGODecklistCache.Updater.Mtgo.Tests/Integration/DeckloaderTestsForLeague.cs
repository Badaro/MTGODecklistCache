using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class DeckloaderTestsForLeague : DeckLoaderTests
    {
        public DeckloaderTestsForLeague()
        {
        }

        protected override int GetDeckCount()
        {
            return 64;
        }

        protected override DateTime GetEventDate()
        {
            return new DateTime(2020, 08, 04, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return new DateTime(2020, 08, 04, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-league-2020-08-04");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = new DateTime(2020, 08, 04, 00, 00, 00, DateTimeKind.Utc),
                Player = "SIMONEFIERRO",
                Result = "5-0",
                AnchorUri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-league-2020-08-04#deck_SIMONEFIERRO"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Shriekhorn",           Count=4 },
                    new DeckItem(){ CardName="Bloodghast",           Count=4 },
                    new DeckItem(){ CardName="Ox of Agonas",         Count=2 },
                    new DeckItem(){ CardName="Prized Amalgam",       Count=4 },
                    new DeckItem(){ CardName="Merchant of the Vale", Count=3 },
                    new DeckItem(){ CardName="Narcomoeba",           Count=3 },
                    new DeckItem(){ CardName="Stinkweed Imp",        Count=4 },
                    new DeckItem(){ CardName="Golgari Thug",         Count=1 },
                    new DeckItem(){ CardName="Darkblast",            Count=1 },
                    new DeckItem(){ CardName="Stomping Ground",      Count=2 },
                    new DeckItem(){ CardName="Copperline Gorge",     Count=3 },
                    new DeckItem(){ CardName="Steam Vents",          Count=1 },
                    new DeckItem(){ CardName="Dakmor Salvage",       Count=1 },
                    new DeckItem(){ CardName="Arid Mesa",            Count=2 },
                    new DeckItem(){ CardName="Blast Zone",           Count=1 },
                    new DeckItem(){ CardName="Gemstone Mine",        Count=1 },
                    new DeckItem(){ CardName="Forgotten Cave",       Count=2 },
                    new DeckItem(){ CardName="Blood Crypt",          Count=2 },
                    new DeckItem(){ CardName="Blackcleave Cliffs",   Count=2 },
                    new DeckItem(){ CardName="Bloodstained Mire",    Count=1 },
                    new DeckItem(){ CardName="Wooded Foothills",     Count=1 },
                    new DeckItem(){ CardName="Mountain",             Count=1 },
                    new DeckItem(){ CardName="Conflagrate",          Count=2 },
                    new DeckItem(){ CardName="Life from the Loam",   Count=4 },
                    new DeckItem(){ CardName="Cathartic Reunion",    Count=4 },
                    new DeckItem(){ CardName="Creeping Chill",       Count=4 },
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Shenanigans",         Count=2 },
                    new DeckItem(){ CardName="Leyline of the Void", Count=3 },
                    new DeckItem(){ CardName="Ghost Quarter",       Count=1 },
                    new DeckItem(){ CardName="Lightning Axe",       Count=3 },
                    new DeckItem(){ CardName="Thoughtseize",        Count=3 },
                    new DeckItem(){ CardName="Nature's Claim",      Count=3 },
                },
            };
        }
    }
}
