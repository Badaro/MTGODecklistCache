using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.MtgMelee;
using MTGODecklistCache.Updater.PlayerLink;
using MTGODecklistCache.Updater.Wizards;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

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
            UpdateFolder<Tournament>(cacheFolder, "manatraders.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<Tournament>(Path.Combine(rawDataFolder, "ManaTraders")),
                t => MTGODecklistCache.Updater.ManaTraders.TournamentLoader.GetTournamentDetails(t));

            // Updates Starcity cache folder
            UpdateFolder<Tournament>(cacheFolder, "starcitygames.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<Tournament>(Path.Combine(rawDataFolder, "StarCityGames")),
                t => MTGODecklistCache.Updater.StarCityGames.TournamentLoader.GetTournamentDetails(t));

            // Updates Starcity cache folder
            UpdateFolder<MtgMeleeTournament>(cacheFolder, "starcitygames.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<MtgMeleeTournament>(Path.Combine(rawDataFolder, "StarCityGames_MtgMelee")),
                t => MTGODecklistCache.Updater.MtgMelee.TournamentLoader.GetTournamentDetails(t));

            // Updates NRG cache folder
            UpdateFolder<MtgMeleeTournament>(cacheFolder, "nerdragegaming.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<MtgMeleeTournament>(Path.Combine(rawDataFolder, "NerdRageGaming")),
                t => MTGODecklistCache.Updater.MtgMelee.TournamentLoader.GetTournamentDetails(t));

            // Updates MagicGG cache folder
            UpdateFolder<Tournament>(cacheFolder, "magic.gg",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<Tournament>(Path.Combine(rawDataFolder, "MagicGG")),
                t => MTGODecklistCache.Updater.MagicGG.TournamentLoader.GetTournamentDetails(t));

            // Updates InsightEsports cache folder
            UpdateFolder<MtgMeleeTournament>(cacheFolder, "insightesports.net",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<MtgMeleeTournament>(Path.Combine(rawDataFolder, "InsightEsports")),
                t => MTGODecklistCache.Updater.MtgMelee.TournamentLoader.GetTournamentDetails(t));

            // Updates HBMO cache folder
            UpdateFolder<PlayerLinkTournament>(cacheFolder, "hunterburtonmemorialopen.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<PlayerLinkTournament>(Path.Combine(rawDataFolder, "HunterBurtonMemorialOpen")),
                t => MTGODecklistCache.Updater.PlayerLink.TournamentLoader.GetTournamentDetails(t));

            // Updates LaBicheTournaments cache folder
            UpdateFolder<Tournament>(cacheFolder, "labichetournaments",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<Tournament>(Path.Combine(rawDataFolder, "LaBicheTournaments")),
                t => MTGODecklistCache.Updater.MoxField.TournamentLoader.GetTournamentDetails(t));

            // Updates CFBEvents cache folder
            UpdateFolder<Tournament>(cacheFolder, "channelfireball.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<Tournament>(Path.Combine(rawDataFolder, "ChannelFireball")),
                t => MTGODecklistCache.Updater.PlainText.TournamentLoader.GetTournamentDetails(t));

            // Updates SvenskaMagic cache folder
            UpdateFolder<MtgMeleeTournament>(cacheFolder, "svenskamagic.com",
                () => MTGODecklistCache.Updater.Common.FolderTournamentList.GetTournaments<MtgMeleeTournament>(Path.Combine(rawDataFolder, "SvenskaMagic")),
                t => MTGODecklistCache.Updater.MtgMelee.TournamentLoader.GetTournamentDetails(t));

        }

        static void UpdateFolder<T>(string cacheRootFolder, string provider, Func<T[]> tournamentList, Func<T, CacheItem> tournamentLoader) where T:Tournament
        {
            string cacheFolder = Path.Combine(cacheRootFolder, provider);

            Console.WriteLine($"Downloading tournament list for {provider}");
            foreach (var tournament in tournamentList().OrderBy(t => t.Date))
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
