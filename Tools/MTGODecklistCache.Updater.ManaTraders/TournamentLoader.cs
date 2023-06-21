using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MTGODecklistCache.Updater.ManaTraders
{
    public static class TournamentLoader
    {
        static string _csvRoot = "https://www.manatraders.com/tournaments/download_csv_by_month_and_year?month={month}&year={year}";
        static string _swissRoot = "https://www.manatraders.com/tournaments/swiss_json_by_month_and_year?month={month}&year={year}";

        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            string csvUrl = _csvRoot.Replace("{year}", tournament.Date.Year.ToString()).Replace("{month}", tournament.Date.Month.ToString());
            string swissUrl = _swissRoot.Replace("{year}", tournament.Date.Year.ToString()).Replace("{month}", tournament.Date.Month.ToString());
            string standingsUrl = $"{tournament.Uri.ToString()}swiss";
            string bracketUrl = $"{tournament.Uri.ToString()}finals";

            var standings = ParseStandings(standingsUrl);
            var deckUris = ParseDeckUris(tournament.Uri.ToString());
            var decks = ParseDecks(csvUrl, standings, deckUris);
            var bracket = ParseBracket(bracketUrl);
            var swiss = ParseSwiss(swissUrl);

            var rounds = new List<Round>();
            rounds.AddRange(swiss);
            rounds.AddRange(bracket);

            decks = OrderNormalizer.ReorderDecks(decks, standings, bracket);

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks,
                Standings = standings,
                Rounds = rounds.ToArray()
            };
        }

        private static Dictionary<string, Uri> ParseDeckUris(string rootUrl)
        {
            Dictionary<string, Uri> result = new Dictionary<string, Uri>();
            string pageContent = new WebClient().DownloadString(rootUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var tables = doc.DocumentNode.SelectNodes("//table[@class='table table-tournament-rankings']").ToArray();
            if (tables.Length < 2) return result;

            var deckTables = tables[tables.Length-2];
            foreach (var row in deckTables.SelectNodes("tbody/tr"))
            {
                var columns = row.SelectNodes("td").ToArray();
                if (columns.Length < 6) continue;

                var playerColumn = columns[1];
                var urlColumn = columns[5];
                var urlLink = urlColumn.SelectSingleNode("a");

                if (urlLink != null)
                {
                    var playerName = playerColumn.InnerText.Trim().ToLower();
                    var playerUri = urlLink.Attributes["href"].Value;

                    if (!result.ContainsKey(playerName)) result.Add(playerName, new Uri(playerUri));
                }
            }

            return result;
        }

        private static Deck[] ParseDecks(string csvUrl, Standing[] standings, Dictionary<string, Uri> deckUris)
        {
            List<Deck> result = new List<Deck>();

            string csvData = new WebClient().DownloadString(csvUrl);

            ManaTradersCsvRecord[] records;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData)))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true }))
                    {
                        records = csv.GetRecords<ManaTradersCsvRecord>().ToArray();
                    }
                }
            }

            foreach (var player in records.Select(r => r.Player).Distinct())
            {
                ManaTradersCsvRecord[] playerCards = records.Where(r => r.Player.Trim() == player.Trim()).ToArray();

                string playerName = player;
                if (standings != null)
                {
                    var playerStanding = standings.FirstOrDefault(s => s.Player.ToLower().Trim() == player.ToLower().Trim());

                    if (playerStanding != null)
                    {
                        playerName = playerStanding.Player.Trim(); // Name from standings has the correct casing, name from CSV is forced lowercase
                    }
                    else
                    {
                        playerName = player.Trim();
                    }
                }

                Uri deckUri = null;
                if (deckUris.ContainsKey(playerName.ToLowerInvariant())) deckUri = deckUris[playerName.ToLowerInvariant()];

                result.Add(new Deck()
                {
                    AnchorUri = deckUri,
                    Date = null,
                    Player = playerName,
                    Mainboard = playerCards.Where(c => !c.Sideboard).Select(c => new DeckItem() { Count = c.Count, CardName = CardNameNormalizer.Normalize(c.Card) }).ToArray(),
                    Sideboard = playerCards.Where(c => c.Sideboard).Select(c => new DeckItem() { Count = c.Count, CardName = CardNameNormalizer.Normalize(c.Card) }).ToArray(),
                });
            }

            return result.ToArray();
        }

        private static Standing[] ParseStandings(string standingsUrl)
        {
            string pageContent = new WebClient().DownloadString(standingsUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var standingsRoot = doc.DocumentNode.SelectSingleNode("//table[@class='table table-tournament-rankings']");
            if (standingsRoot == null) return null;

            List<Standing> result = new List<Standing>();

            var standingNodes = standingsRoot.SelectNodes("tbody/tr");
            foreach (var standingNode in standingNodes)
            {
                var rows = standingNode.SelectNodes("td");

                int rank = int.Parse(rows[0].InnerText);
                string player = rows[1].InnerText.Trim();
                int points = int.Parse(rows[2].InnerText);
                double omwp = double.Parse(rows[5].InnerText.Trim('%'), CultureInfo.InvariantCulture) / 100d;
                double gwp = double.Parse(rows[6].InnerText.Trim('%'), CultureInfo.InvariantCulture) / 100d;
                double ogwp = double.Parse(rows[7].InnerText.Trim('%'), CultureInfo.InvariantCulture) / 100d;

                result.Add(new Standing()
                {
                    Rank = rank,
                    Player = player,
                    Points = points,
                    OMWP = omwp,
                    GWP = gwp,
                    OGWP = ogwp
                });
            }

            return result.ToArray();
        }

        private static Round[] ParseBracket(string bracketUrl)
        {
            string pageContent = new WebClient().DownloadString(bracketUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var bracketRootNodes = doc.DocumentNode.SelectNodes("//div[@class='tournament-brackets']");

            List<RoundItem> brackets = new List<RoundItem>();

            foreach (var bracketRoot in bracketRootNodes)
            {
                var bracketNodes = bracketRoot.SelectNodes("ul/li");
                foreach (var bracketNode in bracketNodes)
                {
                    List<string> players = new List<string>();
                    List<int> wins = new List<int>();

                    foreach (var playerNode in bracketNode.SelectNodes("div"))
                    {
                        players.Add(playerNode.SelectNodes("div").First().InnerText);

                        string playerWins = playerNode.SelectNodes("div").Last().InnerText;
                        if (Int32.TryParse(playerWins, out int parsedWins)) wins.Add(parsedWins);
                        else wins.Add(0);
                    }

                    if (players[0] == "-") continue;

                    if (wins[0] > wins[1])
                    {
                        brackets.Add(new RoundItem()
                        {
                            Player1 = players[0],
                            Player2 = players[1],
                            Result = wins[0] + "-" + wins[1] + "-0"
                        });
                    }
                    else
                    {
                        brackets.Add(new RoundItem()
                        {
                            Player1 = players[1],
                            Player2 = players[0],
                            Result = wins[1] + "-" + wins[0] + "-0"
                        });
                    }
                }
            }

            List<Round> rounds = new List<Round>();
            if (brackets.Count == 7)
            {
                // No extra rounds
                rounds.Add(new Round()
                {
                    RoundName = "Quarterfinals",
                    Matches = brackets.Take(4).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Semifinals",
                    Matches = brackets.Skip(4).Take(2).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Finals",
                    Matches = brackets.Skip(6).ToArray()
                });
            }
            else
            {
                rounds.Add(new Round()
                {
                    RoundName = "Quarterfinals",
                    Matches = brackets.Take(4).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Loser Semifinals",
                    Matches = brackets.Skip(10).Take(2).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Semifinals",
                    Matches = brackets.Skip(4).Take(2).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Match for 7th and 8th places",
                    Matches = brackets.Skip(15).Take(1).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Match for 5th and 6th places",
                    Matches = brackets.Skip(12).Take(1).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Match for 3rd and 4th places",
                    Matches = brackets.Skip(9).Take(1).ToArray()
                });
                rounds.Add(new Round()
                {
                    RoundName = "Finals",
                    Matches = brackets.Skip(6).Take(1).ToArray()
                });

            }

            return rounds.Where(r => r.Matches.Length > 0).ToArray();
        }

        private static Round[] ParseSwiss(string swissUrl)
        {
            List<Round> result = new List<Round>();

            string jsonData = new WebClient().DownloadString(swissUrl);
            dynamic json = JsonConvert.DeserializeObject(jsonData);

            var jObj = (JObject)json;

            foreach (JProperty round in jObj.Children())
            {
                string roundName = round.Name;

                var matches = round.Children().ToArray().Children().ToArray();

                List<RoundItem> items = new List<RoundItem>();
                foreach (JToken match in matches)
                {
                    string p1 = match.Value<string>("p1");
                    string p2 = match.Value<string>("p2");
                    string res = match.Value<string>("res");
                    items.Add(new RoundItem()
                    {
                        Player1 = p1,
                        Player2 = p2,
                        Result = res
                    });
                }

                result.Add(new Round()
                {
                    RoundName = roundName,
                    Matches = items.ToArray()
                });
            }

            return result.ToArray();
        }
    }
}