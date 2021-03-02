using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MTGODecklistCache.Updater.Model
{
    public class Standing
    {
        public int Rank { get; set; }
        public string Player { get; set; }
        public int Points { get; set; }
        public double OMWP { get; set; }
        public double GWP { get; set; }
        public double OGWP { get; set; }

        public override string ToString()
        {
            return $"#{Rank} {Player} {Points} points";
        }
    }
}
