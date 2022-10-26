using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Mtgo;

namespace MTGODecklistCache.Updater.Mtgo.Tests
{
    public class TournamentLoaderCrossMonthTests
    {
        private Tournament[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentList.GetTournaments(new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc));
        }

        [Test]
        public void TournamentCountIsCorrect()
        {
            _testData.Length.Should().Be(13);
        }

        [Test]
        public void TournamentDataIsCorrect()
        {
            _testData.Should().BeEquivalentTo(new Tournament[]
            {
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer League",          Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pioneer-league-2020-06-01"),                  JsonFile="pioneer-league-2020-06-01.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Standard League",         Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/standard-league-2020-06-01"),                 JsonFile="standard-league-2020-06-01.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Preliminary",     Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/vintage-preliminary-2020-06-0112162942"),     JsonFile="vintage-preliminary-2020-06-0112162942.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Modern Super Qualifier",  Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-super-qualifier-2020-06-0112162897"),  JsonFile="modern-super-qualifier-2020-06-0112162897.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Standard Challenge",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/standard-challenge-2020-06-0112162941"),      JsonFile="standard-challenge-2020-06-0112162941.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pauper Challenge",        Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pauper-challenge-2020-06-0112162939"),        JsonFile="pauper-challenge-2020-06-0112162939.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Vintage League",          Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/vintage-league-2020-05-31"),                  JsonFile="vintage-league-2020-05-31.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Challenge",              Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/vintage-challenge-2020-05-3112162931"),       JsonFile="vintage-challenge-2020-05-3112162931.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer Challenge",      Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/pioneer-challenge-2020-05-3112162933"),       JsonFile="pioneer-challenge-2020-05-3112162933.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Modern Challenge",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/modern-challenge-2020-05-3112162936"),        JsonFile="modern-challenge-2020-05-3112162936.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Legacy Challenge",        Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/legacy-challenge-2020-05-3112162938"),        JsonFile="legacy-challenge-2020-05-3112162938.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Challenge",       Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/vintage-challenge-2020-05-3112162935"),       JsonFile="vintage-challenge-2020-05-3112162935.json" },
                new Tournament(){ Date=new DateTime(2020, 05, 31, 00, 00, 00, DateTimeKind.Utc), Name="Limited Super Qualifier", Uri=new Uri("https://www.mtgo.com/en/mtgo/decklist/limited-super-qualifier-2020-05-3112162896"), JsonFile="limited-super-qualifier-2020-05-3112162896.json" },
            });
        }
    }
}