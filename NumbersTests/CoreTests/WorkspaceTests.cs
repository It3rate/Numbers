using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersCore.Primitives;

namespace NumbersTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [TestClass]
    public class WorkspaceTests
    {
	    private Brain Brain => Brain.ActiveBrain;
	    private Workspace _workspace;
	    private Trait _trait;
        private FocalRefx _unitFocal;
	    private FocalRefx _maxMin;
	    private Domain _domain;

	    [TestInitialize]
	    public void Init()
	    {
            _workspace = new Workspace(Brain);

		    _trait = new Trait(Brain);
		    _unitFocal = FocalRefx.CreateByValues(_trait, -4, 6);
		    _maxMin = FocalRefx.CreateByValues(_trait, -54, 46);
		    _domain = new Domain(_trait, _unitFocal, _maxMin);
	    }

	    [TestMethod]
	    public void CoreWorkspaceTests()
	    {
            _workspace.AddDomains(true, _domain);
            Assert.AreEqual(_workspace.ActiveElementCount, 3); // domain, unit and range

            var f5 = FocalRefx.CreateByValues(_trait, 20, 90);
            var n5 = new Number(_domain, f5);
            _workspace.AddElements(n5);
            Assert.AreEqual(4, _workspace.ActiveElementCount);
            _workspace.ClearAll();
            Assert.AreEqual( 0, _workspace.ActiveElementCount);
            _workspace.AddDomains(true, _domain);
            Assert.AreEqual(4, _workspace.ActiveElementCount);
            _workspace.RemoveDomains(true, _domain);
            Assert.AreEqual( 0, _workspace.ActiveElementCount);
            _workspace.AddDomains(false, _domain);
            Assert.AreEqual( 1, _workspace.ActiveElementCount);
            _workspace.AddDomains(true, _domain);
            Assert.AreEqual( 4, _workspace.ActiveElementCount);
            _workspace.RemoveTraits(true, _trait);
            Assert.AreEqual(0, _workspace.ActiveElementCount);
        }
    }
}
