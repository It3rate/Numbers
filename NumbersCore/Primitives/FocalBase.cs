
using System;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    // todo: Create a value only (nonRef) focal
    public interface IFocal : IMathElement
    {
        long StartTickPosition { get; set; }
        long EndTickPosition { get; set; }
        long LengthInTicks { get; }
        long AbsLengthInTicks { get; }
        long NonZeroLength { get; }
        int Direction { get; }
        bool IsUnitPerspective { get; }
        bool IsUnotPerspective { get; }

        FocalPositions FocalPositions { get; set; }
        void Reset(long start, long end);
        void Reset(IFocal focal);

        void FlipAroundStartPoint();

        Range RangeAsBasis(IFocal nonBasis);
        Range GetRangeWithBasis(IFocal basis, bool isReciprocal);
        void SetWithRangeAndBasis(Range range, IFocal basis, bool isReciprocal);

        Range UnitTRangeIn(IFocal basis);
        IFocal Clone();
    }

    public abstract class FocalBase : IFocal
    {
        public MathElementKind Kind => MathElementKind.Focal;
        public int Id { get; }
        private static int _focalCounter = 1 + (int)MathElementKind.Focal;
        public int CreationIndex => Id - (int)Kind - 1;

        //private static int focalCounter = 1 + (int)MathElementKind.Focal;
        //public int StartId { get; set; } // ref to start point position
        //public int EndId { get; set; } // ref to end point position

        // can be dealt with by expanding resolution (mult all) or range (add)
        //bool startPrecisionUnderflow; 
        //bool endPrecisionUnderflow;
        //bool startRangeOverflow;
        //bool endRangeOverflow;

        public virtual long StartTickPosition { get; set; }
        public virtual long EndTickPosition { get; set; }

        public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;
        public bool IsUnitPerspective => StartTickPosition <= EndTickPosition;
        public bool IsUnotPerspective => StartTickPosition > EndTickPosition;

        public long LengthInTicks => EndTickPosition - StartTickPosition;
        public long AbsLengthInTicks => Math.Abs(LengthInTicks);
        public long NonZeroLength => LengthInTicks == 0 ? 1 : LengthInTicks;

        // Focal values are always added as unit perspective positions, because
        // there is no unit defined that allows the start point to be interpreted as a unot perspective.
        // Focals are pre-number, positions, not value interpretations.
        protected FocalBase()
        {
            Id = _focalCounter++;
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

        public void FlipAroundStartPoint()
        {
            EndTickPosition = StartTickPosition - LengthInTicks;
        }

        //public Range GetRangeWithBasis(IFocal basis) => GetRange(basis, false);
        //public Range GetRangeWithReciprocalBasis(IFocal basis) => GetRange(basis, true);
        //public void SetWithRangeAndBasis(Range range, IFocal basis) => SetWithRange(range, basis, false);
        //public void SetWithRangeAndReciprocalBasis(Range range, IFocal basis) => SetWithRange(range, basis, true);
        public Range GetRangeWithBasis(IFocal basis, bool isReciprocal)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);
            //var basisDir = basis.Direction;
            var start = (StartTickPosition - basis.StartTickPosition) / len;
            var end = (EndTickPosition - basis.StartTickPosition) / len;
            if (isReciprocal)
            {
                start = Math.Round(start) * len;
                end = Math.Round(end) * len;
            }
            return basis.IsUnitPerspective ? new Range(-start, end) : new Range(end, -start);
        }
        public void SetWithRangeAndBasis(Range range, IFocal basis, bool isReciprocal)
        {
            double start;
            double end;
            var len = (double)basis.NonZeroLength;
            var zeroTick = basis.StartTickPosition;
            if (basis.IsUnitPerspective)
            {
                start = zeroTick - range.Start * len;
                end = zeroTick + range.End * len;
            }
            else
            {
                start = zeroTick + range.End * len;
                end = zeroTick - range.Start * len;
            }

            if (isReciprocal)
            {
                start = Math.Round(start) / Math.Abs(len);
                end = Math.Round(end) / Math.Abs(len);
            }

            StartTickPosition = (long)Math.Round(start);
            EndTickPosition = (long)Math.Round(end);
        }

        public Range RangeAsBasis(IFocal nonBasis) => nonBasis.GetRangeWithBasis(this, false);

        public Range UnitTRangeIn(IFocal basis)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);
            var start = (StartTickPosition - basis.StartTickPosition) / len;
            var end = (EndTickPosition - basis.StartTickPosition) / len;
            return new Range(start, end);
        }

        public abstract IFocal Clone();

        // May need to override these in the base classes? Probably shouldn't care it a focal is a ref or value.
        public override bool Equals(object obj)
        {
            return obj is IFocal other && Equals(other);
        }
        public bool Equals(IFocal value)
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

        public override string ToString() => $"[{StartTickPosition}->{EndTickPosition}]";
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
