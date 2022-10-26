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
    public class NameErrorTests
    {
        [Test]
        public void ShouldFixNameForAetherVial()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2022-10-2512488091")
            }).Decks
                .First(d => d.Player == "mashmalovsky")
                .Mainboard
                .First(c => c.CardName.EndsWith("Vial")).CardName
                .Should().Be("Aether Vial");
        }
    }
}