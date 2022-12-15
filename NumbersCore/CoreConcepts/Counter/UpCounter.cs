using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Counter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpCounter : Number
    {
	    public override Domain Domain
	    {
		    get => CounterDomain.UpDomain;
		    set { }
	    }

	    protected UpCounter() : base(new Focal(0, 0)) { }

    }
}
