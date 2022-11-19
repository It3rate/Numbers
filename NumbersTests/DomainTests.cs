using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numbers.Core;

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
        public static double _err = 0.00001;
        private Trait _trait;
        private FocalRef _unitFocal;
        private FocalRef _maxMin;
        private Domain _domain;

        [TestInitialize]
        public void Init()
        {
	        _trait = new Trait();
	        _unitFocal = FocalRef.CreateByValues(_trait, -4, 6);
	        _maxMin = FocalRef.CreateByValues(_trait, -54, 46);
	        _domain = new Domain(_trait.Id, _unitFocal.Id, _maxMin.Id);
        }

        [TestMethod]
	    public void CoreDomainTests()
	    {
		    Assert.AreEqual(_domain.Kind, MathElementKind.Domain);
            Assert.AreEqual(_domain.MinMaxFocal.LengthInTicks, 100);
            Assert.AreEqual(_domain.BasisFocal.LengthInTicks, 10);
            Assert.AreEqual(_unitFocal.Id, _domain.BasisFocal.Id);
            Assert.AreEqual(_maxMin.Id, _domain.MinMaxFocal.Id);
            Assert.AreEqual(_domain.MinMaxRange.Length, 10.0, _err);

            var f0 = FocalRef.CreateByValues(_trait, 10, 20);
            var n0 = new Number(_domain, f0.Id);
            var n1 = new Number(_domain, f0.Clone().Id);
            var n2 = new Number(_domain, f0.Clone().Id);
            Assert.AreEqual(_domain.NumberIds.Count, 4); // includes unit basis
            
            var dict = new Dictionary<int, Range>();

            _domain.SaveNumberValues(dict, _domain.BasisNumberId);
            Assert.AreEqual(dict.Count, 3); // does not include unit
            var saved = n1.Focal.EndTickPosition;
            n1.Focal.EndTickPosition = 22;
            Assert.AreNotEqual(n1.Focal.EndTickPosition, saved);
            Assert.AreEqual(n1.Focal.EndTickPosition, 22);
            _domain.RestoreNumberValues(dict);
            Assert.AreEqual(n1.Focal.EndTickPosition, saved);

            Assert.IsTrue(_domain.IsUnitPerspective);
            _unitFocal.StartTickPosition = 16;
            Assert.IsFalse(_domain.IsUnitPerspective);

        }

	    [TestMethod]
	    public void DomainValueTests()
        {
            _unitFocal.Reset(0, 10);

	        var num = new Number(_domain, FocalRef.CreateByValues(_trait, 30, 40).Id);
	        var r = num.Value;
	        var ffr = _domain.FocalFromRange(r);
	        Assert.AreEqual(num.Focal, ffr);

	        num = new Number(_domain, FocalRef.CreateByValues(_trait, -30, 1).Id);
	        r = num.Value;
	        ffr = _domain.FocalFromRange(r);
	        Assert.AreEqual(num.Focal, ffr);

	        _unitFocal.Reset(10, -10);
	        num = new Number(_domain, FocalRef.CreateByValues(_trait, 30, 40).Id);
	        r = num.Value;
	        Assert.AreEqual(num.Focal.StartTickPosition, _domain.StartTickPositionFrom(r.Start));
	        Assert.AreEqual(num.Focal.EndTickPosition, _domain.EndTickPositionFrom(r.End));
            ffr = _domain.FocalFromRange(r);
	        Assert.AreEqual(num.Focal, ffr);
        }
    }
}
