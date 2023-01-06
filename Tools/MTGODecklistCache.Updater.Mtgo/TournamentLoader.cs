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

            var bracket = ParseBracket(json);
            var standing = ParseStanding(json);
            var decks = ParseDecks(tournament, json);

            if(standing!=null && bracket!=null) decks = OrderNormalizer.ReorderDecks(decks, standing, bracket);

            return new CacheItem()
            {
                Standings = standing,
                Rounds = bracket,
                Decks = decks,
                Tournament = tournament
            };
        }

        private static Deck[] ParseDecks(Tournament tournament, dynamic json)
        {
            string eventType = json.event_type;
            DateTime eventDate = json.date;

            bool hasWinloss = HasProperty(json, "winloss");

            Dictionary<string, string> playerWinloss = new Dictionary<string, string>();
            if (hasWinloss)
            {
                foreach (var winloss in json.winloss)
                {
                    string player = winloss.player;
                    int wins = winloss.wins;
                    int losses = winloss.losses;

                    playerWinloss.Add(player, $"{wins}-{losses}");
                }
            }

            bool hasDecks = HasProperty(json, "decks");
            if (!hasDecks) return null;

            HashSet<string> addedPlayers = new HashSet<string>();
            var decks = new List<Deck>();
            int rank = 1;
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
                if (eventType == "tournament" && hasWinloss && playerWinloss.ContainsKey(player)) result = playerWinloss[player];
                if (eventType == "tournament" && !hasWinloss)
                {
                    if (rank == 1) result = "1st Place";
                    if (rank == 2) result = "2nd Place";
                    if (rank == 3) result = "3rd Place";
                    if (rank > 3) result = $"{rank}th Place";
                    rank++;
                }

                if (eventType == "tournament" && addedPlayers.Contains(player)) continue;

                decks.Add(new Deck()
                {
                    AnchorUri = new Uri($"{tournament.Uri.ToString()}#deck_{player}"),
                    Date = eventDate,
                    Player = player,
                    Mainboard = mainboard.Select(k => new DeckItem() { CardName = k.Key, Count = k.Value }).ToArray(),
                    Sideboard = sideboard.Select(k => new DeckItem() { CardName = k.Key, Count = k.Value }).ToArray(),
                    Result = result
                });
                addedPlayers.Add(player);
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

            return standings.OrderBy(s => s.Rank).ToArray();
        }

        private static Round[] ParseBracket(dynamic json)
        {
            if (!HasProperty(json, "Brackets")) return null;

            List<Round> brackets = new List<Round>();
            foreach (var bracket in json.Brackets)
            {
                List<RoundItem> matches = new List<RoundItem>();

                foreach (var match in bracket.matches)
                {
                    string player1 = match.players[0].player;
                    string player2 = match.players[1].player;
                    int player1Wins = match.players[0].Wins;
                    int player2Wins = match.players[1].Wins;
                    bool reverseOrder = match.players[1].Winner;

                    if (reverseOrder)
                    {
                        matches.Add(new RoundItem()
                        {
                            Player1 = player2,
                            Player2 = player1,
                            Result = $"{player2Wins}-{player1Wins}-0"
                        });
                    }
                    else
                    {
                        matches.Add(new RoundItem()
                        {
                            Player1 = player1,
                            Player2 = player2,
                            Result = $"{player1Wins}-{player2Wins}-0"
                        });
                    }
                }

                string roundName = "Quarterfinals";
                if (matches.Count == 2) roundName = "Semifinals";
                if (matches.Count == 1) roundName = "Finals";

                brackets.Add(new Round
                {
                    RoundName = roundName,
                    Matches = matches.ToArray()
                });
            }

            List<Round> result = new List<Round>();
            result.AddRange(brackets.Where(r => r.RoundName == "Quarterfinals"));
            result.AddRange(brackets.Where(r => r.RoundName == "Semifinals"));
            result.AddRange(brackets.Where(r => r.RoundName == "Finals"));

            return result.ToArray();
        }

        private static bool HasProperty(dynamic obj, string name)
        {
            var jobj = (JObject)obj;
            return obj.Property(name) != null;
        }
    }
}
