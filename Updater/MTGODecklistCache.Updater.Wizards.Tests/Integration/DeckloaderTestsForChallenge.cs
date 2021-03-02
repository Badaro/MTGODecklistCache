using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
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
            return new DateTime(2020, 06, 07, 00, 00, 00, DateTimeKind.Utc);
        }

        protected override DateTime? GetDeckDate()
        {
            return new DateTime(2020, 06, 07, 00, 00, 00, DateTimeKind.Utc);
        }


        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-08");
        }

        protected override Deck GetFirstDeck()
        {
            return new Deck()
            {
                Date = new DateTime(2020, 06, 07, 00, 00, 00, DateTimeKind.Utc),
                Player = "TSPJendrek",
                Result = "1st Place",
                AnchorUri = new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-08#tspjendrek_st_place"),
                Mainboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Jace, the Mind Sculptor",      Count=3 },
                    new DeckItem(){ CardName="Ice-Fang Coatl",               Count=3 },
                    new DeckItem(){ CardName="Snapcaster Mage",              Count=1 },
                    new DeckItem(){ CardName="Stoneforge Mystic",            Count=4 },
                    new DeckItem(){ CardName="Uro, Titan of Nature's Wrath", Count=3 },
                    new DeckItem(){ CardName="Abrupt Decay",                 Count=1 },
                    new DeckItem(){ CardName="Archmage's Charm",             Count=2 },
                    new DeckItem(){ CardName="Cryptic Command",              Count=2 },
                    new DeckItem(){ CardName="Deprive",                      Count=1 },
                    new DeckItem(){ CardName="Fatal Push",                   Count=4 },
                    new DeckItem(){ CardName="Force of Negation",            Count=3 },
                    new DeckItem(){ CardName="Kaya's Guile",                 Count=1 },
                    new DeckItem(){ CardName="Mana Leak",                    Count=1 },
                    new DeckItem(){ CardName="Spell Snare",                  Count=1 },
                    new DeckItem(){ CardName="Arcum's Astrolabe",            Count=4 },
                    new DeckItem(){ CardName="Batterskull",                  Count=1 },
                    new DeckItem(){ CardName="Sword of Feast and Famine",    Count=1 },
                    new DeckItem(){ CardName="Breeding Pool",                Count=2 },
                    new DeckItem(){ CardName="Flooded Strand",               Count=4 },
                    new DeckItem(){ CardName="Godless Shrine",               Count=1 },
                    new DeckItem(){ CardName="Hallowed Fountain",            Count=1 },
                    new DeckItem(){ CardName="Misty Rainforest",             Count=4 },
                    new DeckItem(){ CardName="Mystic Sanctuary",             Count=2 },
                    new DeckItem(){ CardName="Polluted Delta",               Count=2 },
                    new DeckItem(){ CardName="Snow-Covered Forest",          Count=1 },
                    new DeckItem(){ CardName="Snow-Covered Island",          Count=4 },
                    new DeckItem(){ CardName="Snow-Covered Plains",          Count=1 },
                    new DeckItem(){ CardName="Snow-Covered Swamp",           Count=1 },
                    new DeckItem(){ CardName="Watery Grave",                 Count=1 }
                },
                Sideboard = new DeckItem[]
                {
                    new DeckItem(){ CardName="Abrupt Decay",         Count=2 },
                    new DeckItem(){ CardName="Aether Gust",          Count=2 },
                    new DeckItem(){ CardName="Bitterblossom",        Count=2 },
                    new DeckItem(){ CardName="Mystical Dispute",     Count=1 },
                    new DeckItem(){ CardName="Supreme Verdict",      Count=1 },
                    new DeckItem(){ CardName="Kaya's Guile",         Count=2 },
                    new DeckItem(){ CardName="Ashiok, Dream Render", Count=1 },
                    new DeckItem(){ CardName="Dovin's Veto",         Count=1 },
                    new DeckItem(){ CardName="Nihil Spellbomb",      Count=1 },
                    new DeckItem(){ CardName="Veil of Summer",       Count=2 }
                },
            };
        }
    }
}
