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
        public bool InStore { get; }

        public Focal(Trait trait, long startTickPosition, long endTickPosition, bool addToStore) : base(trait)
        {
	        StartTickPosition = startTickPosition;
	        EndTickPosition = endTickPosition;
	        InStore = addToStore;
		    if (addToStore)
		    {
			    trait.FocalStore.Add(Id, this);
            }
        }
	    public static Focal CreateByValues(Trait trait, long startPosition, long endPosition, bool addToStore)
	    {
		    var result = new Focal(trait, startPosition, endPosition, addToStore);
		    return result;
	    }

	    public override IFocal Clone()
	    {
		    return CreateByValues(MyTrait, StartTickPosition, EndTickPosition, InStore);
	    }

    }
}
