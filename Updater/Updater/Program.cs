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
                Console.WriteLine("Usage: Updater CACHE_FOLDER [START_DATE]");
                return;
            }

            string cacheFolder = args[0];
            DateTime startDate = DateTime.Now.AddDays(-7).ToUniversalTime().Date;
            if (args.Length > 1)
            {
                startDate = DateTime.Parse(args[1], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }

            Console.WriteLine("Downloading tournament list");
            foreach (var tournament in TournamentLoader.GetTournaments(startDate))
            {
                Console.WriteLine($"Downloading tournament {tournament.Uri}");
                var decks = DeckLoader.GetDecks(tournament.Uri);

                string targetFolder = Path.Combine(cacheFolder, tournament.Date.Year.ToString(), tournament.Date.Month.ToString("D2").ToString(), tournament.Date.Day.ToString("D2").ToString());
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string targetFile = Path.Combine(targetFolder, $"{Path.GetFileName(tournament.Uri.LocalPath).Replace("-","_")}.json");

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
