using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.PlayerLink
{
    public class PlayerLinkTournament : Tournament
    {
        public int? DeckOffset { get; set; }
        public int? ExpectedDecks { get; set; }
        public PlayerLinkNameFix[] PlayerNameFixes { get; set; }
    }

    public class PlayerLinkNameFix
    {
        public string OldFirstName { get; set; }
        public string OldLastName { get; set; }
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
    }
}
