using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DndTools.Models.TreasureHoard.Helpers;

namespace UnitTesting
{
    [TestClass]
    public class DiceRollerUnitTests
    {
        // Seed is set to 0, which means DiceROller.Roll("15d20") should return
        // 4
        // 6
        // 6
        // 9
        // 9
        // 9
        // 11
        // 11
        // 13
        // 14
        // 15
        // 16
        // 18
        // 19
        // 19

        // ------------------------------------------------------------------------------------------
        // NOTE: I know the expected/actuals are backwards... It looks cleaner this way and 
        // NOTE: it's the way I originally wrote it, so go fuck yourself if it makes you unhappy.
        // ------------------------------------------------------------------------------------------

        [TestMethod]
        public void Test_RunBadExamples()
        {
            // Extra chars at start and finish
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20dh5kl5x"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("x15d20kh5dl5"));

            // Extra chars throughout
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("1x5d20dh5kl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15xd20kh5dl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15dx20dh5kl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d2x0kh5dl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20xdh5kl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20kxh5dl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20dhx5kl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20kh5xdl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20dh5kxl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20kh5dlx5"));

            // Attempting to drop and keep 
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20dh5kl5"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20kh5dl5"));

            // Trying to add/subtract multiple times
            Assert.ThrowsException<System.Exception>(() => DiceRoller.RollForSum("15d20+50+50"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.RollForSum("15d20+50-50"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.RollForSum("15d20-50-50"));
        }

        [TestMethod]
        public void Test_xdy()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });
        }

        [TestMethod]
        public void Test_SelectiveHighLow()
        {
            // Keeps
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh5"), new List<int>() { 19, 19, 18, 16, 15 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl5"), new List<int>() { 4, 6, 6, 9, 9});

            //Drops
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh5"), new List<int>() { 14, 13, 11, 11, 9, 9, 9, 6, 6, 4});
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl5"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9});
        }

        [TestMethod]
        public void Test_SelectiveHighLow_OutOfBounds()
        {
            // Keeps
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh50"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl50"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });

            // Drops
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh50"), new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl50"), new List<int>() { });
        }

        [TestMethod]
        public void Test_DropLowAndHigh()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl5dh5"), new List<int>() { 14, 13, 11, 11, 9 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh5dl5"), new List<int>() { 14, 13, 11, 11, 9 });
        }

        [TestMethod]
        public void Test_KeepLowAndHigh()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl5kh5"), new List<int>() { 19, 19, 18, 16, 15, 9, 9, 6, 6, 4 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh5kl5"), new List<int>() { 19, 19, 18, 16, 15, 9, 9, 6, 6, 4 });
        }

        [TestMethod]
        public void Test_DropLowAndHigh_OutOfBounds()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl1dh20"), new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh1dl20"), new List<int>() { });

            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl20dh1"), new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh20dl1"), new List<int>() { });
        }

        [TestMethod]
        public void Test_KeepLowAndHigh_OutOfBounds()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl1kh20"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh1kl20"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });

            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl20kh1"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh20kl1"), new List<int>() { 19, 19, 18, 16, 15, 14, 13, 11, 11, 9, 9, 9, 6, 6, 4 });
        }

        [TestMethod]
        public void Test_SelectiveNotationWithParenthesis()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl(5)dh(5)"), new List<int>() { 14, 13, 11, 11, 9 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh(5)dl(5)"), new List<int>() { 14, 13, 11, 11, 9 });

            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl(5)kh(5)"), new List<int>() { 19, 19, 18, 16, 15, 9, 9, 6, 6, 4 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh(5)kl(5)"), new List<int>() { 19, 19, 18, 16, 15, 9, 9, 6, 6, 4 });
        }

        [TestMethod]
        public void Test_Multipliers()
        {
            // Multiplication
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20×2"), 179 * 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x2"), 179 * 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20*2"), 179 * 2);

            // Division
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2"), 179 / 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20÷2"), 179 / 2);

            // Division rounded up
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2", true), (179 / 2) + 1);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20÷2", true), (179 / 2) + 1);

            // Multiple multipliers (honestly don't know why this is here, but this is how it works...)
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x4/2"), 179 * 4 / 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x2x2/4"), 179 * 2 * 2 / 4);

            // This one is weird. Our algorithm is better than C# because we convert to floats as we do the math.
            // 179 / 2 * 4 / 2 * 4 = 716 but C# truncates after the first division resulting in an answer of 712.
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2*4/2*4"), 716);
        }

        [TestMethod]
        public void Test_AdditionSubtraction()
        {
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20+50"), 179 + 50);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20-50"), 179 - 50);
        }

        [TestMethod]
        public void Test_MultipliersWithAdditionSubtraction()
        {
            // Multiplication
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20×2+2"), (179 * 2) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20×2-2"), (179 * 2) - 2);

            // Division
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2+2"), (179 / 2) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2-2"), (179 / 2) - 2);

            // Division rounded up
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2+2", true), ((179 / 2) + 1) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2-2", true), ((179 / 2) + 1) - 2);

            // Multiple multipliers (honestly don't know why this is here, but this is how it works...)
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x4/2+2"), (179 * 4 / 2) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x4/2-2"), (179 * 4 / 2) - 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x2x2/4+2"), (179 * 2 * 2 / 4) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20x2x2/4-2"), (179 * 2 * 2 / 4) - 2);

            // This one is weird. Our algorithm is better than C# because we convert to floats as we do the math.
            // 179 / 2 * 4 / 2 * 4 = 716 but C# truncates after the first division resulting in an answer of 712.
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2*4/2*4+2"), 716 + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.RollForSum("15d20/2*4/2*4-2"), 716 - 2);
        }
    }
}
