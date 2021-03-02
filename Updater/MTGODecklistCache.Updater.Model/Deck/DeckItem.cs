using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Model
{
    public class DeckItem
    {
        public int Count { get; set; }
        public string CardName { get; set; }

        public override string ToString()
        {
            return $"{Count} {CardName}";
        }
    }
}
