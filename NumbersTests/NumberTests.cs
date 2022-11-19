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
    public class NumberTests
    {
	    public static double _err = 0.00001;

        [TestMethod]
	    public void CoreNumberTests()
	    {
		    var trait = new Trait();
		    var uf = FocalRef.CreateByValues(trait, 0, 10);
		    var mr = FocalRef.CreateByValues(trait, -1000, 1010);
		    var d = new Domain(trait.Id, uf.Id, mr.Id);

		    var f0 = FocalRef.CreateByValues(trait, 0, 20);
		    var n0 = new Number(d, f0.Id);
		    var f1 = FocalRef.CreateByValues(trait, 0, 30);
		    var n1 = new Number(d, f1.Id);
		    var f2 = FocalRef.CreateByValues(trait, -32, 0);
		    var n2 = new Number(d, f2.Id);
		    var f3 = FocalRef.CreateByValues(trait, -50, 45);
		    var n3 = new Number(d, f3.Id);
		    var f4 = FocalRef.CreateByValues(trait, 50, -45);
		    var n4 = new Number(d, f4.Id);
		    var f5 = FocalRef.CreateByValues(trait, 53, 69);
		    var n5 = new Number(d, f5.Id);

            Assert.AreEqual(n0.Kind, MathElementKind.Number);
            var n0b = n0.Clone();
            Assert.AreEqual(n0, n0b);
            n0b.Add(n3);
            Assert.AreNotEqual(n0, n0b);

           Assert.AreEqual(uf.AbsLengthInTicks, n0.AbsBasisTicks);
		    Assert.AreEqual(1, n0.Direction);
		    Assert.AreEqual(-1, n4.Direction);
		    Assert.AreEqual(n1.DomainId, n0.Domain.Id);
            Assert.AreEqual(n0.StartValue, n1.StartValue, _err);
            Assert.AreEqual(n1.EndValue, n1.Focal.LengthInTicks / (double)n1.BasisTicks);
            Assert.AreEqual(n3.RemainderStartValue, 0, _err);
            Assert.AreEqual(n3.RemainderEndValue, 5, _err);
            Assert.AreEqual(n2.RemainderStartValue, 2, _err);
            Assert.AreEqual(n2.RemainderEndValue, 0, _err);

            Assert.AreEqual(n2.CeilingRange.Start, 3, _err);
            Assert.AreEqual(n2.CeilingRange.End, 0, _err);
            Assert.AreEqual(n3.FloorRange.Start, 5, _err);
            Assert.AreEqual(n3.FloorRange.End, 4, _err);
            Assert.AreEqual(n5.RoundedRange.Start, -5, _err);
            Assert.AreEqual(n5.RoundedRange.End, 7, _err);
            Assert.AreEqual(n5.RemainderRange.Start, -0.3, _err);
            Assert.AreEqual(n5.RemainderRange.End, 0.9, _err);

            Assert.IsFalse(n3.IsUnit);
            Assert.IsTrue(d.BasisNumber.IsUnit);
            Assert.IsTrue(n4.IsUnitPerspective);
            Assert.IsFalse(n4.IsUnotPerspective);
            Assert.AreEqual(n3.RangeInMinMax.Start, -0.472636, _err);
            Assert.AreEqual(n3.RangeInMinMax.End, 0.519900, _err);

            n1.Subtract(n2);
            Assert.AreEqual(n1.Value, new Range(-3.2, 3));
            n2.Add(n3);
            Assert.AreEqual(n3.Value, new Range(5, 4.5));
            n3.Multiply(n4);
            Assert.AreEqual(n3.Value, new Range(-45,4.8));
            n4.Divide(n5);
            Assert.AreEqual(n4.Value, new Range(-0.8, -0.1));

        }
    }

}
