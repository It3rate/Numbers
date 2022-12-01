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
	    public override long StartTickPosition { get; set; }
	    public override long EndTickPosition { get; set; }

        public Focal(Trait trait, long startTickPosition, long endTickPosition) : base(trait)
        {
	        StartTickPosition = startTickPosition;
	        EndTickPosition = endTickPosition;
        }
	    public static Focal CreateByValues(Trait trait, long startPosition, long endPosition)
	    {
		    var result = new Focal(trait, startPosition, endPosition);
		    return result;
	    }

	    public override IFocal Clone()
	    {
		    return CreateByValues(MyTrait, StartTickPosition, EndTickPosition);
	    }

    }
}
