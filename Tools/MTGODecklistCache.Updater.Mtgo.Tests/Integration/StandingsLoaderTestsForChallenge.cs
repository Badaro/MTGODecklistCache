using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class StandingsLoaderTestsForChallenge : StandingsLoaderTests
    {
        public StandingsLoaderTestsForChallenge()
        {
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2022-10-2312488075");
        }

        protected override Standing GetFirstStanding()
        {
            return new Standing()
            {
                Rank = 1,
                Player = "Baku_91",
                Points = 18,
                OMWP = 0.6735,
                GWP = 0.6667,
                OGWP = 0.6047
            };
        }

        protected override int GetStandingsCount()
        {
            return 32;
        }
    }
}
