using System.Windows.Forms;

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
        void Reset(long start, long end);
        void Reset(IFocal focal);
        Range RangeWithBasis(IFocal basis);
        Range RangeAsBasis(IFocal nonBasis);
        Range UnitTRangeIn(IFocal basis);
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

	    public void Reset(long start, long end)
	    {
		    StartTickPosition = start;
		    EndTickPosition = end;
	    }
	    public void Reset(IFocal focal)
	    {
            Reset(focal.StartTickPosition, focal.EndTickPosition);
	    }

        public Range RangeWithBasis(IFocal basis)
	    {
		    var len = (double) (basis.NonZeroLength);
            var start = (basis.StartTickPosition - StartTickPosition) / len;
		    var end = (EndTickPosition - basis.StartTickPosition) / len;
		    return new Range(start, end);
        }

        public Range RangeAsBasis(IFocal nonBasis) => RangeAsBasis(nonBasis.StartTickPosition, nonBasis.EndTickPosition);
        public Range RangeAsBasis(long startTickPosition, long endTickPosition)
        {
	        var len = (double)(NonZeroLength);
	        var start = (startTickPosition - StartTickPosition) / len;
	        var end = (endTickPosition - StartTickPosition) / len;
	        return new Range(-start, end);
        }

	    public Range UnitTRangeIn(IFocal basis)
	    {
		    var len = (double)Math.Abs(basis.NonZeroLength);
		    var start = (StartTickPosition - basis.StartTickPosition) / len;
		    var end = (EndTickPosition - basis.StartTickPosition) / len;
		    return new Range(start, end);
	    }

        public IFocal Clone()
	    {
		    return CreateByValues(MyTrait, StartTickPosition, EndTickPosition);
        }
	    public override bool Equals(object obj)
	    {
		    return obj is FocalRef other && Equals(other);
	    }
	    public bool Equals(FocalRef value)
	    {
		    return StartTickPosition.Equals(value.StartTickPosition) && EndTickPosition.Equals(value.EndTickPosition);
	    }

	    public override int GetHashCode()
	    {
		    unchecked
		    {
			    var hashCode = StartTickPosition.GetHashCode();
			    hashCode = (hashCode * 397) ^ EndTickPosition.GetHashCode();
			    return hashCode;
		    }
	    }
    }
}
