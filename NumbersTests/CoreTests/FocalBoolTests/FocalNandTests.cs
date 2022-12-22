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
    public class FocalNandTests
    {
        [TestMethod]
        public void NoOverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(30, 40);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Focal.MinMaxFocal, result[0]);
        }

        [TestMethod]
        public void OverlapTest()
        {
            IFocal p = new Focal(10, 20);
            IFocal q = new Focal(15, 25);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, new Focal(long.MinValue, 15));
            CollectionAssert.Contains(result, new Focal(20, long.MaxValue));
        }
        [TestMethod]
        public void OverlapTestOrder1()
        {
            IFocal p = new Focal(15, 25);
            IFocal q = new Focal(10, 20);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, new Focal(long.MinValue, 15));
            CollectionAssert.Contains(result, new Focal(20, long.MaxValue));
        }
        [TestMethod]
        public void OverlapTestOrder2()
        {
            IFocal p = new Focal(25, 15);
            IFocal q = new Focal(10, 20);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, new Focal(long.MinValue, 15));
            CollectionAssert.Contains(result, new Focal(20, long.MaxValue));
        }
        [TestMethod]
        public void OverlapTestOrder3()
        {
            IFocal p = new Focal(25, 15);
            IFocal q = new Focal(20, 10);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, new Focal(long.MinValue, 15));
            CollectionAssert.Contains(result, new Focal(20, long.MaxValue));
        }
        [TestMethod]
        public void OverlapTestDirection()
        {
            IFocal p = new Focal(15, 25);
            IFocal q = new Focal(20, 10);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(2, result.Length);
            CollectionAssert.Contains(result, new Focal(long.MinValue, 15));
            CollectionAssert.Contains(result, new Focal(20, long.MaxValue));
        }

        [TestMethod]
        public void qBeforePTest()
        {
            IFocal p = new Focal(20, 30);
            IFocal q = new Focal(10, 15);
            IFocal[] result = FocalBase.Nand(p, q);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(Focal.MinMaxFocal, result[0]);
        }
    }

}
