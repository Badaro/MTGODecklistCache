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
    public class ErrorHandlingTests
    {
        [Test]
        public void ShouldNotBreakOnEmptyPage()
        {
            // Broken tournament, should return empty dataset
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-mocs-2019-07-17")).Decks
                .Should().HaveCount(0);
        }

        [Test]
        public void ShouldNotBreakOnEmptyDecks()
        {
            // Broken tournament, should return empty dataset
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/sealed-war-block-mcq-2019-05-10")).Decks
                .ToList()
                .ForEach(d =>
                {
                    d.Mainboard.Should().HaveCount(0);
                    d.Sideboard.Should().HaveCount(0);
                });
        }

        [Test]
        public void ShouldNotBreakOnOutOfStandardUrls()
        {
            TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/content/pauper-league")).Decks
                .Should().HaveCount(20);
        }

        [Test]
        public void ShouldTrimWhiteSpacesFromNames()
        {
            var details = TournamentDetailsLoader.GetTournamentDetails(new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-preliminary-2020-03-13"));

            details
                .Decks
                .First(d => d.Player.StartsWith("victor"))
                .Player
                .Should().Be("victor_fefe");

            details
                .Standings
                .First(d => d.Player.StartsWith("victor"))
                .Player
                .Should().Be("victor_fefe");
        }
    }
}