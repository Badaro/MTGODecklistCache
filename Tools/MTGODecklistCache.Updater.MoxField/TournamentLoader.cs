using MTGODecklistCache.Updater.Model;
using System;
using System.Web;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using MTGODecklistCache.Updater.Tools;
using System.Collections.Generic;

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
                Decks = decks.ToArray()
            };
        }

        public static Deck[] ParseDecks(string tournamentUrl, DateTime tournamentDate)
        {
            List<Deck> result = new List<Deck>();
            string[] csv = new WebClient().DownloadString(tournamentUrl).Replace("\r", "").Split("\n");

            foreach (string line in csv)
            {
                string[] cells = line.Split(",");
                string lastName = cells[0];
                string firstName = cells[1];
                string deckUrl = cells[2];

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

                    result.Add(new Deck()
                    {
                        AnchorUri = new Uri(deckUrl),
                        Result = String.Empty,
                        Date = tournamentDate,
                        Player = $"{lastName},{firstName}",
                        Mainboard = mainBoard.ToArray(),
                        Sideboard = sideBoard.ToArray(),
                    });
                }
            }

            return result.ToArray();
        }
    }
}