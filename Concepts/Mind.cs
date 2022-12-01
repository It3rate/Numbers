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

    public class Mind
    {
	    public Brain Brain { get; }

	    public Mind(Brain brain)
	    {
		    Brain = brain;
            Initialize();
	    }

	    private void Initialize()
	    {
            // these typed domains will be loaded from a file or something.
		    TimeTrait = new TimeTrait(Brain);
		    MillisecondTimeDomain = new MillisecondTimeDomain(this);
        }

	    public TimeTrait TimeTrait { get; private set; }
	    public MillisecondTimeDomain MillisecondTimeDomain { get; private set; }

    }
}
