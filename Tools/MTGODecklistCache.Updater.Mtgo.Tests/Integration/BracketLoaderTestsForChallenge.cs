using MTGODecklistCache.Updater.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    class BracketLoaderTestsForChallenge : BracketLoaderTests
    {
        public BracketLoaderTestsForChallenge()
        {
        }

        protected override Round[] GetBracket()
        {
            List<Round> bracket = new List<Round>();
            bracket.Add(new Round()
            {
                RoundName = "Quarterfinals",
                Matches = new RoundItem[]
                {
                    new RoundItem(){ Player1 = "Baku_91",     Player2 = "Ozymandias17",         Result= "2-1-0" },
                    new RoundItem(){ Player1 = "Didackith",   Player2 = "Eureka22422",          Result= "2-1-0" },
                    new RoundItem(){ Player1 = "DNSolver",    Player2 = "Iwouldliketorespond",  Result= "2-1-0" },
                    new RoundItem(){ Player1 = "Oceansoul92", Player2 = "Butakov",              Result= "2-1-0" }
                }
            });
            bracket.Add(new Round()
            {
                RoundName = "Semifinals",
                Matches = new RoundItem[]
                {
                    new RoundItem(){ Player1 = "Baku_91",  Player2 = "Didackith",   Result= "2-0-0" },
                    new RoundItem(){ Player1 = "DNSolver", Player2 = "Oceansoul92", Result= "2-0-0" }
                }
            });
            bracket.Add(new Round()
            {
                RoundName = "Finals",
                Matches = new RoundItem[]
                {
                    new RoundItem() { Player1 = "Baku_91", Player2 = "DNSolver", Result = "2-1-0" }
                }
            });
            return bracket.ToArray();
        }

        protected override Uri GetEventUri()
        {
            return new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2022-10-2312488075");
        }
    }
}
