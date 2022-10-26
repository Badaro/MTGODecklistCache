using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Mtgo;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    public class KnownErrorTests
    {
        [Test]
        public void ShouldConsiderBracketWhenOrderingChallenges()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2022-10-2312488075")
            }).Decks
                .Select(d => d.Player)
                .Should().ContainInOrder(new string[]
                {
                    "Baku_91",
                    "DNSolver",
                    "Oceansoul92",
                    "Didackith",
                    "Eureka22422",
                    "Iwouldliketorespond",
                    "Butakov",
                    "Ozymandias17",
                    "ThomasH",
                    "wonderPreaux",
                    "Cosme_Fulanito",
                    "Peppe",
                    "MarioBBrega",
                    "Testacular",
                    "MoMo321",
                    "fj_rodman",
                    "Bryzem1",
                    "HJ_Kaiser",
                    "Fuz65",
                    "Eonwe7",
                    "SoulStrong",
                    "ScreenwriterNY",
                    "SuperCow12653",
                    "Falco_Lombardi",
                    "discoverN",
                    "CliffBoyardee",
                    "CrusherBotBG",
                    "unluckymonkey",
                    "Granham",
                    "otakkun",
                    "B-Carp",
                    "MaxMagicer",
                });
        }

        [Test]
        public void ShouldIgnoreDuplicateLists()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-challenge-2022-10-2212488066")
            }).Decks
            .Select(d => d.Player)
            .Where(p => p == "leandru")
            .Should().HaveCount(1);
        }


        [Test]
        public void ShouldIncludeSplitCards()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-challenge-2022-10-2312488073")
            }).Decks
            .Where(d => d.Player == "HamburgerJung")
            .SelectMany(d => d.Mainboard)
            .Where(c => c.CardName == "Fire // Ice")
            .Should()
            .HaveCount(1);
        }
    }
}