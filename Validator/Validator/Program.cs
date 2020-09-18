using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Windows.Markup;

namespace Validator
{
    class Program
    {
        static string _mtgJsonSource = "https://mtgjson.com/api/v5/AllSetFiles.zip";
        static string _mtgJsonFolder = "mtgjson_allsetfiles";
        static string _mtgJsonTempFile = "mtgjson_allsetfiles.zip";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Validator CACHE_FOLDER");
                return;
            }

            if (!Directory.Exists(_mtgJsonFolder) || Directory.GetFiles(_mtgJsonFolder, "*.json").Length == 0)
            {
                Console.WriteLine("mtgjson data files not found, downloading");
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

            Console.WriteLine("Validating tournaments");
            foreach (string tournamentFile in Directory.GetFiles(args[0], "*.json", SearchOption.AllDirectories))
            {
                dynamic tournament = JsonConvert.DeserializeObject(File.ReadAllText(tournamentFile));

                // Date validation
                foreach (var deck in tournament.Decks)
                {
                    if (deck.Date != null && tournament.Tournament.Date.ToString() == deck.Date.ToString())
                    {
                        // A couple tournaments actually break the pattern, so they need to be whitelisted
                        if (Path.GetFileNameWithoutExtension(tournamentFile) == "standard-ptq-2016-05-31") continue;
                        if (Path.GetFileNameWithoutExtension(tournamentFile) == "pauper-preliminary-2019-12-16") continue;

                        Console.WriteLine($"Tournament {Path.GetFileNameWithoutExtension(tournamentFile)} is using outdated data model");
                        break;
                    }
                }

                // Deck validation
                if (tournament.Decks.Count == 0)
                {
                    Console.WriteLine($"Tournament {Path.GetFileNameWithoutExtension(tournamentFile)} has no decks");
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
                                Console.WriteLine($"Invalid Card {cardName} in tournament {Path.GetFileNameWithoutExtension(tournamentFile)}");
                            }
                        }
                        foreach (var card in deck.Sideboard)
                        {
                            string cardName = card.CardName;
                            if (!validCards.Contains(cardName))
                            {
                                Console.WriteLine($"Invalid Card {cardName} in tournament {Path.GetFileNameWithoutExtension(tournamentFile)}");
                            }
                        }
                    }

                    if (!hasDecksWithCards)
                    {
                        Console.WriteLine($"Tournament {Path.GetFileNameWithoutExtension(tournamentFile)} has only empty decks");
                    }
                }
            }

            Console.WriteLine("Validation completed");
        }
    }
}
