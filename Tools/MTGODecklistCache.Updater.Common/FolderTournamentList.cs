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
        public static T[] GetTournaments<T>(string rawDataFolder) where T : Tournament
        {
            if (!Directory.Exists(rawDataFolder)) return new T[0];

            List<T> tournaments = new List<T>();
            foreach (string tournamentFile in Directory.GetFiles(rawDataFolder, "*.json", SearchOption.AllDirectories))
            {
                T tournament = JsonConvert.DeserializeObject<T>(File.ReadAllText(tournamentFile));
                tournament.Date = tournament.Date.ToUniversalTime();
                tournament.JsonFile = $"{Path.GetFileNameWithoutExtension(tournamentFile).Replace("_", "-").ToLowerInvariant()}-{ tournament.Date.ToString("yyyy-MM-dd")}.json";
                if(tournament.OriginalJsonFile!=null) tournament.OriginalJsonFile = $"{Path.GetFileNameWithoutExtension(tournament.OriginalJsonFile).Replace("_", "-").ToLowerInvariant()}-{tournament.Date.ToString("yyyy-MM-dd")}.json";
                tournaments.Add(tournament);
            }
            return tournaments.ToArray();
        }
    }
}
