using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using DndTools.Models.TreasureHoard.Helpers;

namespace UnitTesting
{
    [TestClass]
    public class DiceRollerUnitTests
    {
        // Seed is set to 0, which means DiceRoller.Roll("15d20") should return
        // 5
        // 6
        // 6
        // 9
        // 10
        // 10
        // 12
        // 12
        // 13
        // 15
        // 16
        // 17
        // 19
        // 20
        // 20

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
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20+50+50"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20+50-50"));
            Assert.ThrowsException<System.Exception>(() => DiceRoller.Roll("15d20-50-50"));
        }

        [TestMethod]
        public void Test_xdy()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
        }

        [TestMethod]
        public void Test_SelectiveHighLow()
        {
            // Keeps
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh5").KeptRolls, new List<int>() { 20, 20, 19, 17, 16 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl5").KeptRolls, new List<int>() { 10, 9, 6, 6, 5 });

            //Drops
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh5").KeptRolls, new List<int>() { 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl5").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10 });
        }

        [TestMethod]
        public void Test_SelectiveHighLow_OutOfBounds()
        {
            // Keeps
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh50").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl50").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });

            // Drops
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh50").KeptRolls, new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl50").KeptRolls, new List<int>() { });
        }

        [TestMethod]
        public void Test_DropLowAndHigh()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl5dh5").KeptRolls, new List<int>() { 15, 13, 12, 12, 10 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh5dl5").KeptRolls, new List<int>() { 15, 13, 12, 12, 10 });
        }

        [TestMethod]
        public void Test_KeepLowAndHigh()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl5kh5").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 10, 9, 6, 6, 5 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh5kl5").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 10, 9, 6, 6, 5 });
        }

        [TestMethod]
        public void Test_DropLowAndHigh_OutOfBounds()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl1dh20").KeptRolls, new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh1dl20").KeptRolls, new List<int>() { });

            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dl20dh1").KeptRolls, new List<int>() { });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20dh20dl1").KeptRolls, new List<int>() { });
        }

        [TestMethod]
        public void Test_KeepLowAndHigh_OutOfBounds()
        {
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl1kh20").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh1kl20").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });

            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kl20kh1").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
            Random.SetSeed(0); CollectionAssert.AreEquivalent(DiceRoller.Roll("15d20kh20kl1").KeptRolls, new List<int>() { 20, 20, 19, 17, 16, 15, 13, 12, 12, 10, 10, 9, 6, 6, 5 });
        }

        [TestMethod]
        public void Test_Multipliers()
        {
            // Multiplication
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20×2").Value, 190 * 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20x2").Value, 190 * 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20*2").Value, 190 * 2);

            // Division
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20/2").Value, 190 / 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20÷2").Value, 190 / 2);
        }

        [TestMethod]
        public void Test_AdditionSubtraction()
        {
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20+50").Value, 190 + 50);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20-50").Value, 190 - 50);
        }

        [TestMethod]
        public void Test_MultipliersWithAdditionSubtraction()
        {
            // Multiplication
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20×2+2").Value, (190 * 2) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20×2-2").Value, (190 * 2) - 2);

            // Division
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20/2+2").Value, (190 / 2) + 2);
            Random.SetSeed(0); Assert.AreEqual(DiceRoller.Roll("15d20/2-2").Value, (190 / 2) - 2);
        }
    }
}
