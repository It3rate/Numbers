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
	    private Brain _brain;
	    private Workspace _workspace;
	    private Trait _trait;
        private FocalRef _unitFocal;
	    private FocalRef _maxMin;
	    private Domain _domain;

	    [TestInitialize]
	    public void Init()
	    {
            _brain = Brain.BrainA;
            _workspace = new Workspace(_brain);

		    _trait = new Trait();
		    _unitFocal = FocalRef.CreateByValues(_trait, -4, 6);
		    _maxMin = FocalRef.CreateByValues(_trait, -54, 46);
		    _domain = new Domain(_trait.Id, _unitFocal.Id, _maxMin.Id);
	    }

	    [TestMethod]
	    public void CoreWorkspaceTests()
	    {
	    }
    }
}
