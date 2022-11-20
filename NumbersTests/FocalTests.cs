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
            Assert.AreEqual(f.Kind, MathElementKind.Focal);
            Assert.AreEqual(f.StartTickPosition, 150);
            Assert.AreEqual(f.EndTickPosition, 250);
            Assert.AreEqual(f.LengthInTicks, 100);
            Assert.AreEqual(f.Direction, 1);
            Assert.AreEqual(f.NonZeroLength, 100);
            Assert.AreEqual(f.AbsLengthInTicks, 100);
            var f2 = f.Clone();
            Assert.AreNotEqual(f.Id, f2.Id);
            Assert.AreEqual(f.StartTickPosition, f2.StartTickPosition);
            Assert.AreEqual(f.EndTickPosition, f2.EndTickPosition);

            Assert.AreEqual(f2.RangeWithBasis(f), new Range(0, 1));

            f2.StartTickPosition = 450;
            Assert.AreEqual(f2.StartTickPosition, 450);
            Assert.AreEqual(f2.EndTickPosition, 250);
            Assert.AreEqual(f2.LengthInTicks, -200);
            Assert.AreEqual(f2.Direction, -1);
            Assert.AreEqual(f2.NonZeroLength, -200);
            Assert.AreEqual(f2.AbsLengthInTicks, 200);

            // f:[150,250] f2[450,250]
            Assert.AreEqual(f2.RangeWithBasis(f), new Range(-3, 1));
            Assert.AreEqual(f.RangeAsBasis(f2), new Range(-3, 1));
            Assert.AreEqual(f2.RangeAsBasis(f), new Range(-1.5, 1));
            Assert.AreEqual(f.RangeWithBasis(f2), new Range(-1.5, 1));
            f2.StartTickPosition = 500;
            Assert.AreEqual(f2.RangeWithBasis(f), new Range(-3.5, 1));
            Assert.AreEqual(f.RangeAsBasis(f2), new Range(-3.5, 1));
            Assert.AreEqual(f2.RangeAsBasis(f), new Range(-1.4, 1));
            Assert.AreEqual(f.RangeWithBasis(f2), new Range(-1.4, 1));

            f2.StartTickPosition = -150;
            Assert.AreEqual(f2.RangeWithBasis(f), new Range(3, 1));
            Assert.AreEqual(f.RangeWithBasis(f2), new Range(-0.75, 1));

            f2.EndTickPosition = -150;
            Assert.AreEqual(f2.LengthInTicks, 0);
            Assert.AreEqual(f2.NonZeroLength, 1);
            Assert.AreEqual(f2.AbsLengthInTicks, 0);

        }
    }
}
