using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersCore.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumbersTests.CoreTests.FocalBoolTests
{
    [TestClass]
    public class FocalNotTests
    {
        [TestMethod]
        public void NoOverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(30, 40);
            IFocal[] result = FocalBase.Not(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, p);
            CollectionAssert.Contains(result, q);
        }

        [TestMethod]
        public void OverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(15, 25);
            IFocal[] result = FocalBase.Not(p, q);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(10, result[0].StartTickPosition);
            Assert.AreEqual(14, result[0].EndTickPosition);
        }

        [TestMethod]
        public void qBeforePTest()
        {
            IFocal p = new Focal(20, 30);
            IFocal q = new Focal(10, 15);
            IFocal[] result = FocalBase.Not(p, q);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(16, result[0].StartTickPosition);
            Assert.AreEqual(30, result[0].EndTickPosition);
        }
    }

}
