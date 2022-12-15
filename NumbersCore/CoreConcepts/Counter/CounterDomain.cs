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
	    private CounterDomain(IFocal basisFocal, IFocal maxFocal) : base(new CounterTrait(), basisFocal, maxFocal) { }

	    private static CounterDomain _upDomain;
	    public static CounterDomain UpDomain =>
		    _upDomain ?? new CounterDomain(Focal.CreateZeroFocal(1), Focal.CreateByValues(0, long.MaxValue));

	    private static CounterDomain _upDownDomain;
	    public static CounterDomain UpDownDomain =>
		    _upDownDomain ?? new CounterDomain(Focal.CreateZeroFocal(1), Focal.MinMaxFocal);
    }
}
