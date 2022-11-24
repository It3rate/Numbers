using Microsoft.VisualStudio.TestTools.UnitTesting;
using Numbers.Core;
using Numbers.UI;

namespace NumbersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class NumberTests
    {
	    private Trait _trait;
	    private FocalRef _unitFocal;
	    private FocalRef _maxMin;
	    private Domain _domain;

	    [TestInitialize]
	    public void Init()
	    {
		    _trait = new Trait();
		    _unitFocal = FocalRef.CreateByValues(_trait, 0, 10);
		    _maxMin = FocalRef.CreateByValues(_trait, -1000, 1010);
		    _domain = new Domain(_trait.Id, _unitFocal.Id, _maxMin.Id);
	    }
	    [TestMethod]
	    public void UnitChangePositionTests()
	    {
		    var n0 = _domain.CreateNumberByPositions(0, 20);
		    var n1 = _domain.CreateNumberByPositions(20, 0);
		    var n2 = _domain.CreateNumberByPositions(-30, 20);
		    var n3 = _domain.CreateNumberByPositions(-20, -30);
		    Assert.AreEqual(0, n0.StartValue);
		    Assert.AreEqual(2, n0.EndValue);
		    Assert.AreEqual(-2, n1.StartValue);
		    Assert.AreEqual(0, n1.EndValue);
		    Assert.AreEqual(3, n2.StartValue);
		    Assert.AreEqual(2, n2.EndValue);
		    Assert.AreEqual(2, n3.StartValue);
		    Assert.AreEqual(-3, n3.EndValue);
		    _unitFocal.EndTickPosition = 20;
		    Assert.AreEqual(0, n0.StartValue);
		    Assert.AreEqual(1, n0.EndValue);
		    Assert.AreEqual(-1, n1.StartValue);
		    Assert.AreEqual(0, n1.EndValue);
		    Assert.AreEqual(1.5, n2.StartValue);
		    Assert.AreEqual(1, n2.EndValue);
		    Assert.AreEqual(1, n3.StartValue);
		    Assert.AreEqual(-1.5, n3.EndValue);
		    _unitFocal.EndTickPosition = -20;
		    Assert.AreEqual(1, n0.StartValue);
		    Assert.AreEqual(0, n0.EndValue);
		    Assert.AreEqual(0, n1.StartValue);
		    Assert.AreEqual(-1, n1.EndValue);
		    Assert.AreEqual(1, n2.StartValue);
		    Assert.AreEqual(1.5, n2.EndValue);
		    Assert.AreEqual(-1.5, n3.StartValue);
		    Assert.AreEqual(1, n3.EndValue);
		    _unitFocal.StartTickPosition = -10; // unot perspective
		    Assert.AreEqual(3, n0.StartValue);
		    Assert.AreEqual(-1, n0.EndValue);
		    Assert.AreEqual(1, n1.StartValue);
		    Assert.AreEqual(-3, n1.EndValue);
		    Assert.AreEqual(3, n2.StartValue);
		    Assert.AreEqual(2, n2.EndValue);
		    Assert.AreEqual(-2, n3.StartValue);
		    Assert.AreEqual(1, n3.EndValue);
		    _unitFocal.StartTickPosition = 2000; // unot perspective
            _unitFocal.EndTickPosition = -2000; // forces things to about the middle
            Assert.AreEqual(-0.495, n0.StartValue);
            Assert.AreEqual(0.5, n0.EndValue);
            Assert.AreEqual(-0.5, n1.StartValue);
            Assert.AreEqual(0.495, n1.EndValue);
            Assert.AreEqual(-0.495, n2.StartValue);
            Assert.AreEqual(0.5075, n2.EndValue);
            Assert.AreEqual(-0.5075, n3.StartValue);
            Assert.AreEqual(0.505, n3.EndValue);
        }
	    [TestMethod]
	    public void UnitChangeValueTests()
	    {
		    var n0 = _domain.CreateNumberByValues(0, 20);
		    var n1 = _domain.CreateNumberByValues(20, 0);
		    var n2 = _domain.CreateNumberByValues(-30, 20);
		    var n3 = _domain.CreateNumberByValues(-20, -30);
		    Assert.AreEqual(0, n0.StartValue);
		    Assert.AreEqual(20, n0.EndValue);
		    Assert.AreEqual(20, n1.StartValue);
		    Assert.AreEqual(0, n1.EndValue);
		    Assert.AreEqual(-30, n2.StartValue);
		    Assert.AreEqual(20, n2.EndValue);
		    Assert.AreEqual(-20, n3.StartValue); 
		    Assert.AreEqual(-30, n3.EndValue);
		    _unitFocal.StartTickPosition = 100; // unot perspective
		    _unitFocal.EndTickPosition = 0; 
		    Assert.AreEqual(1, n0.StartValue);
		    Assert.AreEqual(1, n0.EndValue);
		    Assert.AreEqual(-1, n1.StartValue);
		    Assert.AreEqual(3, n1.EndValue);
		    Assert.AreEqual(1, n2.StartValue);
		    Assert.AreEqual(-2, n2.EndValue);
		    Assert.AreEqual(-4, n3.StartValue);
		    Assert.AreEqual(-1, n3.EndValue);
        }

        [TestMethod]
	    public void CoreNumberTests()
	    {
		    var f0 = FocalRef.CreateByValues(_trait, 0, 20);
		    var n0 = new Number(_domain, f0.Id);
		    var f1 = FocalRef.CreateByValues(_trait, 0, 30);
		    var n1 = new Number(_domain, f1.Id);
		    var f2 = FocalRef.CreateByValues(_trait, -32, 0);
		    var n2 = new Number(_domain, f2.Id);
		    var f3 = FocalRef.CreateByValues(_trait, -50, 45);
		    var n3 = new Number(_domain, f3.Id);
		    var f4 = FocalRef.CreateByValues(_trait, 50, -45);
		    var n4 = new Number(_domain, f4.Id);
		    var f5 = FocalRef.CreateByValues(_trait, 53, 69);
		    var n5 = new Number(_domain, f5.Id);

		    Assert.AreEqual(MathElementKind.Number, n0.Kind);
		    var n0b = n0.Clone();
		    Assert.AreEqual(n0b, n0);
		    n0b.Add(n3);
		    Assert.AreNotEqual(n0, n0b);

		    Assert.AreEqual(_unitFocal.AbsLengthInTicks, n0.AbsBasisTicks);
		    Assert.AreEqual(1, n0.Direction);
		    Assert.AreEqual(-1, n4.Direction);
		    Assert.AreEqual(n1.DomainId, n0.Domain.Id);
		    Assert.AreEqual(n0.StartValue, n1.StartValue, MathF.tolerance);
		    Assert.AreEqual(n1.EndValue, n1.Focal.LengthInTicks / (double) n1.BasisTicks);
		    Assert.AreEqual(0, n3.RemainderStartValue, MathF.tolerance);
		    Assert.AreEqual(5, n3.RemainderEndValue, MathF.tolerance);
		    Assert.AreEqual(2, n2.RemainderStartValue, MathF.tolerance);
		    Assert.AreEqual(0, n2.RemainderEndValue, MathF.tolerance);

		    Assert.AreEqual(3, n2.CeilingRange.Start, MathF.tolerance);
		    Assert.AreEqual(0, n2.CeilingRange.End, MathF.tolerance);
		    Assert.AreEqual(5, n3.FloorRange.Start, MathF.tolerance);
		    Assert.AreEqual(4, n3.FloorRange.End, MathF.tolerance);
		    Assert.AreEqual(-5, n5.RoundedRange.Start, MathF.tolerance);
		    Assert.AreEqual(7, n5.RoundedRange.End, MathF.tolerance);
		    Assert.AreEqual(-0.3, n5.RemainderRange.Start, MathF.tolerance);
		    Assert.AreEqual(0.9, n5.RemainderRange.End, MathF.tolerance);

		    Assert.IsFalse(n3.IsUnit);
		    Assert.IsTrue(_domain.BasisNumber.IsUnit);
		    Assert.IsTrue(n4.IsUnitPerspective);
		    Assert.IsFalse(n4.IsUnotPerspective);
		    Assert.AreEqual(0.472636, n3.RangeInMinMax.Start, MathF.tolerance);
		    Assert.AreEqual(0.519900, n3.RangeInMinMax.End, MathF.tolerance);

		    n1.Subtract(n2);
		    Assert.AreEqual(new Range(-3.2, 3), n1.Value);
		    n2.Add(n3);
		    Assert.AreEqual(new Range(5, 4.5), n3.Value);
		    n3.Multiply(n4);
		    Assert.AreEqual(new Range(-45, 4.8), n3.Value);
		    n4.Divide(n5);
		    Assert.AreEqual(new Range(-0.8, -0.1), n4.Value);
	    }
    }

}
