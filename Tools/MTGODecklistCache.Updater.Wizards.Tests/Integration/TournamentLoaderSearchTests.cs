using NUnit.Framework;
using FluentAssertions;
using System;
using System.Linq;
using MTGODecklistCache.Updater.Model;
using MTGODecklistCache.Updater.Wizards;

namespace MTGODecklistCache.Updater.Wizards.Tests
{
    public class TournamentLoaderSearchTests
    {
        private Tournament[] _testData = null;

        [OneTimeSetUp]
        public void GetTestData()
        {
            _testData = TournamentList.GetTournaments(new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), "Modern");
        }

        [Test]
        public void TournamentCountIsCorrect()
        {
            _testData.Length.Should().Be(4);
        }

        [Test]
        public void TournamentDataIsCorrect()
        {
            _testData.Should().BeEquivalentTo(new Tournament[]
            {
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern Super Qualifier",           Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-super-qualifier-2020-06-02"),           JsonFile="modern-super-qualifier-2020-06-02.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern Preliminary",               Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-preliminary-2020-06-02"),               JsonFile="modern-preliminary-2020-06-02.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 02, 00, 00, 00, DateTimeKind.Utc), Name="Modern League",                    Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-league-2020-06-02"),                    JsonFile="modern-league-2020-06-02.json" },
                new Tournament(){ Date=new DateTime(2020, 06, 01, 00, 00, 00, DateTimeKind.Utc), Name="Modern Challenge",                 Uri=new Uri("https://magic.wizards.com/en/articles/archive/mtgo-standings/modern-challenge-2020-06-01"),                 JsonFile="modern-challenge-2020-06-01.json" },
            });
        }
    }
}