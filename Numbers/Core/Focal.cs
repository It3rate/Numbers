using System.Windows.Forms;

namespace Numbers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Focal
    {
	    private static int focalCounter = 1;
	    // can be dealt with by expanding resolution (mult all) or range (add)
	    //bool startPrecisionUnderflow; 
	    //bool endPrecisionUnderflow;
	    //bool startRangeOverflow;
	    //bool endRangeOverflow;

	    //public Trait Trait { get; }
	    public int Id { get; }
	    public int StartId { get; set; }
	    public int EndId { get; set; }

	    public Focal(int startId, int endId)
	    {
		    //Trait = trait;
		    StartId = startId;
		    EndId = endId;
		    Id = focalCounter++;
	    }
	   // public Focal(long startValue, long endValue, Trait trait) : this(trait, trait.AddValue(startValue), trait.AddValue(endValue)) { }

    }
}
