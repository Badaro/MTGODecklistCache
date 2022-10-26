using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class StandingsLoaderTestsForPrelim : StandingsLoaderTests
    {
        public StandingsLoaderTestsForPrelim()
        {
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02");
        }

        protected override Standing GetFirstStanding()
        {
            return new Standing()
            {
                Rank = 1,
                Player = "Wartico1",
                Points = 15,
                OMWP = 0.6400,
                GWP = 0.7143,
                OGWP = 0.5948
            };

        }

        protected override int GetStandingsCount()
        {
            return 16;
        }
    }
}
