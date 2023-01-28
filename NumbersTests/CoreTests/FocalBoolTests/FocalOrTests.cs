using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersCore.Primitives;

namespace NumbersTests.CoreTests.FocalBoolTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class FocalOrTests
    {
        [TestMethod]
        public void NoOverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(30, 40);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(p, result[0]);
            Assert.AreEqual(q, result[1]);
        }

        [TestMethod]
        public void OverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(15, 25);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(1, result.Length);
            CollectionAssert.Contains(result, new Focal(10, 25));
        }
        [TestMethod]
        public void OverlapTestOrder1()
        {
            IFocal p = new Focal(15, 25);
            IFocal q = new Focal(10, 20);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(1, result.Length);
            CollectionAssert.Contains(result, new Focal(10, 25));
        }
        [TestMethod]
        public void OverlapTestOrder2()
        {
            IFocal p = new Focal(25, 15);
            IFocal q = new Focal(10, 20);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(1, result.Length);
            CollectionAssert.Contains(result, new Focal(10, 25));
        }
        [TestMethod]
        public void OverlapTestOrder3()
        {
            IFocal p = new Focal(25, 15);
            IFocal q = new Focal(20, 10);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(1, result.Length);
            CollectionAssert.Contains(result, new Focal(10, 25));
        }
        [TestMethod]
        public void OverlapTestDirection()
        {
            IFocal p = new Focal(15, 25);
            IFocal q = new Focal(20, 10);
            IFocal[] result = FocalBase.Or(p, q);
            Assert.AreEqual(1, result.Length);
            CollectionAssert.Contains(result, new Focal(10, 25));
        }

    }

}
