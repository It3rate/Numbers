using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Numbers.Core;

namespace NumbersTests
{
    [TestClass]
    public class FocalTests
    {
	    private Trait _trait;

	    [TestInitialize]
	    public void Init()
	    {
		    _trait = new Trait();
	    }

        [TestMethod]
	    public void ReciprocalFocalTests()
	    {
		    IFocal f0 = FocalRef.CreateByValues(_trait, 0, 10);
		    IFocal f1 = FocalRef.CreateByValues(_trait, 132, 287);
		    Assert.AreEqual(-13.2, f1.GetRangeWithBasis(f0, false).Start);
		    Assert.AreEqual(28.7, f1.GetRangeWithBasis(f0, false).End);
		    Assert.AreEqual(-130, f1.GetRangeWithBasis(f0, true).Start);
		    Assert.AreEqual(290, f1.GetRangeWithBasis(f0, true).End);
        }

	    [TestMethod]
        public void CoreFocalTests()
        {
            IFocal f = FocalRef.CreateByValues(_trait, 150, 250);
            Assert.AreEqual(MathElementKind.Focal, f.Kind);
            Assert.AreEqual(150, f.StartTickPosition);
            Assert.AreEqual(250, f.EndTickPosition);
            Assert.AreEqual(100, f.LengthInTicks);
            Assert.AreEqual(1, f.Direction);
            Assert.AreEqual(100, f.NonZeroLength);
            Assert.AreEqual(100, f.AbsLengthInTicks);
            var f2 = f.Clone();
            Assert.AreNotEqual(f.Id, f2.Id);
            Assert.AreEqual(f2.StartTickPosition, f.StartTickPosition);
            Assert.AreEqual(f.EndTickPosition, f2.EndTickPosition);

            Assert.AreEqual(new Range(0, 1), f2.GetRangeWithBasis(f, false));

            f2.StartTickPosition = 450;
            Assert.AreEqual(450, f2.StartTickPosition);
            Assert.AreEqual(250, f2.EndTickPosition);
            Assert.AreEqual(-200, f2.LengthInTicks);
            Assert.AreEqual(-1, f2.Direction);
            Assert.AreEqual(-200, f2.NonZeroLength);
            Assert.AreEqual(200, f2.AbsLengthInTicks);

            // f:[150,250] f2[450,250]
            Assert.AreEqual(new Range(-3, 1), f2.GetRangeWithBasis(f, false));
            Assert.AreEqual(new Range(-3, 1), f.RangeAsBasis(f2));
            Assert.AreEqual(new Range(-1.5, 1), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1.5, 1), f.GetRangeWithBasis(f2, false));
            // f:[150,250] f2[500,250]
            f2.StartTickPosition = 500;
            Assert.AreEqual(new Range(-3.5, 1), f2.GetRangeWithBasis(f, false));
            Assert.AreEqual(new Range(-3.5, 1), f.RangeAsBasis(f2));
            Assert.AreEqual(new Range(-1.4, 1), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1.4, 1), f.GetRangeWithBasis(f2, false));

            // f:[150,250] f2[-150,250]
            f2.StartTickPosition = -150;
            Assert.AreEqual(new Range(3, 1), f2.GetRangeWithBasis(f, false));
            Assert.AreEqual(new Range(-0.75, 1), f.GetRangeWithBasis(f2, false));

            // f:[150,250] f2[-150,-150]
            f2.EndTickPosition = -150;
            Assert.AreEqual(0, f2.LengthInTicks);
            Assert.AreEqual(1, f2.NonZeroLength);
            Assert.AreEqual(0, f2.AbsLengthInTicks);

        }
    }
}
