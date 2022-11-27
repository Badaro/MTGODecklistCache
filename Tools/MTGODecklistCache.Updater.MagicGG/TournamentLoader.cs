using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MTGODecklistCache.Updater.MagicGG
{
    public static class TournamentLoader
    {
        static string _marker = "window.__NUXT__";
        static string _deckRootPage = "https://s3-us-west-1.amazonaws.com/hvn-decklist.magic.gg/{deckId}.json";

        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            return new CacheItem()
            {
                Tournament = tournament,
                Decks = ParseDecks(tournament.Uri.ToString())
            };
        }

        private static Deck[] ParseDecks(string url)
        {
            List<Deck> result = new List<Deck>();

            string pageContent = new WebClient().DownloadString(url);

            string[] pageLines = pageContent.Replace("\r", "").Split("\n");
            string scriptLine = pageLines.First(l => l.Contains(_marker));

            string[] deckIds = Regex.Matches(scriptLine, "\"decklistId:.*?\"")
                .Select(m => m.Value)
                .Select(v => v.Replace("\"", "").Replace("decklistId:", ""))
                .ToArray();

            foreach (var deckId in deckIds)
            {
                string deckJsonUrl = _deckRootPage.Replace("{deckId}", deckId);
                string deckJson = new WebClient().DownloadString(deckJsonUrl);

                dynamic deck = JsonConvert.DeserializeObject<dynamic>(deckJson);
                string deckPlayer = deck.playerName;
                string deckDate = deck.date;

                List<DeckItem> deckMainBoard = new List<DeckItem>();
                foreach (var card in deck.mainDeckList)
                {
                    string name = card.name;
                    int count = card.quantity;

                    name = CardNameNormalizer.Normalize(name);

                    deckMainBoard.Add(new DeckItem() { CardName = name, Count = count });
                }

                List<DeckItem> deckSideBoard = new List<DeckItem>();
                foreach (var card in deck.sideBoard)
                {
                    string name = card.name;
                    int count = card.quantity;

                    name = CardNameNormalizer.Normalize(name);

                    deckSideBoard.Add(new DeckItem() { CardName = name, Count = count });
                }

                string[] dateFormats = new string[] { "M/d/yyyy", "MMMM dd, yyyy" };

                DateTime? deckParsedDate = null;
                foreach (string dateFormat in dateFormats)
                {
                    if (DateTime.TryParseExact(deckDate, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime validDate))
                    {
                        deckParsedDate = validDate.ToUniversalTime();
                    }
                }
                if (deckParsedDate == null) throw new Exception($"Error parsing decl date: {deckDate}");

                result.Add(new Deck()
                {
                    AnchorUri = new Uri($"{url}#{deckId.Replace(" ", "%2520")}"),
                    Player = deckPlayer,
                    Date = deckParsedDate,
                    Mainboard = deckMainBoard.ToArray(),
                    Sideboard = deckSideBoard.ToArray()
                });
            }

            return result.ToArray();
        }
    }
}