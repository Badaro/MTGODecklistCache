using System.Data.SQLite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace MTGODecklistCache.Validator.App
{
    class Program
    {
        static int _mtgJsonMaxAgeInDays = 1;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MTGODecklistCache.Validator.App CACHE_FOLDER");
                return;
            }

            HashSet<string> validCards = GetCardNamesFromSqlite();

            Console.WriteLine("Starting tournament validation");
            int count = 0;
            string[] tournamentFiles = Directory.GetFiles(args[0], "*.json", SearchOption.AllDirectories);
            Console.Write($"Loading tournaments: {count}/{tournamentFiles.Length}");

            List<string> validationErrors = new List<string>();

            foreach (string tournamentFile in tournamentFiles)
            {
                if (count % 100 == 0) Console.Write($"\rLoading tournaments: {count}/{tournamentFiles.Length}"); ;
                count++;

                dynamic tournament = JsonConvert.DeserializeObject(File.ReadAllText(tournamentFile));

                // Deck validation
                if (tournament.Decks.Count == 0)
                {
                    validationErrors.Add($"Tournament {Path.GetFileNameWithoutExtension(tournamentFile)} has no decks");
                }
                else
                {
                    // Card validation
                    bool hasDecksWithCards = false;
                    foreach (var deck in tournament.Decks)
                    {
                        hasDecksWithCards |= deck.Mainboard.Count > 0;
                        foreach (var card in deck.Mainboard)
                        {
                            string cardName = card.CardName;
                            if (!validCards.Contains(cardName))
                            {
                                validationErrors.Add($"Invalid Card {cardName} in tournament {Path.GetFileNameWithoutExtension(tournamentFile)}");
                            }
                        }
                        foreach (var card in deck.Sideboard)
                        {
                            string cardName = card.CardName;
                            if (!validCards.Contains(cardName))
                            {
                                validationErrors.Add($"Invalid Card {cardName} in tournament {Path.GetFileNameWithoutExtension(tournamentFile)}");
                            }
                        }
                    }

                    if (!hasDecksWithCards)
                    {
                        validationErrors.Add($"Tournament {Path.GetFileNameWithoutExtension(tournamentFile)} has only empty decks");
                    }
                }
            }

            Console.Write(Environment.NewLine);
            Console.WriteLine("Finished tournament validation");
            Console.WriteLine($"Found {validationErrors.Count} errors in tournament files");
            foreach (string validationError in validationErrors) Console.WriteLine(validationError);
        }

        #region GetCardNames - SQLite Version

        static string _mtgJsonSqliteSource = "https://mtgjson.com/api/v5/AllPrintings.sqlite.zip";
        static string _mtgJsonSqliteFolder = "mtgjson_allprintings_sqlite";
        static string _mtgJsonSqliteTempFile = "mtgjson_allprintings_sqlite.zip";

        private static HashSet<string> GetCardNamesFromSqlite()
        {
            bool mtgJsonFilesExist = Directory.Exists(_mtgJsonSqliteFolder) && Directory.GetFiles(_mtgJsonSqliteFolder, "*.sqlite").Length > 0;
            bool mtgJsonFilesRecent = mtgJsonFilesExist && new DirectoryInfo(_mtgJsonSqliteFolder).LastWriteTime > DateTime.Now.AddDays(-1 * _mtgJsonMaxAgeInDays);

            if (!mtgJsonFilesExist || !mtgJsonFilesRecent)
            {
                Console.WriteLine("mtgjson data files not found or outdated, downloading");
                new WebClient().DownloadFile(_mtgJsonSqliteSource, _mtgJsonSqliteTempFile);

                if (Directory.Exists(_mtgJsonSqliteFolder)) Directory.Delete(_mtgJsonSqliteFolder, true);
                Directory.CreateDirectory(_mtgJsonSqliteFolder);

                ZipFile.ExtractToDirectory(_mtgJsonSqliteTempFile, _mtgJsonSqliteFolder);
                File.Delete(_mtgJsonSqliteTempFile);
            }

            HashSet<string> validCards = new HashSet<string>();
            Console.WriteLine("Loading card names from mtgjson database");

            string dbFile = new FileInfo(Directory.GetFiles(_mtgJsonSqliteFolder, "*.sqlite").First()).FullName;
            using (var connection = new SQLiteConnection($"Data Source={dbFile}"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT name, faceName, layout from cards inner join cardLegalities on cards.uuid = cardLegalities.uuid where borderColor<>'silver' and borderColor<>'gold'";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var faceName = reader.GetValue(1) == System.DBNull.Value ? String.Empty : reader.GetString(1);
                        var layout = reader.GetString(2);

                        string cardName;
                        if (layout == "transform" || layout == "flip" || layout == "adventure" || layout == "meld" || layout == "modal_dfc") cardName = faceName;
                        else cardName = name;
                        if (!validCards.Contains(cardName)) validCards.Add(cardName);
                    }
                }
            }

            return validCards;
        }

        #endregion
    }
}