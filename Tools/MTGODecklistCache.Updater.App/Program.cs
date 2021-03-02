using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Wizards;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;

namespace MTGODecklistCache.Updater.App
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: MTGODecklistCache.Updater.App CACHE_FOLDER RAWDATA_FOLDER [START_DATE] [END_DATE]");
                return;
            }

            string cacheFolder = new DirectoryInfo(args[0]).FullName;
            string rawDataFolder = new DirectoryInfo(args[1]).FullName;

            DateTime startDate = DateTime.Now.AddDays(-7).ToUniversalTime().Date;
            if (args.Length > 2)
            {
                startDate = DateTime.Parse(args[2], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            DateTime? endDate = null;
            if (args.Length > 3)
            {
                endDate = DateTime.Parse(args[3], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }

            // Updates Wizards cache folder
            UpdateFolder(Path.Combine(cacheFolder, "magic.wizards.com"),
                () => MTGODecklistCache.Updater.Wizards.TournamentList.GetTournaments(startDate, endDate, null, 7),
                t => MTGODecklistCache.Updater.Wizards.TournamentLoader.GetTournamentDetails(t),
                t => $"{Path.GetFileName(t.Uri.LocalPath)}.json");

            // Updates ManaTraders cache folder
            UpdateFolder(Path.Combine(cacheFolder, "manatraders.com"),
                () => MTGODecklistCache.Updater.ManaTraders.TournamentList.GetTournaments(Path.Combine(rawDataFolder, "ManaTraders")),
                t => MTGODecklistCache.Updater.ManaTraders.TournamentLoader.GetTournamentDetails(t),
                t => $"{Path.GetFileNameWithoutExtension((t as MTGODecklistCache.Updater.ManaTraders.ManaTradersTournament).File).Replace("_", "-").ToLowerInvariant()}-{t.Date.ToString("yyyy-MM-dd")}.json");
        }

        static void UpdateFolder(string cacheFolder, Func<Tournament[]> tournamentList, Func<Tournament, CacheItem> tournamentLoader, Func<Tournament, string> fileNameLoader)
        {
            Console.WriteLine("Downloading tournament list");
            foreach (var tournament in tournamentList())
            {
                Console.WriteLine($"Downloading tournament {tournament.Uri}");
                string targetFolder = Path.Combine(cacheFolder, tournament.Date.Year.ToString(), tournament.Date.Month.ToString("D2").ToString(), tournament.Date.Day.ToString("D2").ToString());
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string targetFile = Path.Combine(targetFolder, fileNameLoader(tournament));
                if (File.Exists(targetFile))
                {
                    Console.WriteLine($"Already downloaded, skipping");
                    continue;
                }

                var details = tournamentLoader(tournament);
                string contents = JsonConvert.SerializeObject(details, Formatting.Indented);

                File.WriteAllText(targetFile, contents);
            }
        }
    }
}
