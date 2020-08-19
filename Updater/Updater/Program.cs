using MTGODecklistParser.Data;
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
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Updater CACHE_FOLDER [START_DATE] [END_DATE]");
                return;
            }

            string cacheFolder = new DirectoryInfo(args[0]).FullName;
            DateTime startDate = DateTime.Now.AddDays(-7).ToUniversalTime().Date;
            if (args.Length > 1)
            {
                startDate = DateTime.Parse(args[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            DateTime? endDate = null;
            if (args.Length > 2)
            {
                endDate = DateTime.Parse(args[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }

            Console.WriteLine("Downloading tournament list");
            foreach (var tournament in TournamentLoader.GetTournaments(startDate, endDate))
            {
                Console.WriteLine($"Downloading tournament {tournament.Uri}");
                string targetFolder = Path.Combine(cacheFolder, tournament.Date.Year.ToString(), tournament.Date.Month.ToString("D2").ToString(), tournament.Date.Day.ToString("D2").ToString());
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string targetFile = Path.Combine(targetFolder, $"{Path.GetFileName(tournament.Uri.LocalPath).Replace("-","_")}.json");
                if (File.Exists(targetFile))
                {
                    Console.WriteLine($"Already downloaded, skipping");
                    continue;
                }

                var decks = DeckLoader.GetDecks(tournament.Uri);
                string contents = JsonConvert.SerializeObject(new CacheItem()
                {
                    Tournament = tournament,
                    Decks = decks
                }, Formatting.Indented);

                File.WriteAllText(targetFile, contents);
            }
        }
    }

    class CacheItem
    {
        public Tournament Tournament { get; set; }
        public Deck[] Decks { get; set; }
    }
}
