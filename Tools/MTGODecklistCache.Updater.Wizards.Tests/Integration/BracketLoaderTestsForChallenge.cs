using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    class BracketLoaderTestsForChallenge : BracketLoaderTests
    {
        public BracketLoaderTestsForChallenge()
        {
        }

        protected override Bracket GetBracket()
        {
            return new Bracket()
            {
                Quarterfinals = new BracketItem[]
                {
                    new BracketItem(){ Player1 = "TSPJendrek", Player2 = "AstralPlane",  Result= "2-0" },
                    new BracketItem(){ Player1 = "JB2002",     Player2 = "signblindman", Result= "2-1" },
                    new BracketItem(){ Player1 = "ZYURYO",     Player2 = "Blitzlion27",  Result= "2-0" },
                    new BracketItem(){ Player1 = "Yanti",      Player2 = "SvenSvenSven", Result= "2-1" }
                },
                Semifinals = new BracketItem[]
                {
                    new BracketItem(){ Player1 = "TSPJendrek", Player2 = "JB2002", Result= "2-1" },
                    new BracketItem(){ Player1 = "ZYURYO",     Player2 = "Yanti",  Result= "2-0" }
                },
                Finals = new BracketItem() { Player1 = "TSPJendrek", Player2 = "ZYURYO", Result = "2-0" }
            };
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-08");
        }
    }
}
