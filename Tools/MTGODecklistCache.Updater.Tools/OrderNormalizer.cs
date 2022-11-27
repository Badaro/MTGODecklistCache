using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTGODecklistCache.Updater.Tools
{
    public static class OrderNormalizer
    {
        public static Deck[] ReorderDecks(Deck[] decks, Standing[] standings, Bracket bracket)
        {
            if (bracket == null) return decks;

            List<Deck> orderedDecks = new List<Deck>();

            int position = 1;
            foreach (var player in GetPlayerOrder(decks, standings, bracket))
            {
                var deck = decks.First(d => d.Player == player);

                string rank = $"{position}th Place";
                if (position == 1) rank = "1st Place";
                if (position == 2) rank = "2nd Place";
                if (position == 3) rank = "3rd Place";
                position++;

                deck.Result = rank;

                orderedDecks.Add(deck);
            }

            return orderedDecks.ToArray();
        }

        private static string[] GetPlayerOrder(Deck[] decks, Standing[] standings, Bracket bracket)
        {
            List<string> result = new List<string>();

            foreach (var standing in standings) result.Add(standing.Player);

            if (bracket.Quarterfinals != null) result = PushToTop(result, bracket.Quarterfinals.Select(s => s.Player2).ToList(), standings);
            if (bracket.Semifinals != null) result = PushToTop(result, bracket.Semifinals.Select(s => s.Player2).ToList(), standings);
            if (bracket.Finals != null) result = PushToTop(result, new List<string>() { bracket.Finals.Player2 }, standings);
            if (bracket.Finals != null) result = PushToTop(result, new List<string>() { bracket.Finals.Player1 }, standings);

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
