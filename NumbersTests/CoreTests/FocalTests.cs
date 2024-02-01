using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using static System.Net.Mime.MediaTypeNames;

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
        public void InvertedEndPositionTests()
        {
            Focal f0 = Focal.CreateByValues(0, 10);
            Assert.AreEqual(-10, f0.InvertedEndPosition);
            Focal f1 = Focal.CreateByValues(20, 30);
            Assert.AreEqual(10, f1.InvertedEndPosition);
            Focal f2 = Focal.CreateByValues(-20, -15);
            Assert.AreEqual(-25, f2.InvertedEndPosition);
            Focal f3 = Focal.CreateByValues(-20, -35);
            Assert.AreEqual(-5, f3.InvertedEndPosition);
            Focal f4 = Focal.CreateByValues(30, 15);
            Assert.AreEqual(45, f4.InvertedEndPosition);
            Focal f5 = Focal.CreateByValues(30, 45);
            Assert.AreEqual(15, f5.InvertedEndPosition);
        }

        [TestMethod]
	    public void BasisSignTests()
	    {
		    Focal fb = Focal.CreateByValues(0, 10);
		    Focal fn = Focal.CreateByValues(20, 30);
		    Focal ftest = Focal.CreateByValues(0, 0);

		    // focal [0,10]  fn [20, 30]
            Assert.AreEqual(new Range(-2, 3), fn.GetRangeWithBasis(fb, false, true));
            ftest.SetWithRangeAndBasis(new Range(-2, 3), fb, false, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);

            Assert.AreEqual(new Range(-20, 30), fn.GetRangeWithBasis(fb, true, true));
            ftest.SetWithRangeAndBasis(new Range(-20, 30), fb, true, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);


            fb.EndPosition = -10; // focal [0,-10]  fn [40, 30]

            Assert.AreEqual(new Range(3, -2), fn.GetRangeWithBasis(fb, false, true));
		    ftest.SetWithRangeAndBasis(new Range(3, -2), fb, false, true);
		    Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
		    Assert.AreEqual(ftest.EndPosition, fn.EndPosition);

		    Assert.AreEqual(new Range(30, -20), fn.GetRangeWithBasis(fb, true, true));
		    ftest.SetWithRangeAndBasis(new Range(30, -20), fb, true, true);
		    Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
		    Assert.AreEqual(ftest.EndPosition, fn.EndPosition);


            fn.StartPosition = 40; // focal [0,-10],  fn [40, 30]

            Assert.AreEqual(new Range(3, -4), fn.GetRangeWithBasis(fb, false, true));
            ftest.SetWithRangeAndBasis(new Range(3, -4), fb, false, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);

            Assert.AreEqual(new Range(30, -40), fn.GetRangeWithBasis(fb, true, true));
            ftest.SetWithRangeAndBasis(new Range(30, -40), fb, true, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);


            fb.EndPosition = 10; // focal [0,10],  fn [40, 30]

            Assert.AreEqual(new Range(-4, 3), fn.GetRangeWithBasis(fb, false, true));
            ftest.SetWithRangeAndBasis(new Range(-4, 3), fb, false, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);

            Assert.AreEqual(new Range(-40, 30), fn.GetRangeWithBasis(fb, true, true));
            ftest.SetWithRangeAndBasis(new Range(-40, 30), fb, true, true);
            Assert.AreEqual(ftest.StartPosition, fn.StartPosition);
            Assert.AreEqual(ftest.EndPosition, fn.EndPosition);
        }
	    [TestMethod]
	    public void ReciprocalTests()
	    {
		    Focal f0 = Focal.CreateByValues(0, 10);
		    Focal f1 = Focal.CreateByValues(132, 287);
		    Assert.AreEqual(-13.2, f1.GetRangeWithBasis(f0, false, true).Start);
		    Assert.AreEqual(28.7, f1.GetRangeWithBasis(f0, false, true).End);
		    Assert.AreEqual(-130, f1.GetRangeWithBasis(f0, true, true).Start);
		    Assert.AreEqual(290, f1.GetRangeWithBasis(f0, true, true).End);
	    }

        [TestMethod]
	    public void RangeToUnitTests()
	    {
		    Focal fb = Focal.CreateByValues(0, 10);
		    Focal fn = Focal.CreateByValues(20, 30);
		    Focal testFocal = Focal.CreateByValues(0, 0);
		    var range = fn.GetRangeWithBasis(fb, false, true);
            testFocal.SetWithRangeAndBasis(range, fb, false, true);
            Assert.AreEqual(testFocal.StartPosition, fn.StartPosition);
            Assert.AreEqual(testFocal.EndPosition, fn.EndPosition);

        }

	    [TestMethod]
        public void CoreFocalTests()
        {
            Focal f = Focal.CreateByValues(150, 250);
            Assert.AreEqual(MathElementKind.Focal, f.Kind);
            Assert.AreEqual(150, f.StartPosition);
            Assert.AreEqual(250, f.EndPosition);
            Assert.AreEqual(100, f.LengthInTicks);
            Assert.AreEqual(1, f.Direction);
            Assert.AreEqual(100, f.NonZeroLength);
            Assert.AreEqual(100, f.AbsLengthInTicks);
            var f2 = f.Clone();
            Assert.AreNotEqual(f.Id, f2.Id);
            Assert.AreEqual(f2.StartPosition, f.StartPosition);
            Assert.AreEqual(f.EndPosition, f2.EndPosition);

            Assert.AreEqual(new Range(0, 1), f2.GetRangeWithBasis(f, false, true));

            f2.StartPosition = 450;
            Assert.AreEqual(450, f2.StartPosition);
            Assert.AreEqual(250, f2.EndPosition);
            Assert.AreEqual(-200, f2.LengthInTicks);
            Assert.AreEqual(-1, f2.Direction);
            Assert.AreEqual(-200, f2.NonZeroLength);
            Assert.AreEqual(200, f2.AbsLengthInTicks);

            // f:[150,250] f2[450,250]
            Assert.AreEqual(new Range(-3, 1), f2.GetRangeWithBasis(f, false, true));
            Assert.AreEqual(new Range(-3, 1), f.RangeAsBasis(f2));
            Assert.AreEqual(new Range(-1, 1.5), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1, 1.5), f.GetRangeWithBasis(f2, false, true));
            // f:[150,250] f2[500,250]
            f2.StartPosition = 500;
            Assert.AreEqual(new Range(-3.5, 1), f2.GetRangeWithBasis(f, false, true));
            Assert.AreEqual(new Range(-3.5, 1), f.RangeAsBasis(f2));
            Assert.AreEqual(new Range(-1, 1.4), f2.RangeAsBasis(f));
            Assert.AreEqual(new Range(-1, 1.4), f.GetRangeWithBasis(f2, false, true));

            // f:[150,250] f2[-150,250]
            f2.StartPosition = -150;
            Assert.AreEqual(new Range(3, 1), f2.GetRangeWithBasis(f, false, true));
            Assert.AreEqual(new Range(-0.75, 1), f.GetRangeWithBasis(f2, false, true));

            // f:[150,250] f2[-150,-150]
            f2.EndPosition = -150;
            Assert.AreEqual(0, f2.LengthInTicks);
            Assert.AreEqual(1, f2.NonZeroLength);
            Assert.AreEqual(0, f2.AbsLengthInTicks);

        }
    }
}
