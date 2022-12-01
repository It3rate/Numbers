using Concepts.Time;
using Concepts.Traits;
using NumbersCore.Primitives;

namespace Concepts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Knowledge
    {
	    public Brain Brain { get; }

	    public Knowledge(Brain brain)
	    {
		    Brain = brain;
            Initialize();
	    }

	    private void Initialize()
	    {
            // these typed domains will be loaded from a file or something.
		    TimeTrait = TimeTrait.CreateIn(this);
		    MillisecondTimeDomain = new MillisecondTimeDomain(this);
        }

	    public TimeTrait TimeTrait { get; private set; }
	    public MillisecondTimeDomain MillisecondTimeDomain { get; private set; }

    }
}
