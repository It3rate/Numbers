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
    public class WorkspaceTests
    {
	    private Brain MyBrain => Brain.ActiveBrain;
	    private Workspace _workspace;
	    private Trait _trait;
        private FocalRef _unitFocal;
	    private FocalRef _maxMin;
	    private Domain _domain;

	    [TestInitialize]
	    public void Init()
	    {
            _workspace = new Workspace();

		    _trait = new Trait();
		    _unitFocal = FocalRef.CreateByValues(_trait, -4, 6);
		    _maxMin = FocalRef.CreateByValues(_trait, -54, 46);
		    _domain = new Domain(_trait.Id, _unitFocal.Id, _maxMin.Id);
	    }

	    [TestMethod]
	    public void CoreWorkspaceTests()
	    {
            _workspace.AddDomains(true, _domain);
            Assert.AreEqual(_workspace.ActiveElementCount, 3); // domain, unit and range

            var f5 = FocalRef.CreateByValues(_trait, 20, 90);
            var n5 = new Number(_domain, f5.Id);
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
