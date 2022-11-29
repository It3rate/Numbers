using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class FocalVal : FocalBase
    {
	    public override long StartTickPosition { get; set; }
	    public override long EndTickPosition { get; set; }

        public FocalVal(Trait trait, long startTickPosition, long endTickPosition) : base(trait)
        {
	        StartTickPosition = startTickPosition;
	        EndTickPosition = endTickPosition;
        }
	    public static FocalVal CreateByValues(Trait trait, long startPosition, long endPosition)
	    {
		    var result = new FocalVal(trait, startPosition, endPosition);
		    trait.FocalStore.Add(result.Id, result);
		    return result;
	    }

	    public override IFocal Clone()
	    {
		    return CreateByValues(MyTrait, StartTickPosition, EndTickPosition);
	    }

    }
}
