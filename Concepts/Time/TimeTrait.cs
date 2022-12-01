using NumbersCore.Primitives;

namespace Concepts.Traits
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TimeTrait : Trait
    {
	    public override string Name => "Time";

	    public TimeTrait(Brain brain) : base(brain) {}
    }
}
