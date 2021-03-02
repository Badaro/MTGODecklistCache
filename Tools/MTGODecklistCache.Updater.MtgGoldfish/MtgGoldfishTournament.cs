using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MtgGoldfish
{
    public class MtgGoldfishTournament : Tournament
    {
        [JsonIgnore]
        public string File { get; set; }
    }
}
