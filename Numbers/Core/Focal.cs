using System.Windows.Forms;
using Numbers.Views;

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
        FocalPositions FocalPositions { get; set; }
        void Reset(long start, long end);
        void Reset(IFocal focal);
        Range RangeAsBasis(IFocal nonBasis);

        Range GetRangeWithBasis(IFocal basis, bool isReciprocal);
        void SetWithRangeAndBasis(Range range, IFocal basis, bool isReciprocal);

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
	    public long NonZeroLength => LengthInTicks == 0 ? 1 : LengthInTicks;

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

	    public FocalPositions FocalPositions
	    {
		    get => new FocalPositions(this);
		    set
		    {
			    StartTickPosition = value.StartTickPosition;
			    EndTickPosition = value.EndTickPosition;
		    }
	    }
    
	    //public Range GetRangeWithBasis(IFocal basis) => GetRange(basis, false);
	    //public Range GetRangeWithReciprocalBasis(IFocal basis) => GetRange(basis, true);
        //public void SetWithRangeAndBasis(Range range, IFocal basis) => SetWithRange(range, basis, false);
        //public void SetWithRangeAndReciprocalBasis(Range range, IFocal basis) => SetWithRange(range, basis, true);
        public Range GetRangeWithBasis(IFocal basis, bool isReciprocal)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);    
            var basisDir = basis.Direction;
            var start = (StartTickPosition - basis.StartTickPosition) / len * basisDir;
            var end = (EndTickPosition - basis.StartTickPosition) / len * basisDir;
            if (isReciprocal)
            {
	            start = Math.Round(start) * len;
	            end = Math.Round(end) * len;
            }
            return new Range(-start, end);
        }
        public void SetWithRangeAndBasis(Range range, IFocal basis, bool isReciprocal)
        {
            var len = isReciprocal ? 1.0 : Math.Abs(basis.NonZeroLength);
            var start = basis.StartTickPosition + range.Start * len;
            var end = basis.StartTickPosition + range.End * len;
            var basisDir = basis.Direction;
            StartTickPosition = (long)Math.Round(-start * basisDir);
            EndTickPosition = (long)Math.Round(end * basisDir);
        }

        public Range RangeAsBasis(IFocal nonBasis) => nonBasis.GetRangeWithBasis(this, false);// RangeAsBasis(nonBasis.StartTickPosition, nonBasis.EndTickPosition);
        //public Range RangeAsBasis(long startTickPosition, long endTickPosition)
        //{
	       // var len = Math.Abs(NonZeroLength);
	       // var start = (startTickPosition - StartTickPosition) / len;
	       // var end = (endTickPosition - StartTickPosition) / len;
	       // var basisDir = Math.Sign(len);
	       // return new Range(-start * basisDir, end * basisDir);
        //}

        //public void SetWithRange(Range range) => SetWithRange(range, this);
        //public void SetWithRange(Range range, IFocal basis)
        //{
	       // var len = (double)(basis.AbsLengthInTicks);
	       // var sp = (long) (-range.Start * len + basis.StartTickPosition);
	       // var ep = (long) (range.End * len + basis.StartTickPosition);
        //    StartTickPosition = sp;
	       // EndTickPosition = ep;
        //}


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

	    public override string ToString() =>  $"[{StartTickPosition}->{EndTickPosition}]";
    }

    public class FocalPositions
    {
	    public long StartTickPosition { get; set; }
	    public long EndTickPosition { get; set; }

	    public FocalPositions(IFocal focal)
	    {
		    StartTickPosition = focal.StartTickPosition;
		    EndTickPosition = focal.EndTickPosition;
        }

	    public long Length => EndTickPosition - StartTickPosition;
    }
}
