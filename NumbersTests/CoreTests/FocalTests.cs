using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace NumbersTests
{
    [TestClass]
    public class FocalTests
    {
	    private Brain _brain => Brain.ActiveBrain;
        private Trait _trait;

	    [TestInitialize]
	    public void Init()
	    {
		    _trait = Trait.CreateIn(_brain, "numbers tests");
        }

	    [TestMethod]
	    public void BasisSignTests()
	    {
		    Focal fb = FocalRef.CreateByValues(_trait, 0, 10);
		    Focal fn = FocalRef.CreateByValues(_trait, 20, 30);
		    Focal ftest = FocalRef.CreateByValues(_trait, 0, 0);

		    // focal [0,10]  fn [20, 30]
            Assert.AreEqual(new Range(-2, 3), fn.GetRangeWithBasis(fb, false));
            ftest.SetWithRangeAndBasis(new Range(-2, 3), fb, false);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);

            Assert.AreEqual(new Range(-20, 30), fn.GetRangeWithBasis(fb, true));
            ftest.SetWithRangeAndBasis(new Range(-20, 30), fb, true);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);


            fb.EndTickPosition = -10; // focal [0,-10]  fn [40, 30]

            Assert.AreEqual(new Range(3, -2), fn.GetRangeWithBasis(fb, false));
		    ftest.SetWithRangeAndBasis(new Range(3, -2), fb, false);
		    Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
		    Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);

		    Assert.AreEqual(new Range(30, -20), fn.GetRangeWithBasis(fb, true));
		    ftest.SetWithRangeAndBasis(new Range(30, -20), fb, true);
		    Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
		    Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);


            fn.StartTickPosition = 40; // focal [0,-10],  fn [40, 30]

            Assert.AreEqual(new Range(3, -4), fn.GetRangeWithBasis(fb, false));
            ftest.SetWithRangeAndBasis(new Range(3, -4), fb, false);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);

            Assert.AreEqual(new Range(30, -40), fn.GetRangeWithBasis(fb, true));
            ftest.SetWithRangeAndBasis(new Range(30, -40), fb, true);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);


            fb.EndTickPosition = 10; // focal [0,10],  fn [40, 30]

            Assert.AreEqual(new Range(-4, 3), fn.GetRangeWithBasis(fb, false));
            ftest.SetWithRangeAndBasis(new Range(-4, 3), fb, false);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);

            Assert.AreEqual(new Range(-40, 30), fn.GetRangeWithBasis(fb, true));
            ftest.SetWithRangeAndBasis(new Range(-40, 30), fb, true);
            Assert.AreEqual(ftest.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(ftest.EndTickPosition, fn.EndTickPosition);
        }
	    [TestMethod]
	    public void ReciprocalTests()
	    {
		    Focal f0 = FocalRef.CreateByValues(_trait, 0, 10);
		    Focal f1 = FocalRef.CreateByValues(_trait, 132, 287);
		    Assert.AreEqual(-13.2, f1.GetRangeWithBasis(f0, false).Start);
		    Assert.AreEqual(28.7, f1.GetRangeWithBasis(f0, false).End);
		    Assert.AreEqual(-130, f1.GetRangeWithBasis(f0, true).Start);
		    Assert.AreEqual(290, f1.GetRangeWithBasis(f0, true).End);
	    }

        [TestMethod]
	    public void RangeToUnitTests()
	    {
		    Focal fb = FocalRef.CreateByValues(_trait, 0, 10);
		    Focal fn = FocalRef.CreateByValues(_trait, 20, 30);
		    Focal testFocal = FocalRef.CreateByValues(_trait, 0, 0);
		    var range = fn.GetRangeWithBasis(fb, false);
            testFocal.SetWithRangeAndBasis(range, fb, false);
            Assert.AreEqual(testFocal.StartTickPosition, fn.StartTickPosition);
            Assert.AreEqual(testFocal.EndTickPosition, fn.EndTickPosition);

        }

	    [TestMethod]
        public void CoreFocalTests()
        {
            Focal f = FocalRef.CreateByValues(_trait, 150, 250);
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
            Assert.AreEqual(new Range(-1, 1.5), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1, 1.5), f.GetRangeWithBasis(f2, false));
            // f:[150,250] f2[500,250]
            f2.StartTickPosition = 500;
            Assert.AreEqual(new Range(-3.5, 1), f2.GetRangeWithBasis(f, false));
            Assert.AreEqual(new Range(-3.5, 1), f.RangeAsBasis(f2));
            Assert.AreEqual(new Range(-1, 1.4), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1, 1.4), f.GetRangeWithBasis(f2, false));

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
