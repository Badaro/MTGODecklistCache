using MTGODecklistCache.Updater.Model;
using System;
using System.Web;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using MTGODecklistCache.Updater.Tools;
using System.Collections.Generic;
using System.Globalization;

namespace MTGODecklistCache.Updater.MoxField
{
    public static class TournamentLoader
    {
        static string _apiUrl = "https://api.moxfield.com/v2/decks/all/{deckId}";

        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            var decks = ParseDecks(tournament.Uri.ToString(), tournament.Date);

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks.Select(d => d.Item1).ToArray(),
                Standings = decks.Select(d => d.Item2).ToArray()
            };
        }

        public static Tuple<Deck, Standing>[] ParseDecks(string tournamentUrl, DateTime tournamentDate)
        {
            List<Tuple<Deck, Standing>> decks = new List<Tuple<Deck, Standing>>();
            string[] csv = new WebClient().DownloadString(tournamentUrl).Replace("\r", "").Split("\n").Skip(1).ToArray();

            int rank = 1;
            foreach (string line in csv)
            {
                string[] cells = line.Split(",");
                string player = cells[0];
                string points = cells[1];
                string result = cells[2];
                string omwp = cells[3];
                string gwp = cells[4];
                string ogwp = cells[5];
                string deckUrl = cells[6];

                // Normalization
                double parsedOmw = FormatPercent(omwp);
                double parsedGp = FormatPercent(gwp);
                double parsedOgp = FormatPercent(ogwp);
                result = result.Replace(" ", "");

                string parsedRank = rank.ToString();
                if (parsedRank == "1") parsedRank += "st Place";
                else if (parsedRank == "2") parsedRank += "nd Place";
                else if (parsedRank == "3") parsedRank += "rd Place";
                else parsedRank += "th Place";

                if (!String.IsNullOrEmpty(deckUrl))
                {
                    string deckId = deckUrl.Split("/").Last();
                    string deckApiUrl = _apiUrl.Replace("{deckId}", deckId);

                    string json = new WebClient().DownloadString(deckApiUrl);

                    dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

                    List<DeckItem> mainBoard = new List<DeckItem>();
                    foreach (var card in data.mainboard)
                    {
                        int count = card.First.quantity;
                        string name = card.First.card.name;
                        name = CardNameNormalizer.Normalize(name);

                        mainBoard.Add(new DeckItem()
                        {
                            CardName = name,
                            Count = count
                        });
                    }

                    List<DeckItem> sideBoard = new List<DeckItem>();
                    foreach (var card in data.sideboard)
                    {
                        int count = card.First.quantity;
                        string name = card.First.card.name;
                        name = CardNameNormalizer.Normalize(name);

                        sideBoard.Add(new DeckItem()
                        {
                            CardName = name,
                            Count = count
                        });
                    }

                    Deck deck = new Deck()
                    {
                        AnchorUri = new Uri(deckUrl),
                        Result = result,
                        Date = tournamentDate,
                        Player = player,
                        Mainboard = mainBoard.ToArray(),
                        Sideboard = sideBoard.ToArray()
                    };

                    Standing standing = new Standing()
                    {
                        Player = player,
                        Points = Convert.ToInt32(points),
                        Rank = rank++,
                        OMWP = FormatPercent(omwp),
                        GWP = FormatPercent(gwp),
                        OGWP = FormatPercent(ogwp),
                    };

                    decks.Add(new Tuple<Deck, Standing>(deck, standing));
                }
            }

            return decks.ToArray();
        }

        static double FormatPercent(string raw)
        {
            if (raw.Contains("%"))
            {
                raw = raw.Replace("%", "");
                double result = Double.Parse(raw, CultureInfo.InvariantCulture);
                return result / 100d;
            }
            else
            {
                return Double.Parse(raw, CultureInfo.InvariantCulture);
            }
        }
    }
}