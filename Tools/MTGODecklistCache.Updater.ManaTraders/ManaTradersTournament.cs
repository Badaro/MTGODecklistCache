using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.ManaTraders
{
    public class ManaTradersTournament : Tournament
    {
        [JsonIgnore]
        public string Csv { get; set; }
        [JsonIgnore]
        public string Swiss { get; set; }
        [JsonIgnore]
        public string File { get; set; }
    }
}
