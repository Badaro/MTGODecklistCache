using CsvHelper;
using HtmlAgilityPack;
using Microsoft.VisualStudio.Services.Common.Internal;
using MTGODecklistParser.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Updater
{
    public static class TournamentDetailsLoader
    {
        static string _deckRootPage = "https://www.mtggoldfish.com/deck/{deckId}";

        public static TournamentDetails GetTournamentDetails(string url)
        {
            var decks = ParseDecks(url);

            return new TournamentDetails()
            {
                Decks = decks
            };
        }

        private static Deck[] ParseDecks(string url)
        {
            List<Deck> result = new List<Deck>();

            string pageContent = new WebClient().DownloadString(url);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(pageContent);

            var deckNodes = doc.DocumentNode.SelectNodes("//tr[@class='tournament-decklist']");
            if (deckNodes == null) return new Deck[0];

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

                string playerResult = deckData.First(d => d.StartsWith("Event")).Split(", ").Last().TrimStart('(').TrimEnd(')');

                result.Add(new Deck()
                {
                    AnchorUri = new Uri(deckPage),
                    Date = null,
                    Mainboard = mainBoard.ToArray(),
                    Sideboard = sideBoard.ToArray(),
                    Player = playerName,
                    Result = playerResult
                });
            }

            return result.ToArray();
        }
    }
}