using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class Deck
    {
        public DateTime? Date { get; set; }
        public string Player { get; set; }
        public string Result { get; set; }
        public Uri AnchorUri { get; set; }
        public DeckItem[] Mainboard { get; set; }
        public DeckItem[] Sideboard { get; set; }

        public bool Contains(params string[] cards)
        {
            return cards.All(c => Mainboard.Any(i => i.CardName == c) || Sideboard.Any(i => i.CardName == c));
        }

        public override string ToString()
        {
            var total = Mainboard.Select(c => c.Count).Sum() + Sideboard.Select(c => c.Count).Sum();
            return $"{total} cards";
        }
    }
}
