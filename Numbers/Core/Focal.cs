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
	    public int StartId { get; set; } // ref to start point value
	    public int EndId { get; set; } // ref to end point value
	    public long StartTickValue
	    {
		    get => Trait.Values[StartId];
		    set => Trait.Values[StartId] = value;
	    }
	    public long EndTickValue
        {
		    get => Trait.Values[EndId];
		    set => Trait.Values[EndId] = value;
	    }
	    public long LengthTicks => EndTickValue - StartTickValue;

        // A unit tick is always positive direction (greater than zero). A unot is a unit flipped around zero, so same length pointing in opposite direction.
	    public long UnitTick => StartTickValue >= EndTickValue ? StartTickValue : EndTickValue;
	    public long ZeroTick => StartTickValue >= EndTickValue ? EndTickValue : StartTickValue;
	    public long UnotTick => ZeroTick - UnitTick;
        public long UnitLengthTicks => Math.Abs(EndTickValue - StartTickValue);

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
