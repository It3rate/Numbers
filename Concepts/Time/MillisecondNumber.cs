using System.Dynamic;
using System.Runtime.CompilerServices;
using NumbersCore.Primitives;

namespace Concepts.Time
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MillisecondNumber : Number
    {
	    private MillisecondTimeDomain MillisecondTimeDomain => (MillisecondTimeDomain) Domain;

	    protected MillisecondNumber(IFocal focal) : base(focal) { }

	    public static MillisecondNumber Create(long duration, bool addToStore = false) => Create(0, duration, addToStore);
	    
	    public static MillisecondNumber Create(long delay, long duration, bool addToStore = false)
        {
	        var focal = NumbersCore.Primitives.Focal.CreateZeroFocal(duration);
	        var result = new MillisecondNumber(focal);
	        Knowledge.Instance.MillisecondTimeDomain.AddNumber(result, addToStore);
	        return result;
        }

        public static MillisecondNumber Zero(bool addToStore = false) => new MillisecondNumber(NumbersCore.Primitives.Focal.CreateZeroFocal(0));
    }
}
