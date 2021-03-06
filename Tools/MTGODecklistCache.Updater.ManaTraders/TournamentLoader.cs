﻿using CsvHelper;
using CsvHelper.Configuration;
using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
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

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks,
                Standings = standings,
                Bracket = bracket,
                Rounds = swiss
            };
        }

        private static Dictionary<string, Uri> ParseDeckUris(string rootUrl)
        {
            Dictionary<string, Uri> result = new Dictionary<string, Uri>();
            string pageContent = new WebClient().DownloadString(rootUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var tables = doc.DocumentNode.SelectNodes("//table[@class='table table-tournament-rankings']").ToArray();
            if (tables.Length < 3) return result;

            var deckTables = tables[1];
            foreach (var row in deckTables.SelectNodes("tbody/tr"))
            {
                var columns = row.SelectNodes("td").ToArray();
                if (columns.Length < 6) continue;

                var playerColumn = columns[1];
                var urlColumn = columns[5];
                var urlLink = urlColumn.SelectSingleNode("a");

                if(urlLink!=null)
                {
                    var playerName = playerColumn.InnerText.Trim().ToLower();
                    var playerUri = urlLink.Attributes["href"].Value;

                    result.Add(playerName, new Uri(playerUri));
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
                string playerResult = null;
                if (standings != null)
                {
                    var playerStanding = standings.FirstOrDefault(s => s.Player.ToLower().Trim() == player.ToLower().Trim());

                    if (playerStanding != null)
                    {
                        var rankSuffix = "th";
                        if (playerStanding.Rank == 1) rankSuffix = "st";
                        if (playerStanding.Rank == 2) rankSuffix = "nd";
                        if (playerStanding.Rank == 3) rankSuffix = "rd";

                        playerResult = playerStanding.Rank.ToString() + rankSuffix + " Place";
                        playerName = playerStanding.Player.Trim(); // Name from standings has the correct casing, name from CSV is forced lowercase
                    }
                    else
                    {
                        playerResult = "-";
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
                    Result = playerResult,
                    Mainboard = playerCards.Where(c => !c.Sideboard).Select(c => new DeckItem() { Count = c.Count, CardName = FixCardName(c.Card) }).ToArray(),
                    Sideboard = playerCards.Where(c => c.Sideboard).Select(c => new DeckItem() { Count = c.Count, CardName = FixCardName(c.Card) }).ToArray(),
                });
            }

            return result.OrderBy(r => r.Result != "-" ? Int32.Parse(r.Result.Substring(0, r.Result.Length - 8)) : Int32.MaxValue).ToArray();
        }

        private static string FixCardName(string cardName)
        {
            // Normalizes card format with MTGO website
            if (cardName.Contains("Full Art")) return cardName.Replace("Full Art", "").Trim();
            if (cardName.Contains("/")) return cardName.Replace("/", " // ");
            return cardName;
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

        private static Bracket ParseBracket(string bracketUrl)
        {
            string pageContent = new WebClient().DownloadString(bracketUrl);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var bracketRoot = doc.DocumentNode.SelectSingleNode("//div[@class='tournament-brackets']");
            if (bracketRoot == null) return null;

            List<BracketItem> brackets = new List<BracketItem>();

            var bracketNodes = bracketRoot.SelectNodes("ul/li");
            foreach (var bracketNode in bracketNodes)
            {
                List<string> players = new List<string>();
                List<int> wins = new List<int>();

                foreach (var playerNode in bracketNode.SelectNodes("div"))
                {
                    players.Add(playerNode.SelectNodes("div").First().InnerText);
                    wins.Add(Convert.ToInt32(playerNode.SelectNodes("div").Last().InnerText));
                }

                if (wins[0] > wins[1])
                {
                    brackets.Add(new BracketItem()
                    {
                        Player1 = players[0],
                        Player2 = players[1],
                        Result = wins[0] + "-" + wins[1]
                    });
                }
                else
                {
                    brackets.Add(new BracketItem()
                    {
                        Player1 = players[1],
                        Player2 = players[0],
                        Result = wins[1] + "-" + wins[0]
                    });
                }
            }

            return new Bracket()
            {
                Quarterfinals = brackets.Take(4).ToArray(),
                Semifinals = brackets.Skip(4).Take(2).ToArray(),
                Finals = brackets.Skip(6).First()
            };
        }

        private static Round[] ParseSwiss(string swissUrl)
        {
            List<Round> result = new List<Round>();

            string jsonData = new WebClient().DownloadString(swissUrl);
            dynamic json = JsonConvert.DeserializeObject(jsonData);

            var jObj = (JObject)json;

            foreach (JProperty round in jObj.Children())
            {
                int roundNumber = Int32.Parse(round.Name.Replace("Round ", ""));

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
                    RoundNumber = roundNumber,
                    Matches = items.ToArray()
                });
            }

            return result.ToArray();
        }
    }
}