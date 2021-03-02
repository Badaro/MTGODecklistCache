using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class Round
    {
        public int RoundNumber { get; set; }
        public RoundItem[] Matches { get; set; }

        public override string ToString()
        {
            return $"Round: {RoundNumber}, {Matches.Length} matches";
        }
    }
}
