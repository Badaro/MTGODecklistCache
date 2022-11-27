using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class BracketLoaderTestsForPrelim : BracketLoaderTests
    {
        public BracketLoaderTestsForPrelim()
        {
        }

        protected override Round[] GetBracket()
        {
            return null;
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2022-10-2512488091");
        }
    }
}
