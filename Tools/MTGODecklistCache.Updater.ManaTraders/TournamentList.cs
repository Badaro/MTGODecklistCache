using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGODecklistCache.Updater.ManaTraders
{
    public static class TournamentList
    {
        public static Tournament[] GetTournaments(string rawDataFolder)
        {
            List<Tournament> tournaments = new List<Tournament>();
            foreach (string tournamentFile in Directory.GetFiles(rawDataFolder, "*.json"))
            {
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText(tournamentFile));
                DateTime date = json.Date;
                date = date.ToUniversalTime();
                tournaments.Add(new Tournament()
                {
                    Name = json.Name,
                    Date = date,
                    Uri = json.Url,
                    JsonFile = $"{Path.GetFileNameWithoutExtension(tournamentFile).Replace("_", "-").ToLowerInvariant()}-{date.ToString("yyyy-MM-dd")}.json"
                });
            }
            return tournaments.ToArray();
        }
    }
}
