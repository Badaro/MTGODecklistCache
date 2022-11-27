using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class Round
    {
        public string RoundName { get; set; }
        public RoundItem[] Matches { get; set; }

        public override string ToString()
        {
            return $"Round: {RoundName}, {Matches.Length} matches";
        }
    }
}
