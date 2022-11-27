using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.ManaTraders;
using System.Collections.Generic;
using System.IO;

namespace MTGODecklistCache.Updater.ManaTraders.Tests
{
    public class TournamentLoaderTests
    {
        private Tournament[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentList.GetTournaments();
        }

        [Test]
        public void TournamentCountIsCorrect()
        {
            _testData.Length.Should().BeGreaterThan(0);
        }

        [Test]
        public void TournamentDataIsCorrect()
        {
            List<string> validYears = new List<string>();
            for (int i = 2020; i <= DateTime.Now.Year; i++) validYears.Add(i.ToString());

            foreach(var tournament in _testData.ToList())
            {
                tournament.Uri.ToString().Should().Contain("https://www.manatraders.com/tournaments/");
                tournament.Name.Should().ContainAny("Standard", "Modern", "Pioneer", "Vintage", "Pauper", "Legacy");
                tournament.Name.Should().ContainAny(System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.MonthNames);
                tournament.Name.Should().ContainAny(validYears);
                Path.GetFileNameWithoutExtension(tournament.JsonFile).Should().EndWith(tournament.Date.ToString("yyyy-MM-dd"));
                Path.GetExtension(tournament.JsonFile).Should().Be(".json");
            }
        }
    }
}