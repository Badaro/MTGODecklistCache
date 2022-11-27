using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGODecklistCache.Updater.Tools
{
    public static class OrderNormalizer
    {
        public static Deck[] ReorderDecks(Deck[] decks, Standing[] standings, Round[] bracketRounds)
        {
            List<Deck> orderedDecks = new List<Deck>();

            var playerOrder = GetPlayerOrder(decks, standings, bracketRounds);

            int position = 1;
            foreach (var player in playerOrder)
            {
                var deck = decks.FirstOrDefault(d => d.Player == player);
                if (deck == null)
                {
                    position++;
                    continue;
                }
                else
                {
                    string rank = $"{position}th Place";
                    if (position == 1) rank = "1st Place";
                    if (position == 2) rank = "2nd Place";
                    if (position == 3) rank = "3rd Place";
                    position++;

                    deck.Result = rank;
                }

                orderedDecks.Add(deck);
            }

            return orderedDecks.ToArray();
        }

        private static string[] GetPlayerOrder(Deck[] decks, Standing[] standings, Round[] bracketRounds)
        {
            List<string> result = new List<string>();

            for (int i = 1; i <= standings.Last().Rank; i++)
            {
                string playerName = standings.FirstOrDefault(s => s.Rank == i)?.Player;
                if (playerName == null) result.Add("-");
                else result.Add(playerName);
            }

            foreach (var bracketRound in bracketRounds)
            {
                result = PushToTop(result, bracketRound.Matches.Select(s => s.Player2).ToList(), standings);
                result = PushToTop(result, bracketRound.Matches.Select(s => s.Player1).ToList(), standings);
            }

            // In case some player is missing from Standings
            foreach (var deck in decks) if (!result.Contains(deck.Player)) result.Add(deck.Player);

            return result.Distinct().ToArray();
        }

        private static List<string> PushToTop(List<string> players, List<string> pushedPlayers, Standing[] standings)
        {
            Dictionary<string, int> playerRanks = new Dictionary<string, int>();
            foreach (var player in pushedPlayers.Where(p => standings.Any(s => s.Player == p)))
            {
                var rank = standings.First(s => s.Player == player).Rank;
                playerRanks.Add(player, rank);
            }

            string[] remainingPlayers = players.Where(c => !pushedPlayers.Contains(c)).ToArray();

            List<string> result = new List<string>();
            foreach (var player in playerRanks.OrderBy(p => p.Value)) result.Add(player.Key);
            foreach (var player in remainingPlayers) result.Add(player);

            return result;
        }
    }
}
