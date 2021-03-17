using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MTGODecklistCache.Updater.MtgGoldfish
{
    public static class TournamentLoader
    {
        static string _deckRootPage = "https://www.mtggoldfish.com/deck/{deckId}";

        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            var decks = ParseDecks(tournament.Uri.ToString());

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks.Select(d => d.Item1).ToArray(),
                Standings = decks.Where(d => d.Item2 != null).Select(d => d.Item2).ToArray()
            };
        }

        private static Tuple<Deck, Standing>[] ParseDecks(string url)
        {
            List<Tuple<Deck, Standing>> result = new List<Tuple<Deck, Standing>>();

            string pageContent = new WebClient().DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var deckNodes = doc.DocumentNode.SelectNodes("//tr[@class='tournament-decklist']");
            if (deckNodes == null) return new Tuple<Deck, Standing>[0];

            int position = 1;
            foreach (var deckNode in deckNodes)
            {
                var deckId = deckNode.Attributes["data-deckid"].Value;

                string deckPage = _deckRootPage.Replace("{deckId}", deckId);
                string deckPageContent = new WebClient().DownloadString(deckPage);

                HtmlDocument deckDoc = new HtmlDocument();
                deckDoc.LoadHtml(deckPageContent);

                var cardList = WebUtility.HtmlDecode(
                    deckDoc.DocumentNode
                    .SelectSingleNode("//input[@id='deck_input_deck']")
                    .Attributes["value"]
                    .Value).Split("\n", StringSplitOptions.RemoveEmptyEntries).ToArray();

                List<DeckItem> mainBoard = new List<DeckItem>();
                List<DeckItem> sideBoard = new List<DeckItem>();
                bool insideSideboard = false;

                foreach (var card in cardList)
                {
                    if (card == "sideboard")
                    {
                        insideSideboard = true;
                    }
                    else
                    {
                        int splitPosition = card.IndexOf(" ");
                        int count = Convert.ToInt32(new String(card.Take(splitPosition).ToArray()));
                        string name = new String(card.Skip(splitPosition + 1).ToArray());

                        if (insideSideboard) sideBoard.Add(new DeckItem() { CardName = name, Count = count });
                        else mainBoard.Add(new DeckItem() { CardName = name, Count = count });
                    }
                }

                string playerName = deckDoc.DocumentNode
                    .SelectSingleNode("//span[@class='author']").InnerText.Replace("by ", "");

                string[] deckData = deckDoc.DocumentNode
                    .SelectSingleNode("//p[@class='deck-container-information']").InnerText.Split("\n", StringSplitOptions.RemoveEmptyEntries);

                string playerResult = position.ToString();
                if (position == 1) playerResult += "st Place";
                if (position == 2) playerResult += "nd Place";
                if (position == 3) playerResult += "rd Place";
                if (position > 3) playerResult += "th Place";

                string playerScore = deckData.First(d => d.StartsWith("Event")).Split(", ").Last().TrimStart('(').TrimEnd(')');

                int wins = 0;
                if (Regex.IsMatch(playerScore, "\\d+-\\d+"))
                {
                    wins = Convert.ToInt32(playerScore.Split("-").First());
                }

                Standing standing = new Standing()
                {
                    Player = playerName,
                    Rank = position,
                    Points = wins * 3
                };

                Deck deck = new Deck()
                {
                    AnchorUri = new Uri(deckPage),
                    Date = null,
                    Mainboard = mainBoard.ToArray(),
                    Sideboard = sideBoard.ToArray(),
                    Player = playerName,
                    Result = playerResult
                };

                result.Add(new Tuple<Deck, Standing>(deck, standing));
                position++;
            }

            return result.ToArray();
        }
    }
}