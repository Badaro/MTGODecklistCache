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
            UpdateFolder(cacheFolder, "magic.wizards.com",
                () => MTGODecklistCache.Updater.Wizards.TournamentList.GetTournaments(startDate, endDate, null, 7),
                t => MTGODecklistCache.Updater.Wizards.TournamentLoader.GetTournamentDetails(t));

            // Updates ManaTraders cache folder
            UpdateFolder(cacheFolder, "manatraders.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments(Path.Combine(rawDataFolder, "ManaTraders")),
                t => MTGODecklistCache.Updater.ManaTraders.TournamentLoader.GetTournamentDetails(t));

            // Updates NRG cache folder
            UpdateFolder(cacheFolder, "nerdragegaming.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments(Path.Combine(rawDataFolder, "NerdRageGaming")),
                t => MTGODecklistCache.Updater.MtgGoldfish.TournamentLoader.GetTournamentDetails(t));
        }

        static void UpdateFolder(string cacheRootFolder, string provider, Func<Tournament[]> tournamentList, Func<Tournament, CacheItem> tournamentLoader)
        {
            string cacheFolder = Path.Combine(cacheRootFolder, provider);

            Console.WriteLine($"Downloading tournament list for {provider}");
            foreach (var tournament in tournamentList())
            {
                Console.WriteLine($"- Downloading tournament {tournament.JsonFile}");
                string targetFolder = Path.Combine(cacheFolder, tournament.Date.Year.ToString(), tournament.Date.Month.ToString("D2").ToString(), tournament.Date.Day.ToString("D2").ToString());
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string targetFile = Path.Combine(targetFolder, tournament.JsonFile);
                if (File.Exists(targetFile))
                {
                    Console.WriteLine($"-- Already downloaded, skipping");
                    continue;
                }

                var details = tournamentLoader(tournament);
                string contents = JsonConvert.SerializeObject(details, Formatting.Indented);

                File.WriteAllText(targetFile, contents);
            }
        }
    }
}
