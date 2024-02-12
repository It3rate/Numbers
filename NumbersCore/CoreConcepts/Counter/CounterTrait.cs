using NumbersCore.CoreConcepts.Time;
using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Counter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CounterTrait : Trait
    {
	    public override string Name => "Repeats";

	    public static CounterTrait CreateIn(Knowledge knowledge) => (CounterTrait)knowledge.Brain.AddTrait(new CounterTrait());
    }
}
