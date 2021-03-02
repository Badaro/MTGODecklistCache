using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGODecklistCache.Updater.MtgGoldfish
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
                tournaments.Add(new MtgGoldfishTournament()
                {
                    Name = json.Name,
                    Date = date.ToUniversalTime(),
                    Uri = json.Url,
                    File = Path.GetFileName(tournamentFile)
                });
            }
            return tournaments.ToArray();
        }
    }
}
