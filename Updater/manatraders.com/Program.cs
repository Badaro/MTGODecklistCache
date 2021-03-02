using MTGODecklistParser.Model;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Reflection.Metadata;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: Updater CACHE_FOLDER RAWDATA_FOLDER");
                return;
            }

            string cacheFolder = new DirectoryInfo(args[0]).FullName;
            string rawFolder = new DirectoryInfo(args[1]).FullName;

            Console.WriteLine("Downloading tournament list");
            foreach (ManaTradersTournament tournament in TournamentLoader.GetTournaments(rawFolder))
            {
                Console.WriteLine($"Downloading tournament {tournament.Uri}");
                string targetFolder = Path.Combine(cacheFolder, tournament.Date.Year.ToString(), tournament.Date.Month.ToString("D2").ToString(), tournament.Date.Day.ToString("D2").ToString());
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string targetFile = Path.Combine(targetFolder, $"{Path.GetFileNameWithoutExtension(tournament.File).Replace("_", "-").ToLowerInvariant()}-{tournament.Date.ToString("yyyy-MM-dd")}.json");
                if (File.Exists(targetFile))
                {
                    Console.WriteLine($"Already downloaded, skipping");
                    continue;
                }

                var details = TournamentDetailsLoader.GetTournamentDetails(tournament.Csv, tournament.Swiss, $"{tournament.Uri.ToString()}swiss", $"{tournament.Uri.ToString()}finals");
                string contents = JsonConvert.SerializeObject(new CacheItem()
                {
                    Tournament = new Tournament() { Name = tournament.Name, Date = tournament.Date, Uri = tournament.Uri },
                    Decks = details.Decks,
                    Bracket = details.Bracket,
                    Standings = details.Standings,
                    Rounds = details.Rounds
                }, Formatting.Indented);

                File.WriteAllText(targetFile, contents);
            }
        }
    }

    class CacheItem
    {
        public Tournament Tournament { get; set; }
        public Deck[] Decks { get; set; }
        public Round[] Rounds { get; set; }
        public Bracket Bracket { get; set; }
        public Standing[] Standings { get; set; }
    }
}
