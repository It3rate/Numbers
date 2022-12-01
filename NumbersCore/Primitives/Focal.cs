using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Focal : FocalBase
    {
        public Focal(long startTickPosition, long endTickPosition)
        {
	        StartTickPosition = startTickPosition;
	        EndTickPosition = endTickPosition;
        }
	    public static Focal CreateByValues(long startPosition, long endPosition)
	    {
		    var result = new Focal(startPosition, endPosition);
		    return result;
	    }

	    public override IFocal Clone()
	    {
		    return CreateByValues(StartTickPosition, EndTickPosition);
	    }

	    public static IFocal CreateZeroFocal(long ticks) { return Focal.CreateByValues(0, ticks); }
        public static IFocal CreateBalancedFocal(long halfTicks) { return Focal.CreateByValues(-halfTicks, halfTicks); }
	    private static IFocal _maxFocal;
	    public static IFocal MaxFocal => _maxFocal ?? (_maxFocal = Focal.CreateByValues(long.MinValue, long.MaxValue));

    }
}
