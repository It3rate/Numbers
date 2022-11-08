using System.Windows.Forms;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IFocal : IMathElement
    {
	    long StartTickValue { get; set; }
	    long EndTickValue { get; set; }
	    long LengthInTicks { get; }
	    Pointing Direction { get; }
	    RatioSeg RatioIn(Domain domain);
    }

    public class Focal : IFocal
    {
	    public MathElementKind Kind => MathElementKind.Focal;
	    public int Id { get; }
	    private static int focalCounter = 1 + (int)MathElementKind.Focal;
	    // can be dealt with by expanding resolution (mult all) or range (add)
	    //bool startPrecisionUnderflow; 
	    //bool endPrecisionUnderflow;
	    //bool startRangeOverflow;
	    //bool endRangeOverflow;

	    //public Trait Trait { get; }
	    public int StartId { get; set; } // ref to start point value
	    public int EndId { get; set; } // ref to end point value
	    public long StartTickValue
	    {
		    get => Trait.ValueStore[StartId];
		    set => Trait.ValueStore[StartId] = value;
	    }
	    public long EndTickValue
        {
		    get => Trait.ValueStore[EndId];
		    set => Trait.ValueStore[EndId] = value;
	    }
	    public long LengthInTicks => EndTickValue - StartTickValue;

	    public Focal(int startId, int endId)
	    {
		    //Trait = trait;
		    StartId = startId;
		    EndId = endId;
		    Id = focalCounter++;
        }
        public Pointing Direction => StartTickValue < EndTickValue ? Pointing.Left : Pointing.Right;
        public RatioSeg RatioIn(Domain domain)
        {
	        var mx = domain.MaxRange;
	        var start = (-StartTickValue - mx.StartTickValue) / (float)(mx.LengthInTicks);
	        var end = (EndTickValue - mx.StartTickValue) / (float)(mx.LengthInTicks);
            return new RatioSeg(start, end);
        }
    }

    public class RatioSeg
    {
	    public float Start { get; set; }
	    public float End { get; set; }

	    public RatioSeg(float start, float end)
	    {
		    Start = start;
		    End = end;
	    }
    }
}
