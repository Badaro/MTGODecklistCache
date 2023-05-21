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
        static string _phasePage = "https://melee.gg/Tournament/GetPhaseStandings/{phaseId}";
        static string _phaseParameters = "columns%5B0%5D%5Bdata%5D=Rank&columns%5B0%5D%5Bname%5D=Rank&columns%5B0%5D%5Bsearchable%5D=false&columns%5B0%5D%5Borderable%5D=true&columns%5B0%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B0%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B1%5D%5Bdata%5D=Name&columns%5B1%5D%5Bname%5D=Name&columns%5B1%5D%5Bsearchable%5D=true&columns%5B1%5D%5Borderable%5D=true&columns%5B1%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B1%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B2%5D%5Bdata%5D=Decklists&columns%5B2%5D%5Bname%5D=Decklists&columns%5B2%5D%5Bsearchable%5D=false&columns%5B2%5D%5Borderable%5D=false&columns%5B2%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B2%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B3%5D%5Bdata%5D=Record&columns%5B3%5D%5Bname%5D=Record&columns%5B3%5D%5Bsearchable%5D=false&columns%5B3%5D%5Borderable%5D=false&columns%5B3%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B3%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B4%5D%5Bdata%5D=Points&columns%5B4%5D%5Bname%5D=Points&columns%5B4%5D%5Bsearchable%5D=false&columns%5B4%5D%5Borderable%5D=true&columns%5B4%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B4%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B5%5D%5Bdata%5D=Tiebreaker1&columns%5B5%5D%5Bname%5D=Tiebreaker1&columns%5B5%5D%5Bsearchable%5D=false&columns%5B5%5D%5Borderable%5D=true&columns%5B5%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B5%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B6%5D%5Bdata%5D=Tiebreaker2&columns%5B6%5D%5Bname%5D=Tiebreaker2&columns%5B6%5D%5Bsearchable%5D=false&columns%5B6%5D%5Borderable%5D=true&columns%5B6%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B6%5D%5Bsearch%5D%5Bregex%5D=false&columns%5B7%5D%5Bdata%5D=Tiebreaker3&columns%5B7%5D%5Bname%5D=Tiebreaker3&columns%5B7%5D%5Bsearchable%5D=false&columns%5B7%5D%5Borderable%5D=true&columns%5B7%5D%5Bsearch%5D%5Bvalue%5D=&columns%5B7%5D%5Bsearch%5D%5Bregex%5D=false&order%5B0%5D%5Bcolumn%5D=0&order%5B0%5D%5Bdir%5D=asc&start={start}&length=25&search%5Bvalue%5D=&search%5Bregex%5D=false";
        static string _deckPage = "https://melee.gg/Decklist/View/{deckId}";

        public static CacheItem GetTournamentDetails(MtgMeleeTournament tournament)
        {
            var data = ParseDecks(tournament.Uri.ToString(), tournament);

            // Consolidates matches from deck pages and remove duplicates
            Dictionary<string, Dictionary<string, RoundItem>> consolidatedRounds = new Dictionary<string, Dictionary<string, RoundItem>>();
            foreach (var deckData in data)
            {
                if (deckData.Rounds != null)
                {
                    foreach (var deckRound in deckData.Rounds)
                    {
                        if (!consolidatedRounds.ContainsKey(deckRound.RoundName)) consolidatedRounds.Add(deckRound.RoundName, new Dictionary<string, RoundItem>());
                        string roundItemKey = $"{deckRound.RoundName}_{deckRound.Matches[0].Player1}_{deckRound.Matches[0].Player2}";
                        if (!consolidatedRounds[deckRound.RoundName].ContainsKey(roundItemKey)) consolidatedRounds[deckRound.RoundName].Add(roundItemKey, deckRound.Matches[0]);
                    }
                }
            }

            var decks = data.Where(d => d.Deck != null).Select(d => d.Deck).ToArray();
            var standings = data.Where(d => d.Standing != null).Select(d => d.Standing).ToArray();
            var rounds = consolidatedRounds.Select(r => new Round() { RoundName = r.Key, Matches = r.Value.Select(m => m.Value).ToArray() }).ToArray();

            var bracket = new List<Round>();
            bracket.AddRange(rounds.Where(r => r.RoundName == "Quarterfinals"));
            bracket.AddRange(rounds.Where(r => r.RoundName == "Semifinals"));
            bracket.AddRange(rounds.Where(r => r.RoundName == "Finals"));

            if(bracket.Count() > 0)
            {
                decks = OrderNormalizer.ReorderDecks(decks, standings, bracket.ToArray());
            }

            return new CacheItem()
            {
                Tournament = new Tournament(tournament),
                Decks = decks,
                Standings = standings,
                Rounds = rounds
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

                int bufferWidth = 80;
                // try { bufferWidth = Console.BufferWidth; } catch { };

                foreach (var player in phase.data)
                {
                    hasData = true;
                    string playerName = player.Name;
                    playerName = NormalizeSpaces(playerName);

                    if (bufferWidth > 0)
                    {
                        Console.Write($"\r{new String(' ', bufferWidth)}");
                        Console.Write($"\r[MtgMelee] Downloading player {playerName} ({++currentPosition})");
                    }
                    else
                    {
                        Console.WriteLine($"[MtgMelee] Downloading player {playerName} ({++currentPosition})");
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

                        var roundsDiv = deckDoc.DocumentNode.SelectSingleNode("//div[@id='tournament-path-grid-item']");
                        if (roundsDiv != null)
                        {
                            rounds = new List<Round>();
                            foreach (var roundDiv in roundsDiv.SelectNodes("div/div/div/table/tbody/tr"))
                            {
                                var round = ParseRoundNode(playerName, roundDiv);
                                if (round != null) rounds.Add(round);
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

        private static Round ParseRoundNode(string playerName, HtmlNode roundNode)
        {
            var roundColumns = roundNode.SelectNodes("td");
            if (roundColumns.First().InnerText.Trim() == "No results found") return null;

            string roundName = roundColumns.First().InnerHtml;
            string roundOpponent = roundColumns.Skip(1).First().SelectSingleNode("a")?.InnerHtml;
            if (roundOpponent == null)
            {
                roundOpponent = "-";
            }
            string roundResult = roundColumns.Skip(3).First().InnerHtml;

            roundName = NormalizeSpaces(WebUtility.HtmlDecode(roundName));
            roundOpponent = NormalizeSpaces(WebUtility.HtmlDecode(roundOpponent));
            roundResult = NormalizeSpaces(WebUtility.HtmlDecode(roundResult));

            RoundItem item = null;
            if (roundResult.StartsWith($"{playerName} won"))
            {
                // Victory
                item = new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = roundOpponent,
                    Result = roundResult.Split(" ").Last()
                };
            }
            if (roundResult.StartsWith($"{roundOpponent} won"))
            {
                // Defeat
                item = new RoundItem()
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
                    item = new RoundItem()
                    {
                        Player1 = playerName,
                        Player2 = roundOpponent,
                        Result = roundResult.Split(" ").First()
                    };
                }
                else
                {
                    item = new RoundItem()
                    {
                        Player1 = roundOpponent,
                        Player2 = playerName,
                        Result = roundResult.Split(" ").First()
                    };
                }
            }
            if (roundResult.EndsWith(" bye"))
            {
                item = new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = "-",
                    Result = "2-0-0"
                };
            }
            if (roundResult.StartsWith($"{playerName} forfeited"))
            {
                // Victory
                item = new RoundItem()
                {
                    Player1 = playerName,
                    Player2 = roundOpponent,
                    Result = "0-2-0"
                };
            }
            if (roundResult.StartsWith($"Not reported"))
            {
                if (String.Compare(playerName, roundOpponent) < 0)
                {
                    item = new RoundItem()
                    {
                        Player1 = playerName,
                        Player2 = roundOpponent,
                        Result = "0-0-0"
                    };
                }
                else
                {
                    item = new RoundItem()
                    {
                        Player1 = roundOpponent,
                        Player2 = playerName,
                        Result = "0-0-0"
                    };
                }
            }

            if (item == null) throw new FormatException($"Cannot parse round data for player {playerName} and opponent {roundOpponent}");

            return new Round
            {
                RoundName = roundName,
                Matches = new RoundItem[]
                {
                    item
                }
            };
        }

        private static string NormalizeSpaces(string data)
        {
            return Regex.Replace(data, "\\s+", " ").Trim();
        }
    }
}