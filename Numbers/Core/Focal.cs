using System.Windows.Forms;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IFocal : IMathElement
    {
	    long StartTickPosition { get; set; }
	    long EndTickPosition { get; set; }
	    long LengthInTicks { get; }
	    int Direction { get; }
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
	    public int StartId { get; set; } // ref to start point position
	    public int EndId { get; set; } // ref to end point position
        public long StartTickPosition
	    {
		    get => PositionStore[StartId];
		    set => PositionStore[StartId] = value;
	    }
	    public long EndTickPosition
        {
		    get => PositionStore[EndId];
		    set => PositionStore[EndId] = value;
	    }
	    public long LengthInTicks => EndTickPosition - StartTickPosition;
	    public long AbsLengthInTicks => Math.Abs(LengthInTicks);

        public Focal(int startId, int endId)
	    {
		    //Trait = trait;
		    StartId = startId;
		    EndId = endId;
		    Id = focalCounter++;
        }
        public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;

        public static List<long> PositionStore { get; } = new List<long>(4096);
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

	    public override string ToString()
	    {
		    return $"[{Start:0.00},{End:0.00}]";
	    }
    }
}
