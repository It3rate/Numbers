﻿using System.Windows.Forms;
using Numbers.Mind;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // todo: Create a value only (nonRef) focal
    public interface IFocal : IMathElement
    {
	    long StartTickPosition { get; set; }
	    long EndTickPosition { get; set; }
	    long LengthInTicks { get; }
        long AbsLengthInTicks { get; }
        long NonZeroLength { get; }
        int Direction { get; }
        IFocal Clone();
    }

    public class FocalRef : IFocal
    {
        public MathElementKind Kind => MathElementKind.Focal;
	    public int Id { get; }
	    private static int focalCounter = 1 + (int)MathElementKind.Focal;
	    // can be dealt with by expanding resolution (mult all) or range (add)
	    //bool startPrecisionUnderflow; 
	    //bool endPrecisionUnderflow;
	    //bool startRangeOverflow;
	    //bool endRangeOverflow;

	    public Trait MyTrait {get;}
        public int StartId { get; set; } // ref to start point position
	    public int EndId { get; set; } // ref to end point position
        public long StartTickPosition
	    {
		    get => MyTrait.PositionStore[StartId];
		    set => MyTrait.PositionStore[StartId] = value;
	    }
	    public long EndTickPosition
        {
		    get => MyTrait.PositionStore[EndId];
		    set => MyTrait.PositionStore[EndId] = value;
	    }
	    public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;
	    public long LengthInTicks => EndTickPosition - StartTickPosition;
	    public long AbsLengthInTicks => Math.Abs(LengthInTicks);
	    public long NonZeroLength => EndTickPosition - StartTickPosition == 0 ? 1 : EndTickPosition - StartTickPosition;

        // Focal values are always added as unit perspective positions, because
        // there is no unit defined that allows the start point to be interpreted as a unot perspective.
        // Focals are pre-number, positions, not value interpretations.
        private FocalRef(Trait trait, int startId, int endId)
	    {
		    MyTrait = trait;
		    StartId = startId;
		    EndId = endId;
		    Id = focalCounter++;
	    }
	    public static FocalRef CreateByIds(Trait trait, int startId, int endId)
	    {
		    var result = new FocalRef(trait, startId, endId);
		    trait.FocalStore.Add(result.Id, result);
		    return result;
        }
	    public static FocalRef CreateByValues(Trait trait, long startPosition, long endPosition)
	    {
		    trait.PositionStore.Add(focalCounter++, startPosition);
		    trait.PositionStore.Add(focalCounter++, endPosition);
            var result = new FocalRef(trait, focalCounter - 2, focalCounter - 1);
            trait.FocalStore.Add(result.Id, result);
            return result;
	    }

	    public IFocal Clone()
	    {
		    return CreateByIds(MyTrait, StartId, EndId);
	    }
    }

    // todo: probably remove RatioSeg, maybe its just a complex number.
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
