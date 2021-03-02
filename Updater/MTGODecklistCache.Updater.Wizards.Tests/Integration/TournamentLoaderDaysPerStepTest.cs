using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Wizards;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    public class TournamentLoaderDaysPerStepTest
    {
        private Tournament[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            // Using daysPerStep should still return the same data even if it causes the search to go beyond the start/end date boundaries
            _testData = TournamentList.GetTournaments(new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), null, 7);
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
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer Preliminary",              Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/pioneer-preliminary-2020-06-02") },
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern Super Qualifier",           Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-super-qualifier-2020-06-02") },
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern Preliminary",               Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02") },
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern League",                    Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-06-02") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Sealed IKO Block Super Qualifier", Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/sealed-iko-block-super-qualifier-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Challenge",                Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/vintage-challenge-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Vintage Preliminary",              Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/vintage-preliminary-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Standard Challenge",               Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/standard-challenge-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pauper Challenge",                 Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/pauper-challenge-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Modern Challenge",                 Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Legacy Challenge",                 Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/legacy-challenge-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Standard League",                  Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/standard-league-2020-06-01") },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Pioneer League",                   Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/pioneer-league-2020-06-01") }
            });
        }
    }
}