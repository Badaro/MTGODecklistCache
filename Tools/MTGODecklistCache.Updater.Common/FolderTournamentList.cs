using MTGODecklistCache.Updater.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MTGODecklistCache.Updater.Common
{
    public static class FolderTournamentList
    {
        public static Tournament[] GetTournaments(string rawDataFolder)
        {
            List<Tournament> tournaments = new List<Tournament>();
            foreach (string tournamentFile in Directory.GetFiles(rawDataFolder, "*.json"))
            {
                Tournament tournament = JsonConvert.DeserializeObject<Tournament>(File.ReadAllText(tournamentFile));
                tournament.Date = tournament.Date.ToUniversalTime();
                tournament.JsonFile = $"{Path.GetFileNameWithoutExtension(tournamentFile).Replace("_", "-").ToLowerInvariant()}-{ tournament.Date.ToString("yyyy-MM-dd")}.json";
                tournaments.Add(tournament);
            }
            return tournaments.ToArray();
        }
    }
}
