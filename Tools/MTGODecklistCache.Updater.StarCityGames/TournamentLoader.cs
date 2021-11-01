using HtmlAgilityPack;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace MTGODecklistCache.Updater.StarCityGames
{
    public static class TournamentLoader
    {
        public static CacheItem GetTournamentDetails(Tournament tournament)
        {
            var decks = ParseDecks(tournament.Uri.ToString());

            return new CacheItem()
            {
                Tournament = tournament,
                Decks = decks.ToArray()
            };
        }

        private static Deck[] ParseDecks(string url)
        {
            List<Deck> result = new List<Deck>();

            string pageContent = new WebClient().DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var playerRows = doc.DocumentNode.SelectNodes("//table").First().SelectNodes("tr").Skip(5).SkipLast(1).ToArray();

            foreach (var playerRow in playerRows)
            {
                var playerColumns = playerRow.SelectNodes("td").ToArray();

                string deckUrl = playerColumns[0].SelectSingleNode("a").GetAttributeValue("href", null);
                string playerResult = playerColumns[1].SelectSingleNode("span").InnerText + " Place";
                string playerName = playerColumns[2].InnerText;
                string deckDate = playerColumns[5].InnerText;


                string deckPageContent = new WebClient().DownloadString(deckUrl);
                HtmlDocument deckPageDoc = new HtmlDocument();
                deckPageDoc.LoadHtml(deckPageContent);

                List<DeckItem> mainBoard = new List<DeckItem>();
                List<DeckItem> sideBoard = new List<DeckItem>();

                var deckRoot = deckPageDoc.DocumentNode.SelectNodes("//div[@class='deck_card_wrapper']").First();

                foreach (var mainBoardNode in deckRoot.SelectNodes("div/ul/li"))
                {
                    mainBoard.Add(ParseCardRow(mainBoardNode));
                }
                foreach (var sideBoardNode in deckRoot.SelectNodes("div/div/ul/li"))
                {
                    sideBoard.Add(ParseCardRow(sideBoardNode));
                }

                result.Add(new Deck()
                {
                    AnchorUri = new Uri(deckUrl),
                    Date = DateTime.Parse(deckDate),
                    Mainboard = mainBoard.ToArray(),
                    Sideboard = sideBoard.ToArray(),
                    Player = playerName,
                    Result = playerResult
                });
            }

            return result.ToArray();
        }

        private static DeckItem ParseCardRow(HtmlNode cardNode)
        {
            string card = cardNode.InnerText;
            int splitPosition = card.IndexOf(" ");
            int count = Convert.ToInt32(new String(card.Take(splitPosition).ToArray()));
            string name = new String(card.Skip(splitPosition + 1).ToArray());
            name = CardNameNormalizer.Normalize(name);

            return new DeckItem() { CardName = name, Count = count };
        }
    }
}
