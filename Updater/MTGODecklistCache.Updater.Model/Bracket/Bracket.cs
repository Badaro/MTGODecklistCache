using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class Bracket
    {
        public BracketItem[] Quarterfinals { get; set; }
        public BracketItem[] Semifinals { get; set; }
        public BracketItem Finals { get; set; }

        public override string ToString()
        {
            return $"Final: {Finals.Player1} {Finals.Result} {Finals.Player2}";
        }
    }
}
