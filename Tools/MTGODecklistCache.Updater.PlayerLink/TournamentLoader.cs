using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
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

namespace MTGODecklistCache.Updater.PlayerLink
{
    public static class TournamentLoader
    {
        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            var standingsByPlayer = GetStandingsByPlayer(tournament.Uri.ToString());
            var decks = GetDecks(tournament.Uri.ToString().Replace("/standings/", "/deck/"), tournament.Date, standingsByPlayer);

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks.ToArray(),
                Standings = standingsByPlayer.Select(s => s.Value).ToArray()
            };
        }


        private static Dictionary<string, Standing> GetStandingsByPlayer(string standingsUrl)
        {
            Dictionary<string, Standing> result = new Dictionary<string, Standing>(StringComparer.InvariantCultureIgnoreCase);

            string pageContent = new WebClient().DownloadString(standingsUrl);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var standingsTable = doc.DocumentNode.SelectNodes("//table").First();

            foreach (var row in standingsTable.SelectNodes("tbody/tr"))
            {
                var columns = row.SelectNodes("td").ToArray();
                int rank = Int32.Parse(columns[0].InnerText);
                string player = columns[1].InnerText;
                int points = Int32.Parse(columns[2].InnerText);
                double omw = Double.Parse(columns[3].InnerText, CultureInfo.InvariantCulture);
                double pgw = Double.Parse(columns[4].InnerText, CultureInfo.InvariantCulture);
                double ogw = Double.Parse(columns[5].InnerText, CultureInfo.InvariantCulture);

                result.Add(player, new Standing()
                {
                    Player = player,
                    Rank = rank,
                    Points = points,
                    OMWP = omw,
                    GWP = pgw,
                    OGWP = ogw
                });
            }

            return result;
        }

        private static List<Deck> GetDecks(string decksUri, DateTime eventDate, Dictionary<string, Standing> standings)
        {
            List<Deck> result = new List<Deck>();

            string pageContent = new WebClient().DownloadString(decksUri);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var playerTable = doc.DocumentNode.SelectNodes("//table").First();
            foreach (var row in playerTable.SelectNodes("tbody/tr"))
            {
                var columns = row.SelectNodes("td").ToArray();
                string lastName = columns[0].InnerText;
                string firstName = columns[1].InnerText;
                string deckUrl = columns[3].SelectSingleNode("a").GetAttributeValue("href", null);

                string rawDeckUrl = deckUrl.Replace("deck/view", "deck/raw");
                string rawDeckContent = new WebClient().DownloadString(rawDeckUrl);

                HtmlDocument rawDeck = new HtmlDocument();
                rawDeck.LoadHtml(rawDeckContent);

                string rawDeckBody = rawDeck.DocumentNode.SelectSingleNode("//body").InnerText;

                string[] deckItems = rawDeckBody.Replace("\r", "").Split("\n").Select(i => i.Trim()).Where(i => i.Length > 0).ToArray();

                List<DeckItem> mainBoard = new List<DeckItem>();
                List<DeckItem> sideBoard = new List<DeckItem>();
                foreach (string item in deckItems)
                {
                    if (item.StartsWith("[")) continue;
                    if (item.StartsWith("Title")) continue;
                    if (item.StartsWith("-")) continue;

                    bool isSideboard = item.StartsWith("*");

                    int splitPosition = item.IndexOf(" ");
                    string cardCount = item.Substring(0, splitPosition).TrimStart('*');
                    string cardName = NormalizeCardName(item.Substring(splitPosition, item.Length - splitPosition));

                    DeckItem deckItem = new DeckItem()
                    {
                        Count = Int32.Parse(cardCount),
                        CardName = cardName
                    };

                    if (isSideboard) sideBoard.Add(deckItem);
                    else mainBoard.Add(deckItem);
                }

                string playerName = $"{lastName}, {firstName}";
                string rank = "-";

                List<string> possiblePlayerNames = new List<string>() { $"{lastName}, {firstName}" };
                if (lastName.Contains("-"))
                {
                    possiblePlayerNames.AddRange(lastName.Split("-").Select(l => $"{l.Trim()}, {firstName}").ToArray());
                }

                foreach (string possiblePlayerName in possiblePlayerNames)
                {
                    if (standings.ContainsKey(possiblePlayerName))
                    {
                        playerName = standings[possiblePlayerName].Player;
                        rank = standings[possiblePlayerName].Rank.ToString();
                        if (rank == "1") rank += "st";
                        else if (rank == "2") rank += "nd";
                        else rank += "rd";
                    }
                }

                var deck = new Deck()
                {
                    AnchorUri = new Uri(deckUrl),
                    Date = eventDate,
                    Player = playerName,
                    Result = rank,
                    Mainboard = mainBoard.ToArray(),
                    Sideboard = sideBoard.ToArray()
                };

                result.Add(deck);
            }

            return result.OrderBy(d => standings.ContainsKey(d.Player) ? standings[d.Player].Rank : Int32.MaxValue).ToList();
        }

        private static string NormalizeCardName(string cardName)
        {
            #region Split card list
            string[] splitCards = new string[]
            {
                "Alive // Well",
                "Appeal // Authority",
                "Armed // Dangerous",
                "Assault // Battery",
                "Assure // Assemble",
                "Beck // Call",
                "Bedeck // Bedazzle",
                "Boom // Bust",
                "Bound // Determined",
                "Breaking // Entering",
                "Carnival // Carnage",
                "Catch // Release",
                "Claim // Fame",
                "Collision // Colossus",
                "Commit // Memory",
                "Connive // Concoct",
                "Consecrate // Consume",
                "Consign // Oblivion",
                "Crime // Punishment",
                "Cut // Ribbons",
                "Dead // Gone",
                "Depose // Deploy",
                "Destined // Lead",
                "Discovery // Dispersal",
                "Down // Dirty",
                "Driven // Despair",
                "Dusk // Dawn",
                "Expansion // Explosion",
                "Failure // Comply",
                "Far // Away",
                "Farm // Market",
                "Fast // Furious",
                "Find // Finality",
                "Fire // Ice",
                "Flesh // Blood",
                "Flower // Flourish",
                "Give // Take",
                "Grind // Dust",
                "Heaven // Earth",
                "Hide // Seek",
                "Hit // Run",
                "Illusion // Reality",
                "Incubation // Incongruity",
                "Insult // Injury",
                "Integrity // Intervention",
                "Invert // Invent",
                "Leave // Chance",
                "Life // Death",
                "Mouth // Feed",
                "Never // Return",
                "Night // Day",
                "Odds // Ends",
                "Onward // Victory",
                "Order // Chaos",
                "Pain // Suffering",
                "Prepare // Fight",
                "Profit // Loss",
                "Protect // Serve",
                "Pure // Simple",
                "Rags // Riches",
                "Ready // Willing",
                "Reason // Believe",
                "Reduce // Rubble",
                "Refuse // Cooperate",
                "Repudiate // Replicate",
                "Research // Development",
                "Response // Resurgence",
                "Revival // Revenge",
                "Rise // Fall",
                "Road // Ruin",
                "Rough // Tumble",
                "Said // Done",
                "Spite // Malice",
                "Spring // Mind",
                "Stand // Deliver",
                "Start // Finish",
                "Status // Statue",
                "Struggle // Survive",
                "Supply // Demand",
                "Thrash // Threat",
                "Toil // Trouble",
                "Trial // Error",
                "Turn // Burn",
                "Warrant // Warden",
                "Wax // Wane",
                "Wear // Tear"
            };
            #endregion

            cardName = HttpUtility.HtmlDecode(cardName).Trim();

            if (cardName.Contains("//"))
            {
                if (!splitCards.Contains(cardName))
                {
                    cardName = cardName.Split("//").First().Trim();
                }
            }

            return cardName;
        }
    }
}