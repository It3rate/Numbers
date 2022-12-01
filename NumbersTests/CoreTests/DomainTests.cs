using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace NumbersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class DomainTests
    {
	    private Brain _brain;
	    private Trait _trait;
        private FocalRef _unitFocal;
        private FocalRef _maxMin;
        private Domain _domain;

        [TestInitialize]
        public void Init()
        {
	        _brain = Brain.ActiveBrain;
            _trait = new Trait(_brain);
	        _unitFocal = FocalRef.CreateByValues(_trait, -4, 6);
	        _maxMin = FocalRef.CreateByValues(_trait, -54, 46);
	        _domain = new Domain(_trait, _unitFocal, _maxMin);
        }

        [TestMethod]
	    public void CoreDomainTests()
	    {
		    Assert.AreEqual(MathElementKind.Domain, _domain.Kind);
            Assert.AreEqual(100, _domain.MinMaxFocal.LengthInTicks);
            Assert.AreEqual(10, _domain.BasisFocal.LengthInTicks);
            Assert.AreEqual(_domain.BasisFocal.Id, _unitFocal.Id);
            Assert.AreEqual(_domain.MinMaxFocal.Id, _maxMin.Id);
            Assert.AreEqual(_domain.MinMaxRange.Length, 10.0, Utils.Tolerance);

            var f0 = FocalRef.CreateByValues(_trait, 10, 20);
            var n0 = new Number(_domain, f0);
            var n1 = new Number(_domain, f0.Clone());
            var n2 = new Number(_domain, f0.Clone());
            Assert.AreEqual(5, _domain.NumberIds.Count); // includes unit basis and minmax

            var dict = new Dictionary<int, Range>();

            _domain.SaveNumberValues(dict, _domain.BasisNumberId);
            Assert.AreEqual(4, dict.Count); // does not include unit
            var saved = n1.Focal.EndTickPosition;
            n1.Focal.EndTickPosition = 22;
            Assert.AreNotEqual(n1.Focal.EndTickPosition, saved);
            Assert.AreEqual(22, n1.Focal.EndTickPosition);
            _domain.RestoreNumberValues(dict);
            Assert.AreEqual(saved, n1.Focal.EndTickPosition);

            Assert.IsTrue(_domain.IsUnitPerspective);
            _unitFocal.StartTickPosition = 16;
            Assert.IsFalse(_domain.IsUnitPerspective);

        }

	    [TestMethod]
	    public void DomainValueTests()
        {
            _unitFocal.Reset(0, 10);

	        var num = new Number(_domain, FocalRef.CreateByValues(_trait, 30, 40));
	        var r = num.Value;
	        var ffr = _domain.CreateFocalFromRange(r, true);
            Assert.AreEqual(ffr, num.Focal);

            num = new Number(_domain, FocalRef.CreateByValues(_trait, -30, 1));
            r = num.Value;
            ffr = _domain.CreateFocalFromRange(r, true);
            Assert.AreEqual(ffr, num.Focal);

            _unitFocal.Reset(10, -10);
	        var testFocal = FocalRef.CreateByValues(_trait, 0, 6);

            num = new Number(_domain, FocalRef.CreateByValues(_trait, 30, 40));
	        r = num.Value;
	        _domain.SetValueOf(testFocal, r);

            Assert.AreEqual(num.Focal.StartTickPosition, testFocal.StartTickPosition);
            Assert.AreEqual(num.Focal.EndTickPosition, testFocal.EndTickPosition);
            ffr = _domain.CreateFocalFromRange(r, true);
            Assert.AreEqual(ffr, num.Focal);
        }
    }
}
