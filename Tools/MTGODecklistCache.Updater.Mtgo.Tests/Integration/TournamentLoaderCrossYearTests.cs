using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Mtgo;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    public class TournamentLoaderCrossYearTests
    {
        private Tournament[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentList.GetTournaments(new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc));
        }

        [Test]
        public void TournamentCountIsCorrect()
        {
            _testData.Length.Should().Be(15);
        }

        [Test]
        public void TournamentDataIsCorrect()
        {
            _testData.Should().BeEquivalentTo(new Tournament[]
            {
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Legacy League",            Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-league-2022-01-01"),                   JsonFile="legacy-league-2022-01-01.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer Preliminary",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pioneer-preliminary-2022-01-0112367808"),     JsonFile="pioneer-preliminary-2022-01-0112367808.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Modern Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2022-01-0112367807"),      JsonFile="modern-preliminary-2022-01-0112367807.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Preliminary",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/vintage-preliminary-2022-01-0112367804"),     JsonFile="vintage-preliminary-2022-01-0112367804.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pauper Challenge",         Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pauper-challenge-2022-01-0112367810"),        JsonFile="pauper-challenge-2022-01-0112367810.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer Preliminary",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pioneer-preliminary-2022-01-0112367805"),     JsonFile="pioneer-preliminary-2022-01-0112367805.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Standard Challenge",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/standard-challenge-2022-01-0112367812"),      JsonFile="standard-challenge-2022-01-0112367812.json" },
                new Tournament(){ Date=new DateTime(2022, 01, 01, 00, 00, 00, DateTimeKind.Utc), Name="Convention Championship",  Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/convention-championship-2022-01-0112367722"), JsonFile="convention-championship-2022-01-0112367722.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Modern League",            Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-league-2021-12-31"),                   JsonFile="modern-league-2021-12-31.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer Preliminary",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pioneer-preliminary-2021-12-3112367798"),     JsonFile="pioneer-preliminary-2021-12-3112367798.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Legacy Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-preliminary-2021-12-3112367795"),      JsonFile="legacy-preliminary-2021-12-3112367795.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Legacy Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-preliminary-2021-12-3112367802"),      JsonFile="legacy-preliminary-2021-12-3112367802.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Modern Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2021-12-3112367793"),      JsonFile="modern-preliminary-2021-12-3112367793.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Legacy Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-preliminary-2021-12-3112367799"),      JsonFile="legacy-preliminary-2021-12-3112367799.json" },
                new Tournament(){ Date=new DateTime(2021, 12, 31, 00, 00, 00, DateTimeKind.Utc), Name="Modern Preliminary",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-preliminary-2021-12-3112367801"),      JsonFile="modern-preliminary-2021-12-3112367801.json" },
            });
        }
    }
}