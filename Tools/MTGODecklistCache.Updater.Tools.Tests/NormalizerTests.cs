using FluentAssertions;
using NUnit.Framework;

namespace MTGODecklistCache.Updater.Tools.Tests
{
    public class CardNormalizerTests
    {
        [Test]
        public void ShouldRemoveLeadingSpaces()
        {
            CardNameNormalizer.Normalize(" Arclight Phoenix").Should().Be("Arclight Phoenix");
            CardNameNormalizer.Normalize("  Arclight Phoenix").Should().Be("Arclight Phoenix");
        }

        [Test]
        public void ShouldRemoveTrailingSpaces()
        {
            CardNameNormalizer.Normalize("Arclight Phoenix ").Should().Be("Arclight Phoenix");
            CardNameNormalizer.Normalize("Arclight Phoenix  ").Should().Be("Arclight Phoenix");
        }

        [Test]
        public void ShouldNormalizeSplitCards()
        {
            CardNameNormalizer.Normalize("Fire").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire/Ice").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire / Ice").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire//Ice").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire // Ice").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire///Ice").Should().Be("Fire // Ice");
            CardNameNormalizer.Normalize("Fire /// Ice").Should().Be("Fire // Ice");
        }

        [Test]
        public void ShouldNormalizeFlipCards()
        {
            CardNameNormalizer.Normalize("Akki Lavarunner").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner/Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner / Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner//Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner // Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner///Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
            CardNameNormalizer.Normalize("Akki Lavarunner /// Tok-Tok, Volcano Born").Should().Be("Akki Lavarunner");
        }

        [Test]
        public void ShouldNormalizeAdventureCards()
        {
            CardNameNormalizer.Normalize("Brazen Borrower").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower/Petty Theft").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower / Petty Theft").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower//Petty Theft").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower // Petty Theft").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower///Petty Theft").Should().Be("Brazen Borrower");
            CardNameNormalizer.Normalize("Brazen Borrower /// Petty Theft").Should().Be("Brazen Borrower");
        }

        [Test]
        public void ShouldNormalizeDualFaceCards()
        {
            CardNameNormalizer.Normalize("Delver of Secrets").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets/Insectile Aberration ").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets / Insectile Aberration ").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets//Insectile Aberration ").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets // Insectile Aberration ").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets///Insectile Aberration ").Should().Be("Delver of Secrets");
            CardNameNormalizer.Normalize("Delver of Secrets /// Insectile Aberration ").Should().Be("Delver of Secrets");
        }

        [Test]
        public void ShouldFixAetherVial()
        {
            CardNameNormalizer.Normalize("Æther Vial").Should().Be("Aether Vial");
            CardNameNormalizer.Normalize("Ã\u0086ther Vial").Should().Be("Aether Vial");
        }

        [Test]
        public void ShouldFixAetherInDualFaceCards()
        {
            CardNameNormalizer.Normalize("Invasion of Kaladesh // Ætherwing, Golden-Scale Flagship").Should().Be("Invasion of Kaladesh");
            CardNameNormalizer.Normalize("Invasion of Kaladesh // Ã\u0086therwing, Golden-Scale Flagship").Should().Be("Invasion of Kaladesh");
        }

        [Test]
        public void ShouldFixFullArtLands()
        {
            CardNameNormalizer.Normalize("Full Art Plains").Should().Be("Plains");
            CardNameNormalizer.Normalize("Full Art Island").Should().Be("Island");
            CardNameNormalizer.Normalize("Full Art Swamp").Should().Be("Swamp");
            CardNameNormalizer.Normalize("Full Art Mountain").Should().Be("Mountain");
            CardNameNormalizer.Normalize("Full Art Forest").Should().Be("Forest");
        }

        [Test]
        public void ShouldFixLurrus()
        {
            CardNameNormalizer.Normalize("Lurrus of the Dream Den").Should().Be("Lurrus of the Dream-Den");
        }

        [Test]
        public void ShouldFixKongming()
        {
            CardNameNormalizer.Normalize("Kongming, ??quot?Sleeping Dragon??quot?").Should().Be("Kongming, \"Sleeping Dragon\"");

        }

        [Test]
        public void ShouldFixGhazbanOgre()
        {
            CardNameNormalizer.Normalize("GhazbA?n Ogre").Should().Be("Ghazbán Ogre");
        }

        [Test]
        public void ShouldFixVault()
        {
            CardNameNormalizer.Normalize("Lim-DA?l's Vault").Should().Be("Lim-Dûl's Vault");
            CardNameNormalizer.Normalize("Lim-DAul's Vault").Should().Be("Lim-Dûl's Vault");
            CardNameNormalizer.Normalize("Lim-DÃ»l's Vault").Should().Be("Lim-Dûl's Vault");
        }

        [Test]
        public void ShouldFixSeance()
        {
            CardNameNormalizer.Normalize("SAcance").Should().Be("Séance");
            CardNameNormalizer.Normalize("SÃ©ance").Should().Be("Séance");
        }

        [Test]
        public void ShouldFixDjinn()
        {
            CardNameNormalizer.Normalize("JuzA?m Djinn").Should().Be("Juzám Djinn");
            CardNameNormalizer.Normalize("JuzÃ¡m Djinn").Should().Be("Juzám Djinn");
        }

        [Test]
        public void ShouldFixSolkanar()
        {
            CardNameNormalizer.Normalize("Sol'kanar the Tainted").Should().Be("Sol'Kanar the Tainted");
        }

        [Test]
        public void ShouldFixMinsc()
        {
            CardNameNormalizer.Normalize("Minsc ?amp? Boo, Timeless Heroes").Should().Be("Minsc & Boo, Timeless Heroes");
        }

        [Test]
        public void ShouldFixGrunt()
        {
            CardNameNormalizer.Normalize("Jotun Grunt").Should().Be("Jötun Grunt");
        }

        [Test]
        public void ShouldFixRain()
        {
            CardNameNormalizer.Normalize("Rain Of Tears").Should().Be("Rain of Tears");
        }

        [Test]
        public void ShouldFixAltar()
        {
            CardNameNormalizer.Normalize("Altar Of Dementia").Should().Be("Altar of Dementia");
        }


        [Test]
        public void ShouldFixTura()
        {
            CardNameNormalizer.Normalize("Tura KennerÃ¼d, Skyknight").Should().Be("Tura Kennerüd, Skyknight");
        }

        [Test]
        public void ShouldFixHall()
        {
            CardNameNormalizer.Normalize("Hall of the Storm Giants").Should().Be("Hall of Storm Giants");
        }
    }
}