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
	    [TestMethod]
	    public void CoreDomainTests()
	    {
		    var trait = new Trait();
		    var uf = FocalRef.CreateByValues(trait, -4, 6);
		    var mr = FocalRef.CreateByValues(trait, -54, 46);
            var d = new Domain(trait.Id, uf.Id, mr.Id);

            Assert.AreEqual(d.Kind, MathElementKind.Domain);
            Assert.AreEqual(d.MinMaxFocal.LengthInTicks, 100);
            Assert.AreEqual(d.UnitFocal.LengthInTicks, 10);
            Assert.AreEqual(uf.Id, d.UnitFocal.Id);
            Assert.AreEqual(mr.Id, d.MinMaxFocal.Id);
            Assert.AreEqual(d.MinMaxRange.Length, 10.0, _err);

            var f0 = FocalRef.CreateByValues(trait, 10, 20);
            var n0 = new Number(d, f0.Id);
            var n1 = new Number(d, f0.Clone().Id);
            var n2 = new Number(d, f0.Clone().Id);
            Assert.AreEqual(d.NumberIds.Count, 4); // includes unit basis
            
            var dict = new Dictionary<int, Range>();

            d.SaveNumberValues(dict, d.UnitNumberId);
            Assert.AreEqual(dict.Count, 3); // does not include unit
            var saved = n1.Focal.EndTickPosition;
            n1.Focal.EndTickPosition = 22;
            Assert.AreNotEqual(n1.Focal.EndTickPosition, saved);
            Assert.AreEqual(n1.Focal.EndTickPosition, 22);
            d.RestoreNumberValues(dict);
            Assert.AreEqual(n1.Focal.EndTickPosition, saved);

        }
    }
}
