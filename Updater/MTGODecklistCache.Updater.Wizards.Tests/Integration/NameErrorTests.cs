using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Wizards;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    public class NameErrorTests
    {
        [Test]
        public void ShouldFixNameForLurrusofTheDreamDen()
        {
            // Without the dash on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-04-30")).Decks
                .First(d => d.Player == "PietroSas")
                .Sideboard
                .First(c => c.CardName.StartsWith("Lurrus")).CardName
                .Should().Be("Lurrus of the Dream-Den");
        }

        [Test]
        public void ShouldFixNameForGhazbanOgre()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/pauper-event-2014-06-21")).Decks
                .First(d => d.Player == "Lincao")
                .Sideboard
                .First(c => c.CardName.StartsWith("Ghaz")).CardName
                .Should().Be("Ghazbán Ogre");
        }

        [Test]
        public void ShouldFixNameForLimDulsVault()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-daily-2014-06-28")).Decks
                .First(d => d.Player == "ecobaronen")
                .Mainboard
                .First(c => c.CardName.StartsWith("Lim-D")).CardName
                .Should().Be("Lim-Dûl's Vault");

            // Broken on Wizard's site in a different way
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-league-2020-06-27")).Decks
                .First(d => d.Player == "sawatarix")
                .Mainboard
                .First(c => c.CardName.StartsWith("Lim-D")).CardName
                .Should().Be("Lim-Dûl's Vault");

            // Broken on Wizard's site in a third different way
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-challenge-2020-08-31")).Decks
                .First(d => d.Player == "twinlesstwin")
                .Mainboard
                .First(c => c.CardName.StartsWith("Lim-D")).CardName
                .Should().Be("Lim-Dûl's Vault");
        }

        [Test]
        public void ShouldFixNameForKongmingSleepingDragon()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/sealed-vma-block-champ-qual-2014-08-15-0")).Decks
                .First(d => d.Player == "Smang")
                .Mainboard
                .First(c => c.CardName.StartsWith("Kongming")).CardName
                .Should().Be("Kongming, \"Sleeping Dragon\"");
        }

        [Test]
        public void ShouldFixNameForSeance()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-daily-2015-01-26-0")).Decks
                .First(d => d.Player == "RaptureReady")
                .Mainboard
                .First(c => c.CardName.EndsWith("ance")).CardName
                .Should().Be("Séance");
        }

        [Test]
        public void ShouldFixNameForAetherVial()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-constructed-league-2016-04-30")).Decks
                .First(d => d.Player == "Stuhl")
                .Mainboard
                .First(c => c.CardName.EndsWith("Vial")).CardName
                .Should().Be("Aether Vial");
        }

        [Test]
        public void ShouldFixNameForGhirapurAetherGrid()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-constructed-league-2016-04-30")).Decks
                .First(d => d.Player == "POC")
                .Sideboard
                .First(c => c.CardName.EndsWith("Grid")).CardName
                .Should().Be("Ghirapur Aether Grid");
        }

        [Test]
        public void ShouldFixNameForUnravelTheAether()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/competitive-modern-constructed-league-2019-05-28")).Decks
                .First(d => d.Player == "DreadedDead")
                .Sideboard
                .First(c => c.CardName.StartsWith("Unravel")).CardName
                .Should().Be("Unravel the Aether");
        }

        [Test]
        public void ShouldFixNameForTrinisphere()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-ptq-2019-09-09")).Decks
                .First(d => d.Player == "The french goblin")
                .Sideboard
                .First(c => c.CardName.StartsWith("Trini")).CardName
                .Should().Be("Trinisphere");
        }

        [Test]
        public void ShouldFixNameForExpansionExplosion()
        {
            // Broken on Wizard's site
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/pioneer-challenge-2020-09-20")).Decks
                .First(d => d.Player == "Capriccioso")
                .Mainboard
                .First(c => c.CardName.StartsWith("Expan")).CardName
                .Should().Be("Expansion // Explosion");
        }

        [Test]
        public void ShouldParseCorrectlyJotunGrunt()
        {
            // Code was not parsing this correctly in the past
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-daily-2014-07-07")).Decks
                .First(d => d.Player == "senrabselim")
                .Mainboard
                .First(c => c.CardName.EndsWith("Grunt")).CardName
                .Should().Be("Jötun Grunt");
        }

        [Test]
        public void ShouldParseCorrectlyLimDulsVault()
        {
            // Code was not parsing this correctly in the past
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-league-2020-06-06")).Decks
                .First(d => d.Player == "ryo_sll")
                .Mainboard
                .First(c => c.CardName.EndsWith("Vault")).CardName
                .Should().Be("Lim-Dûl's Vault");
        }
    }
}