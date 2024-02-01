using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Counter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CounterDomain : Domain
    {
	    private CounterDomain(Focal basisFocal, Focal maxFocal) : base(new CounterTrait(), basisFocal, maxFocal) { }

	    public static CounterDomain UpDomain { get; } = new CounterDomain(Focal.CreateZeroFocal(1), new Focal(0, long.MaxValue));

        public static CounterDomain UpDownDomain { get; } = new CounterDomain(Focal.CreateZeroFocal(1), Focal.MinMaxFocal);
    }
}
