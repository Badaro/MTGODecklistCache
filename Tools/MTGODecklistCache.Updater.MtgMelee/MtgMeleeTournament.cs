using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.MtgMelee
{
    public class MtgMeleeTournament : Tournament
    {
        public int? DeckOffset { get; set; }
        public int? ExpectedDecks { get; set; }
        public int? PhaseOffset { get; set; }
        public MtgMeleeMissingDeckBehavior FixBehavior { get; set; }
    }

    public enum MtgMeleeMissingDeckBehavior
    {
        Skip,
        UseLast
    }
}
