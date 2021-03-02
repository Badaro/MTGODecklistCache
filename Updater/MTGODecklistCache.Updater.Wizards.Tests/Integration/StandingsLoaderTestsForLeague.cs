using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    class StandingsLoaderTestsForLeague : StandingsLoaderTests
    {
        public StandingsLoaderTestsForLeague()
        {
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-08-04");
        }

        protected override Standing GetFirstStanding()
        {
            return null;
        }

        protected override int GetStandingsCount()
        {
            return 0;
        }
    }
}
