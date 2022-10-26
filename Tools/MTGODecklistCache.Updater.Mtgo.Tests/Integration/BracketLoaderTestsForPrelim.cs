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

        protected override Bracket GetBracket()
        {
            return null;
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02");
        }
    }
}
