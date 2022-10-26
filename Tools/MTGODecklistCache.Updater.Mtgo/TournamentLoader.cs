using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace MTGODecklistCache.Updater.Mtgo
{
    public static class TournamentLoader
    {
        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            string htmlContent;
            using (WebClient client = new WebClient())
            {
                htmlContent = client.DownloadString(tournament.Uri);
            }

            var dataRow = htmlContent.Replace("\r", "").Split("\n").First(l => l.Trim().StartsWith("window.MTGO.decklists.data"));
            int cutStart = dataRow.IndexOf("{");

            var jsonData = dataRow.Substring(cutStart, dataRow.Length - cutStart - 1);
            dynamic json = JsonConvert.DeserializeObject(jsonData);

            return new CacheItem()
            {
                Bracket = null,
                Standings = ParseStanding(json),
                Rounds = null,
                Decks = ParseDecks(tournament, json),
                Tournament = tournament
            };
        }

        private static Deck[] ParseDecks(Tournament tournament, dynamic json)
        {
            string eventType = json.event_type;
            DateTime eventDate = json.date;

            Dictionary<string, string> playerWinloss = new Dictionary<string, string>();
            if (eventType == "tournament")
            {
                foreach (var winloss in json.winloss)
                {
                    string player = winloss.player;
                    int wins = winloss.wins;
                    int losses = winloss.losses;

                    playerWinloss.Add(player, $"{wins}-{losses}");
                }
            }

            var decks = new List<Deck>();
            foreach (var deck in json.decks)
            {
                Dictionary<string, int> mainboard = new Dictionary<string, int>();
                Dictionary<string, int> sideboard = new Dictionary<string, int>();
                string player = deck.player;

                foreach (var deckSection in deck.deck)
                {
                    bool isSb = deckSection.SB;
                    Dictionary<string, int> deckSectionItems = isSb ? sideboard : mainboard;

                    foreach (var card in deckSection.DECK_CARDS)
                    {
                        string name = card.CARD_ATTRIBUTES.NAME;
                        int quantity = card.Quantity;

                        name = CardNameNormalizer.Normalize(name);

                        // JSON may contain multiple entries for the same card, likely if they come from different sets
                        if (!deckSectionItems.ContainsKey(name)) deckSectionItems.Add(name, 0);
                        deckSectionItems[name] += quantity;
                    }
                }

                string result = String.Empty;
                if (eventType == "league") result = "5-0";
                if (eventType == "tournament") result = playerWinloss[player];

                decks.Add(new Deck()
                {
                    AnchorUri = new Uri($"{tournament.Uri.ToString()}#deck_{player}"),
                    Date = eventDate,
                    Player = player,
                    Mainboard = mainboard.Select(k => new DeckItem() { CardName = k.Key, Count = k.Value }).ToArray(),
                    Sideboard = sideboard.Select(k => new DeckItem() { CardName = k.Key, Count = k.Value }).ToArray(),
                    Result = result
                });
            }

            return decks.ToArray();
        }

        private static Standing[] ParseStanding(dynamic json)
        {
            if (!HasProperty(json, "STANDINGS")) return null;

            List<Standing> standings = new List<Standing>();

            foreach (var standing in json.STANDINGS)
            {
                string player = standing.NAME;
                int points = standing.POINTS;
                int rank = standing.RANK;
                double GWP = standing.GWP;
                double OGWP = standing.OGWP;
                double OMWP = standing.OMWP;

                standings.Add(new Standing()
                {
                    Player = player,
                    Points = points,
                    Rank = rank,
                    GWP = GWP,
                    OGWP = OGWP,
                    OMWP = OMWP
                });
            }

            return standings.ToArray();
        }

        //private static Standing[] ParseStandings(HtmlDocument doc)
        //{
        //    var standingsRoot = doc.DocumentNode.SelectSingleNode("//table[@class='sticky-enabled']");
        //    if (standingsRoot == null) return null;

        //    List<Standing> result = new List<Standing>();

        //    var standingNodes = standingsRoot.SelectNodes("tbody/tr");
        //    foreach (var standingNode in standingNodes)
        //    {
        //        var rows = standingNode.SelectNodes("td");

        //        int rank = int.Parse(rows[0].InnerText);
        //        string player = rows[1].InnerText.Trim();
        //        int points = int.Parse(rows[2].InnerText);
        //        double omwp = double.Parse(rows[3].InnerText, CultureInfo.InvariantCulture);
        //        double gwp = double.Parse(rows[4].InnerText, CultureInfo.InvariantCulture);
        //        double ogwp = double.Parse(rows[5].InnerText, CultureInfo.InvariantCulture);

        //        result.Add(new Standing()
        //        {
        //            Rank = rank,
        //            Player = player,
        //            Points = points,
        //            OMWP = omwp,
        //            GWP = gwp,
        //            OGWP = ogwp
        //        });
        //    }

        //    return result.ToArray();
        //}

        //private static Bracket ParseBracket(HtmlDocument doc)
        //{
        //    var bracketRoot = doc.DocumentNode.SelectSingleNode("//div[@class='wrap-bracket-slider']");
        //    if (bracketRoot == null) return null;

        //    var bracketNodes = bracketRoot.SelectNodes("div/div[@class='finalists']");

        //    return new Bracket()
        //    {
        //        Quarterfinals = ParseBracketItem(bracketNodes.Skip(0).First()),
        //        Semifinals = ParseBracketItem(bracketNodes.Skip(1).First()),
        //        Finals = ParseBracketItem(bracketNodes.Skip(2).First()).First()
        //    };
        //}

        //private static BracketItem[] ParseBracketItem(HtmlNode node)
        //{
        //    var playerNodes = node.SelectNodes("div/div[@class='player']");

        //    List<string> players = new List<string>();
        //    foreach (var playerNode in playerNodes) players.Add(playerNode.InnerText);

        //    // Cleans up player names
        //    players = players
        //        .Select(p => p.Trim())
        //        .Select(p => Regex.Replace(p, @"^\(\d+\)\s*", ""))
        //        .ToList();

        //    List<BracketItem> result = new List<BracketItem>();
        //    for (var i = 0; i < players.Count; i = i + 2)
        //    {
        //        result.Add(new BracketItem()
        //        {
        //            Player1 = players[i].Split(",").First(),
        //            Result = players[i].Split(", ").Last(),
        //            Player2 = players.Count > 1 ? players[i + 1] : ""
        //        });
        //    }

        //    return result.ToArray();
        //}

        //private static DateTime ExtractDateFromUrl(Uri eventUri)
        //{
        //    string eventPath = eventUri.LocalPath;
        //    string[] eventPathSegments = eventPath.Split("-").Where(e => e.Length > 1).ToArray();
        //    string eventDate = String.Join("-", eventPathSegments.Skip(eventPathSegments.Length - 3).ToArray());

        //    if (DateTime.TryParse(eventDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime parsedDate))
        //    {
        //        return parsedDate.ToUniversalTime();
        //    }
        //    else
        //    {
        //        // This is only used to decide or not to bypass cache, so it's safe to return a fallback for today forcing the bypass
        //        return DateTime.UtcNow.Date;
        //    }
        //}

        private static bool HasProperty(dynamic obj, string name)
        {
            var jobj = (JObject)obj;
            return obj.Property(name) != null;
        }
    }
}
