using System;
using System.Web;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using MTGODecklistCache.Updater.Tools;
using System.Collections.Generic;
using System.Globalization;
using MTGODecklistCache.Updater.Model;
using System.Text.RegularExpressions;

namespace MTGODecklistCache.Updater.PlainText
{
    public static class TournamentLoader
    {
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

            string[] rows = new WebClient().DownloadString(tournamentUrl).Replace("\r", "").Split("\n").Where(r => !String.IsNullOrWhiteSpace(r)).ToArray();

            Deck deck = null;
            Standing standing = null;

            int rounds = 0;
            string player = String.Empty;
            int points = 0;
            int rank = 0;
            bool inSideboard = false;
            List<DeckItem> mainBoard = new List<DeckItem>();
            List<DeckItem> sideBoard = new List<DeckItem>();
            foreach (string row in rows)
            {
                if (row.StartsWith("Rounds:"))
                {
                    rounds = Int32.Parse(row.Split(":").Last().Trim());
                    continue;
                }
                if (row.StartsWith("Sideboard:"))
                {
                    inSideboard = true;
                    continue;
                }
                if (Regex.IsMatch(row, @"^\d+\s.*"))
                {
                    int splitPos = row.IndexOf(" ");
                    int count = Int32.Parse(row.Substring(0, splitPos));
                    string cardName = row.Substring(splitPos).Trim();
                    cardName = CardNameNormalizer.Normalize(cardName);

                    if (inSideboard) sideBoard.Add(new DeckItem() { CardName = cardName, Count = count });
                    else mainBoard.Add(new DeckItem() { CardName = cardName, Count = count });
                }
                else
                {
                    if (player != String.Empty)
                    {
                        deck = new Deck()
                        {
                            AnchorUri = null,
                            Date = tournamentDate,
                            Player = player,
                            Result = $"{points} Points",
                            Mainboard = mainBoard.ToArray(),
                            Sideboard = sideBoard.ToArray()
                        };

                        standing = new Standing()
                        {
                            Player = player,
                            Rank = rank,
                            Points = points
                        };
                        decks.Add(new Tuple<Deck, Standing>(deck, standing));
                    }

                    player = row.Split(":").First().Trim();
                    points = Int32.Parse(row.Split(":").Last().Replace("Points", "").Trim());
                    mainBoard = new List<DeckItem>();
                    sideBoard = new List<DeckItem>();
                    inSideboard = false;
                    rank++;
                }
            }

            deck = new Deck()
            {
                AnchorUri = null,
                Date = tournamentDate,
                Player = player,
                Result = $"{points} Points",
                Mainboard = mainBoard.ToArray(),
                Sideboard = sideBoard.ToArray()
            };

            standing = new Standing()
            {
                Player = player,
                Rank = rank,
                Points = points
            };
            decks.Add(new Tuple<Deck, Standing>(deck, standing));

            return decks.ToArray();
        }
    }
}