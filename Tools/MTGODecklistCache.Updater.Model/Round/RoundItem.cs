using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class RoundItem
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Result { get; set; }

        public override string ToString()
        {
            return $"{Player1} {Result} {Player2}";
        }
    }
}
