using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace MTGODecklistCache.Updater.MtgMelee
{
    public static class TournamentLoader
    {
        static string _phasePage = "https://mtgmelee.com/Tournament/GetPhaseStandings/{phaseId}";
        static string _phaseParameters = "columns%5B0%5D%5Bdata%5D=Rank&columns%5B0%5D%5Bname%5D=Rank&columns%5B0%5D%5Bsearchable%5D=false&columns%5B0%5D%5Borderable%5D=true&columns%5B0%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B0%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B1%5D%5Bdata%5D=Name&columns%5B1%5D%5Bname%5D=Name&columns%5B1%5D%5Bsearchable%5D=true&columns%5B1%5D%5Borderable%5D=true&columns%5B1%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B1%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B2%5D%5Bdata%5D=Decklists&columns%5B2%5D%5Bname%5D=Decklists&columns%5B2%5D%5Bsearchable%5D=false&columns%5B2%5D%5Borderable%5D=false&columns%5B2%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B2%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B3%5D%5Bdata%5D=Record&columns%5B3%5D%5Bname%5D=Record&columns%5B3%5D%5Bsearchable%5D=false&columns%5B3%5D%5Borderable%5D=false&columns%5B3%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B3%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B4%5D%5Bdata%5D=Points&columns%5B4%5D%5Bname%5D=Points&columns%5B4%5D%5Bsearchable%5D=false&columns%5B4%5D%5Borderable%5D=true&columns%5B4%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B4%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B5%5D%5Bdata%5D=Tiebreaker1&columns%5B5%5D%5Bname%5D=Tiebreaker1&columns%5B5%5D%5Bsearchable%5D=false&columns%5B5%5D%5Borderable%5D=true&columns%5B5%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B5%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B6%5D%5Bdata%5D=Tiebreaker2&columns%5B6%5D%5Bname%5D=Tiebreaker2&columns%5B6%5D%5Bsearchable%5D=false&columns%5B6%5D%5Borderable%5D=true&columns%5B6%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B6%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B7%5D%5Bdata%5D=Tiebreaker3&columns%5B7%5D%5Bname%5D=Tiebreaker3&columns%5B7%5D%5Bsearchable%5D=false&columns%5B7%5D%5Borderable%5D=true&columns%5B7%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B7%5D%5Bsearch%5D%5Bregex%5D=false&order%5B0%5D%5Bcolumn%5D=0&order%5B0%5D%5Bdir%5D=asc&start={start}&length=25&search%5Bvalue%5D=&search%5Bregex%5D=false";
        static string _deckPage = "https://mtgmelee.com/Decklist/View/{deckId}";

        public static CacheItem GetTournamentDetails(MtgMeleeTournament tournament)
        {
            var decks = ParseDecks(tournament.Uri.ToString(), tournament);

            return new CacheItem()
            {
                Tournament = new Tournament(tournament),
                Decks = decks.Where(d => d.Item1 != null).Select(d => d.Item1).ToArray(),
                Standings = decks.Where(d => d.Item2 != null).Select(d => d.Item2).ToArray()
            };
        }

        private static Tuple<Deck, Standing>[] ParseDecks(string url, MtgMeleeTournament tournament)
        {
            List<Tuple<Deck, Standing>> result = new List<Tuple<Deck, Standing>>();

            string pageContent = new WebClient().DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var phaseNode = doc.DocumentNode.SelectNodes("//div[@id='standings-phase-selector-container']").First();
            var phaseId = phaseNode.SelectNodes("button[@class='btn btn-primary round-selector']").Last().Attributes["data-id"].Value;

            bool hasData;
            int offset = 0;
            int currentPosition = 0;
            do
            {
                hasData = false;

                string phaseParameters = _phaseParameters.Replace("{start}", offset.ToString());
                string phaseUrl = _phasePage.Replace("{phaseId}", phaseId);

                string json = Encoding.UTF8.GetString(new WebClient().UploadValues(phaseUrl, "POST", HttpUtility.ParseQueryString(phaseParameters)));
                var phase = JsonConvert.DeserializeObject<dynamic>(json);

                foreach (var player in phase.data)
                {
                    hasData = true;
                    string playerName = player.Name;
                    playerName = playerName.Trim();

                    Console.Write($"\r{new String(' ', Console.BufferWidth)}");
                    Console.Write($"\r[MtgMelee] Downloading player {playerName} ({++currentPosition})");

                    int playerPoints = player.Points;

                    int playerPosition = player.Rank;
                    string playerResult = playerPosition.ToString();
                    if (playerPosition == 1) playerResult += "st Place";
                    if (playerPosition == 2) playerResult += "nd Place";
                    if (playerPosition == 3) playerResult += "rd Place";
                    if (playerPosition > 3) playerResult += "th Place";

                    Standing standing = new Standing()
                    {
                        Player = playerName,
                        Rank = playerPosition,
                        Points = playerPoints
                    };

                    string playerDeckId = String.Empty;
                    List<string> playerDeckListIds = new List<string>();
                    foreach (var decklist in player.Decklists)
                    {
                        string deckListId = decklist.ID;
                        playerDeckListIds.Add(deckListId);
                    }
                    if (playerDeckListIds.Count > 0)
                    {
                        if (tournament.DeckOffset == null)
                        {
                            playerDeckId = playerDeckListIds.Last(); // Old behavior for compatibility reasons
                        }
                        else
                        {
                            if (playerDeckListIds.Count >= tournament.ExpectedDecks)
                            {
                                playerDeckId = playerDeckListIds[tournament.DeckOffset.Value];
                            }
                            else
                            {
                                if (tournament.FixBehavior == MtgMeleeMissingDeckBehavior.UseLast)
                                {
                                    playerDeckId = playerDeckListIds.Last();
                                }
                            }
                        }
                    }

                    Deck deck = null;
                    if (!String.IsNullOrEmpty(playerDeckId))
                    {
                        string deckPage = _deckPage.Replace("{deckId}", playerDeckId);
                        string deckPageContent = new WebClient().DownloadString(deckPage);

                        HtmlDocument deckDoc = new HtmlDocument();
                        deckDoc.LoadHtml(deckPageContent);

                        var copyButton = deckDoc.DocumentNode.SelectSingleNode("//button[@class='decklist-builder-copy-button btn-sm btn btn-card text-nowrap ']");
                        var cardList = WebUtility.HtmlDecode(copyButton.Attributes["data-clipboard-text"].Value).Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToArray();

                        List<DeckItem> mainBoard = new List<DeckItem>();
                        List<DeckItem> sideBoard = new List<DeckItem>();
                        bool insideSideboard = false;
                        bool insideCompanion = false;

                        foreach (var card in cardList)
                        {
                            if (card == "Deck" || card == "Companion" || card == "Sideboard")
                            {
                                if (card == "Companion")
                                {
                                    insideCompanion = true;
                                    insideSideboard = false;
                                }
                                else
                                {
                                    if (card == "Sideboard")
                                    {
                                        insideCompanion = false;
                                        insideSideboard = true;
                                    }
                                    else
                                    {
                                        insideCompanion = false;
                                        insideSideboard = false;
                                    }
                                }
                            }
                            else
                            {
                                if (insideCompanion) continue; // Companion is listed in its own section *and* in the sideboard as well

                                int splitPosition = card.IndexOf(" ");
                                int count = Convert.ToInt32(new String(card.Take(splitPosition).ToArray()));
                                string name = new String(card.Skip(splitPosition + 1).ToArray());
                                name = CardNameNormalizer.Normalize(name);

                                if (insideSideboard) sideBoard.Add(new DeckItem() { CardName = name, Count = count });
                                else mainBoard.Add(new DeckItem() { CardName = name, Count = count });
                            }
                        }

                        deck = new Deck()
                        {
                            AnchorUri = new Uri(deckPage),
                            Date = null,
                            Mainboard = mainBoard.ToArray(),
                            Sideboard = sideBoard.ToArray(),
                            Player = playerName,
                            Result = playerResult
                        };
                    }

                    result.Add(new Tuple<Deck, Standing>(deck, standing));
                }

                offset += 25;
            } while (hasData);

            return result.ToArray();
        }
    }
}