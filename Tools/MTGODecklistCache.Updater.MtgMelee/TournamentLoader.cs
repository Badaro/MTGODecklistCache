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

            // Consolidates matches from deck pages and remove duplicates
            Dictionary<int, Dictionary<string, RoundItem>> rounds = new Dictionary<int, Dictionary<string, RoundItem>>();
            foreach (var deck in decks)
            {
                if (deck.Rounds != null)
                {
                    foreach (var round in deck.Rounds)
                    {
                        if (!rounds.ContainsKey(round.RoundNumber)) rounds.Add(round.RoundNumber, new Dictionary<string, RoundItem>());
                        string roundItemKey = $"{round.RoundNumber}_{round.Matches[0].Player1}_{round.Matches[0].Player2}";
                        if (!rounds[round.RoundNumber].ContainsKey(roundItemKey)) rounds[round.RoundNumber].Add(roundItemKey, round.Matches[0]);
                    }
                }
            }

            return new CacheItem()
            {
                Tournament = new Tournament(tournament),
                Decks = decks.Where(d => d.Deck != null).Select(d => d.Deck).ToArray(),
                Standings = decks.Where(d => d.Standing != null).Select(d => d.Standing).ToArray(),
                Rounds = rounds.Select(r => new Round() { RoundNumber = r.Key, Matches = r.Value.Select(m => m.Value).ToArray() }).ToArray()
            };
        }

        private static MtgMeleeDeckInfo[] ParseDecks(string url, MtgMeleeTournament tournament)
        {
            List<MtgMeleeDeckInfo> result = new List<MtgMeleeDeckInfo>();

            string pageContent = new WebClient().DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var phaseNode = doc.DocumentNode.SelectNodes("//div[@id='standings-phase-selector-container']").First();
            var phaseId = phaseNode.SelectNodes("button[@class='btn btn-primary round-selector']").Last().Attributes["data-id"].Value;
            if (tournament.PhaseOffset.HasValue) phaseId = phaseNode.SelectNodes("button[@class='btn btn-primary round-selector']").Skip(tournament.PhaseOffset.Value).First().Attributes["data-id"].Value;

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
                    playerName = NormalizeSpaces(playerName);

                    int bufferWidth = 0;
                    try { bufferWidth = Console.BufferWidth; } catch { };

                    if (bufferWidth > 0)
                    {
                        Console.Write($"\r{new String(' ', bufferWidth)}");
                        Console.Write($"\r[MtgMelee] Downloading player {playerName} ({++currentPosition})");
                    }
                    else
                    {
                        Console.Write($"[MtgMelee] Downloading player {playerName} ({++currentPosition})");
                    }

                    int playerPoints = player.Points;
                    double omwp = player.Tiebreaker1;
                    double gwp = player.Tiebreaker2;
                    double ogwp = player.Tiebreaker3;

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
                        Points = playerPoints,
                        OMWP = omwp,
                        GWP = gwp,
                        OGWP = ogwp
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
                    List<Round> rounds = null;
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

                        int roundNumber = 1;
                        var roundsDiv = deckDoc.DocumentNode.SelectSingleNode("//div[@id='tournament-path-grid-item']");
                        if (roundsDiv != null)
                        {
                            rounds = new List<Round>();
                            foreach (var roundDiv in roundsDiv.SelectNodes("div/div/div/table/tbody/tr"))
                            {
                                var round = ParseRoundNode(playerName, roundDiv);
                                if (round == null) continue;

                                rounds.Add(new Round()
                                {
                                    RoundNumber = roundNumber++,
                                    Matches = new RoundItem[]
                                    {
                                        round
                                    }
                                });
                            }
                        }
                    }

                    result.Add(new MtgMeleeDeckInfo() { Deck = deck, Standing = standing, Rounds = rounds?.ToArray() });
                }

                offset += 25;
            } while (hasData);

            Console.WriteLine($"\r[MtgMelee] Download completed");
            return result.ToArray();
        }

        private static RoundItem ParseRoundNode(string playerName, HtmlNode roundNode)
        {
            var roundColumns = roundNode.SelectNodes("td");
            if (roundColumns.First().InnerText.Trim() == "No results found") return null;

            string roundOpponent = roundColumns.Skip(1).First().SelectSingleNode("a")?.InnerHtml;
            if (roundOpponent == null)
            {
                roundOpponent = "-";
            }
            string roundResult = roundColumns.Skip(3).First().InnerHtml;

            roundOpponent = NormalizeSpaces(WebUtility.HtmlDecode(roundOpponent));
            roundResult = NormalizeSpaces(WebUtility.HtmlDecode(roundResult));

            if (roundResult.StartsWith($"{playerName} won"))
            {
                // Victory
                return new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = roundOpponent,
                    Result = roundResult.Split(" ").Last()
                };
            }
            if (roundResult.StartsWith($"{roundOpponent} won"))
            {
                // Defeat
                return new RoundItem()
                {
                    Player1 = roundOpponent,
                    Player2 = playerName,
                    Result = roundResult.Split(" ").Last()
                };
            }
            if (roundResult.EndsWith("Draw"))
            {
                if (String.Compare(playerName, roundOpponent) < 0)
                {
                    return new RoundItem()
                    {
                        Player1 = playerName,
                        Player2 = roundOpponent,
                        Result = roundResult.Split(" ").First()
                    };
                }
                else
                {
                    return new RoundItem()
                    {
                        Player1 = roundOpponent,
                        Player2 = playerName,
                        Result = roundResult.Split(" ").First()
                    };
                }
            }
            if (roundResult.EndsWith(" bye"))
            {
                return new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = "-",
                    Result = "2-0-0"
                };
            }
            if (roundResult.StartsWith($"{playerName} forfeited"))
            {
                // Victory
                return new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = roundOpponent,
                    Result = "0-2-0"
                };
            }
            throw new FormatException($"Cannot parse round data for player {playerName} and opponent {roundOpponent}");
        }

        private static string NormalizeSpaces(string data)
        {
            return Regex.Replace(data, "\\s+", " ").Trim();
        }
    }
}