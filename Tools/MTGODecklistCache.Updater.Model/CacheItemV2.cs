using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Model
{
    public class CacheItemV2
    {
        public Tournament Tournament { get; set; }
        public Deck[] Decks { get; set; }
        public RoundV2[] Rounds { get; set; }
        public Standing[] Standings { get; set; }
        public override string ToString()
        {
            return $"{Decks.Length} decks";
        }
    }
}
