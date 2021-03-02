using MTGODecklistParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Updater
{
    public class ManaTradersTournament : Tournament
    {
        public string Csv { get; set; }
        public string Swiss { get; set; }
        public string File { get; set; }
    }
}
