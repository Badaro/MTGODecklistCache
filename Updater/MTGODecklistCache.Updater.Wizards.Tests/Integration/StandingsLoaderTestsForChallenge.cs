using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    class StandingsLoaderTestsForChallenge : StandingsLoaderTests
    {
        public StandingsLoaderTestsForChallenge()
        {
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-08");
        }

        protected override Standing GetFirstStanding()
        {
            return new Standing()
            {
                Rank = 1,
                Player = "JB2002",
                Points = 21,
                OMWP = 0.5238,
                GWP = 0.8235,
                OGWP = 0.5104
            };
        }

        protected override int GetStandingsCount()
        {
            return 32;
        }
    }
}
