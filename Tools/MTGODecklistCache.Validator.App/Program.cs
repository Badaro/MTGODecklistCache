using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace MTGODecklistCache.Validator.App
{
    class Program
    {
        static string _mtgJsonSource = "https://mtgjson.com/api/v5/AllSetFiles.zip";
        static string _mtgJsonFolder = "mtgjson_allsetfiles";
        static string _mtgJsonTempFile = "mtgjson_allsetfiles.zip";
        static int _mtgJsonMaxAgeInDays = 1;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MTGODecklistCache.Validator.App CACHE_FOLDER");
                return;
            }

            bool mtgJsonFilesExist = Directory.Exists(_mtgJsonFolder) && Directory.GetFiles(_mtgJsonFolder, "*.json").Length > 0;
            bool mtgJsonFilesRecent = mtgJsonFilesExist && new DirectoryInfo(_mtgJsonFolder).LastWriteTime > DateTime.Now.AddDays(-1 * _mtgJsonMaxAgeInDays);

            if (!mtgJsonFilesExist || !mtgJsonFilesRecent)
            {
                Console.WriteLine("mtgjson data files not found or outdated, downloading");
                new WebClient().DownloadFile(_mtgJsonSource, _mtgJsonTempFile);

                if (Directory.Exists(_mtgJsonFolder)) Directory.Delete(_mtgJsonFolder, true);
                Directory.CreateDirectory(_mtgJsonFolder);

                ZipFile.ExtractToDirectory(_mtgJsonTempFile, _mtgJsonFolder);
                File.Delete(_mtgJsonTempFile);
            }

            HashSet<string> validCards = new HashSet<string>();
            Console.WriteLine("Loading card names from mtgjson data files");
            foreach (string setFile in Directory.GetFiles(_mtgJsonFolder))
            {
                dynamic set = JsonConvert.DeserializeObject(File.ReadAllText(setFile));
                foreach (var card in set.data.cards)
                {
                    string cardName;
                    if (card.layout == "transform" || card.layout == "flip" || card.layout == "adventure" || card.layout == "meld" || card.layout == "modal_dfc") cardName = card.faceName;
                    else cardName = card.name;
                    if (!validCards.Contains(cardName)) validCards.Add(cardName);
                }
            }

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
    }
}