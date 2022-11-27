using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class BracketLoaderTestsForLeague : BracketLoaderTests
    {
        public BracketLoaderTestsForLeague()
        {
        }

        protected override Round[] GetBracket()
        {
            return null;
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-league-2020-08-04");
        }
    }
}
