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

        [Test]
        public void ShouldFixNameForJotunGrunt()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-daily-swiss-2015-11-028954004")
            }).Decks
                .First(d => d.Player == "sattonex")
                .Sideboard
                .First(c => c.CardName.EndsWith("Grunt")).CardName
                .Should().Be("Jötun Grunt");
        }

        [Test]
        public void ShouldFixNameForLimDulsVault()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-daily-swiss-2015-11-098986451")
            }).Decks
                .First(d => d.Player == "mastaflyer")
                .Mainboard
                .First(c => c.CardName.EndsWith("Vault")).CardName
                .Should().Be("Lim-Dûl's Vault");
        }

        [Test]
        public void ShouldFixCasingForRainOfTears()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-daily-swiss-2015-11-098986462")
            }).Decks
                .First(d => d.Player == "oRS")
                .Sideboard
                .First(c => c.CardName.EndsWith("Tears")).CardName
                .Should().Be("Rain of Tears");
        }

        [Test]
        public void ShouldFixCasingForAltarOfDementia()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-league-2020-01-18")
            }).Decks
                .First(d => d.Player == "yanmaster")
                .Mainboard
                .First(c => c.CardName.EndsWith("Dementia")).CardName
                .Should().Be("Altar of Dementia");
        }

        [Test]
        public void ShouldIgnoreEmptySpacesOnCardNames()
        {
            TournamentLoader.GetTournamentDetails(new Tournament()
            {
                Uri = new Uri("https://www.mtgo.com/en/mtgo/decklist/strixhaven-championship-limited-qualifier-2021-03-2712277612")
            }).Decks
                .First(d => d.Player == "JPA93")
                .Sideboard
                .First(c => c.CardName.StartsWith("Karfell Kennel")).CardName
                .Should().NotEndWith(" ") ;
        }
    }
}