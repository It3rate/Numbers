using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Numbers.Core;

namespace NumbersTests
{
    [TestClass]
    public class FocalTests
    {
        [TestMethod]
        public void CoreFocalTests()
        {
            var trait = new Trait();
            IFocal f = FocalRef.CreateByValues(trait, 150, 250);
            Assert.AreEqual(f.StartTickPosition, 150);
            Assert.AreEqual(f.EndTickPosition, 250);
            Assert.AreEqual(f.LengthInTicks, 100);
            Assert.AreEqual(f.Direction, 1);
            Assert.AreEqual(f.NonZeroLength, 100);
            Assert.AreEqual(f.AbsLengthInTicks, 100);
            Assert.AreEqual(f.Kind, MathElementKind.Focal);
            var f2 = f.Clone();
            Assert.AreNotEqual(f.Id, f2.Id);
            Assert.AreEqual(f.StartTickPosition, f2.StartTickPosition);
            Assert.AreEqual(f.EndTickPosition, f2.EndTickPosition);

            f2.StartTickPosition = 450;
            Assert.AreEqual(f2.StartTickPosition, 450);
            Assert.AreEqual(f2.EndTickPosition, 250);
            Assert.AreEqual(f2.LengthInTicks, -200);
            Assert.AreEqual(f2.Direction, -1);
            Assert.AreEqual(f2.NonZeroLength, -200);
            Assert.AreEqual(f2.AbsLengthInTicks, 200);

            f2.EndTickPosition = 450;
            Assert.AreEqual(f2.LengthInTicks, 0);
            Assert.AreEqual(f2.NonZeroLength, 1);
            Assert.AreEqual(f2.AbsLengthInTicks, 0);

        }
    }
}
