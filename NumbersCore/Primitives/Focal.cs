
using System;
using System.Collections.Generic;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    // todo: Create a value only (nonRef) focal
    //public interface Focal : IMathElement
    //{
    //    long StartTickPosition { get; set; }
    //    long EndTickPosition { get; set; }
    //    long LengthInTicks { get; }
    //    long AbsLengthInTicks { get; }
    //    long NonZeroLength { get; }
    //    int Direction { get; }
    //    bool IsUnitPerspective { get; }
    //    bool IsUnotPerspective { get; }
    //    long Min { get; }
    //    long Max { get; }

    //    FocalPositions FocalPositions { get; set; }
    //    void Reset(long start, long end);
    //    void Reset(Focal focal);

    //    void FlipAroundStartPoint();

    //    Range RangeAsBasis(Focal nonBasis);
    //    Range GetRangeWithBasis(Focal basis, bool isReciprocal);
    //    void SetWithRangeAndBasis(Range range, Focal basis, bool isReciprocal);

    //    Range UnitTRangeIn(Focal basis);
    //    Focal Clone();
    //}

    public class Focal
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
        private Focal()
        {
            Id = _focalCounter++;
        }
        public Focal(long startTickPosition, long endTickPosition) : this()
        {
            StartTickPosition = startTickPosition;
            EndTickPosition = endTickPosition;
        }
        public static Focal CreateByValues(long startPosition, long endPosition)
        {
            var result = new Focal(startPosition, endPosition);
            return result;
        }

        public Focal Clone()
        {
            return CreateByValues(StartTickPosition, EndTickPosition);
        }

        public static Focal CreateZeroFocal(long ticks) { return Focal.CreateByValues(0, ticks); }
        public static Focal CreateBalancedFocal(long halfTicks) { return Focal.CreateByValues(-halfTicks, halfTicks); }
        private static Focal _minMaxFocal;
        public static Focal MinMaxFocal => _minMaxFocal ?? (_minMaxFocal = Focal.CreateByValues(long.MinValue, long.MaxValue));
        private static Focal _upMaxFocal;
        public static Focal UpMaxFocal => _upMaxFocal ?? (_upMaxFocal = Focal.CreateByValues(0, long.MaxValue));






        public void Reset(long start, long end)
        {
            StartTickPosition = start;
            EndTickPosition = end;
        }
        public void Reset(Focal focal)
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

        public void InvertBasis()
        {
            EndTickPosition = StartTickPosition - LengthInTicks;
            //var len = LengthInTicks;
            //var sp = StartTickPosition;
            //var ep = EndTickPosition;
            //if(orgPolarity == Alignment.Aligned)
            //{
            //    StartTickPosition = ep;
            //    EndTickPosition = ep + len;
            //}
            //else
            //{
            //    StartTickPosition = sp - len;
            //    EndTickPosition = sp;
            //}
        }
        public Range GetRangeWithBasis(Focal basis, bool isReciprocal, bool isAligned)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);
            var start = (StartTickPosition - basis.StartTickPosition) / len;
            var end = (EndTickPosition - basis.StartTickPosition) / len;
            if (isReciprocal)
            {
                start = Math.Round(start) * len;
                end = Math.Round(end) * len;
            }
            var result = basis.IsUnitPerspective ? new Range(-start, end) : new Range(end, -start);
            return isAligned ? result : new Range(-result.Start, -result.End);
        }
        public void SetWithRangeAndBasis(Range range, Focal basis, bool isReciprocal, bool isAligned)
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

            var stp = (long)Math.Round(start);
            var etp = (long)Math.Round(end);
            StartTickPosition = isAligned ? stp : -stp;
            EndTickPosition = isAligned ? etp : -etp;
        }
        public Range RangeAsBasis(Focal nonBasis) => nonBasis.GetRangeWithBasis(this, false, true);
        public Range UnitTRangeIn(Focal basis)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);
            var start = (StartTickPosition - basis.StartTickPosition) / len;
            var end = (EndTickPosition - basis.StartTickPosition) / len;
            return new Range(start, end);
        }
        public long Min => StartTickPosition <= EndTickPosition ? StartTickPosition : EndTickPosition;
        public long Max => StartTickPosition >= EndTickPosition ? StartTickPosition : EndTickPosition;

        public static long MinPosition(Focal p, Focal q) => Math.Min(p.Min, q.Min);
        public static long MaxPosition(Focal p, Focal q) => Math.Max(p.Max, q.Max);
        public static long MinStart(Focal p, Focal q) => Math.Min(p.StartTickPosition, q.StartTickPosition);
        public static long MaxStart(Focal p, Focal q) => Math.Max(p.StartTickPosition, q.StartTickPosition);
        public static long MinEnd(Focal p, Focal q) => Math.Min(p.EndTickPosition, q.EndTickPosition);
        public static long MaxEnd(Focal p, Focal q) => Math.Max(p.EndTickPosition, q.EndTickPosition);
        public static Focal Overlap(Focal p, Focal q)
        {
	        var start = Math.Max(p.Min, q.Min);
	        var end = Math.Min(p.Max, q.Max);
	        return (start >= end) ? new Focal(0, 0) : new Focal(start, end);
        }
        public static Focal Extent(Focal p, Focal q)
        {
	        var start = Math.Min(p.Min, q.Min);
	        var end = Math.Max(p.Max, q.Max);
	        return new Focal(start, end);
        }

        // Q. Should direction be preserved in a bool operation?
        public static Focal[] Never(Focal p)
        {
	        return new Focal[0];
        }
        public static Focal[] Not(Focal p)
        {
	        // If p starts at the beginning of the time frame and ends at the end, A is always true and the "not A" relationship is empty
	        if (p.StartTickPosition == 0 && p.EndTickPosition == long.MaxValue)
	        {
		        return new Focal[] { };
	        }
	        // If p starts at the beginning of the time frame and ends before the end, the "not A" relationship consists of a single interval from p.EndTickPosition + 1 to the end of the time frame
	        else if (p.StartTickPosition == 0)
	        {
		        return new Focal[] { new Focal(p.EndTickPosition + 1, long.MaxValue) };
	        }
	        // If p starts after the beginning of the time frame and ends at the end, the "not A" relationship consists of a single interval from the beginning of the time frame to p.StartTickPosition - 1
	        else if (p.EndTickPosition == long.MaxValue)
	        {
		        return new Focal[] { new Focal(0, p.StartTickPosition - 1) };
	        }
	        // If p starts and ends within the time frame, the "not A" relationship consists of two intervals: from the beginning of the time frame to p.StartTickPosition - 1, and from p.EndTickPosition + 1 to the end of the time frame
	        else
	        {
		        return new Focal[] { new Focal(0, p.StartTickPosition - 1), new Focal(p.EndTickPosition + 1, long.MaxValue) };
	        }
        }
        public static Focal[] Transfer(Focal p)
        {
	        return new Focal[] { p.Clone() };
        }
        public static Focal[] Always(Focal p)
        {
	        return new Focal[] { Focal.MinMaxFocal.Clone()};
        }

        public static Focal[] Never(Focal p, Focal q)
        {
            return new Focal[0];
        }
        public static Focal[] And(Focal p, Focal q)
        {
	        var overlap = Overlap(p, q);
	        return (overlap.LengthInTicks == 0) ? new Focal[0] : new Focal[] {overlap};
        }
        public static Focal[] B_Inhibits_A(Focal p, Focal q)
        {
	        if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
	        {
		        return new Focal[] { p };
	        }
	        else
	        {
		        return new Focal[] { new Focal(p.StartTickPosition, q.StartTickPosition - 1) };
	        }
        }
        public static Focal[] Transfer_A(Focal p, Focal q)
        {
	        return new Focal[] { p };
        }
        public static Focal[] A_Inhibits_B(Focal p, Focal q)
        {
	        if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
	        {
		        return new Focal[] { q };
	        }
	        else
	        {
		        return new Focal[] { new Focal(q.StartTickPosition, p.StartTickPosition - 1) };
	        }
        }
        public static Focal[] Transfer_B(Focal p, Focal q)
        {
	        return new Focal[] { q };
        }
        public static Focal[] Xor(Focal p, Focal q)
        {
            // Return the symmetric difference of the two input segments as a new array of segments
            List<Focal> result = new List<Focal>();
            Focal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the segments do not intersect, return the segments as separate non-overlapping segments
                result.Add(p);
                result.Add(q);
            }
            else
            {
                // If the segments intersect, return the complement of the intersection in each segment
                Focal[] complement1 = Nor(p, andResult[0]);
                Focal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static Focal[] Or(Focal p, Focal q)
        {
	        var overlap = Overlap(p, q);
	        return (overlap.LengthInTicks == 0) ? new Focal[] {p, q} : new Focal[] {Extent(p, q)};
        }
        public static Focal[] Nor(Focal p, Focal q)
        {
            // Return the complement of the union of the two input Focals as a new array of Focals
            List<Focal> result = new List<Focal>();
            Focal[] orResult = Or(p, q);
            if (orResult.Length == 0)
            {
                // If the Focals do not overlap, return both Focals as separate non-overlapping Focals
                result.Add(p);
                result.Add(q);
            }
            else
            {
                // If the Focals overlap, return the complement of the union in each Focal
                Focal[] complement1 = Nor(p, orResult[0]);
                Focal[] complement2 = Nor(q, orResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static Focal[] Xnor(Focal p, Focal q)
        {
            // Return the complement of the symmetric difference of the two input Focals as a new array of Focals
            List<Focal> result = new List<Focal>();
            Focal[] xorResult = Xor(p, q);
            if (xorResult.Length == 0)
            {
                // If the Focals are equal, return p as a single Focal
                result.Add(p);
            }
            else
            {
                // If the Focals are not equal, return the complement of the symmetric difference in each Focal
                Focal[] complement1 = Nor(p, xorResult[0]);
                Focal[] complement2 = Nor(q, xorResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static Focal[] Not_B(Focal p, Focal q)
        {
	        return Not(q);
        }
        public static Focal[] B_Implies_A(Focal p, Focal q)
        {
	        if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
	        {
		        return new Focal[] { };
	        }
	        else
	        {
		        return new Focal[] { new Focal(MaxStart(p, q), MinEnd(p, q)) };
	        }
        }
        public static Focal[] Not_A(Focal p, Focal q)
        {
	        return Not(p);
        }
        public static Focal[] A_Implies_B(Focal p, Focal q)
        {
	        if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
	        {
		        return new Focal[] { };
	        }
	        else
	        {
		        return new Focal[] { new Focal(MaxStart(p, q), MinEnd(p, q)) };
	        }
        }
        public static Focal[] Nandx(Focal p, Focal q)
        {
            // Return the complement of the intersection of the two input Focals as a new array of Focals
            List<Focal> result = new List<Focal>();
            Focal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the Focals do not intersect, return the union of the Focals as a single Focal
                result.Add(new Focal(MinStart(p, q), MaxEnd(p, q)));
            }
            else
            {
                // If the Focals intersect, return the complement of the intersection in each Focal
                Focal[] complement1 = Nor(p, andResult[0]);
                Focal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static Focal[] Nand(Focal p, Focal q)
        {
            Focal[] result;
            var overlap = Overlap(p, q);
            if(overlap.LengthInTicks == 0)
            {
                result = new Focal[] { Focal.MinMaxFocal.Clone() };
            }
            else
            {
                result = new Focal[]
                { 
                    new Focal(long.MinValue, overlap.StartTickPosition),
                    new Focal(overlap.EndTickPosition, long.MaxValue)
                };
            }
            return result;
        }
        public static Focal[] Always(Focal p, Focal q)
        {
            return new Focal[] { Focal.MinMaxFocal.Clone() };
        }


        //  0000	Never			0			FALSE
        //  0001	Both            A ^ B       AND
        //  0010	Only A          A ^ !B      A AND NOT B
        //  0011	A Maybe B       A           A
        //  0100	Only B			!A ^ B      NOT A AND B
        //  0101	B Maybe A       B           B
        //  0110	One of          A xor B     XOR
        //  0111	At least one    A v B       OR
        //  1000	No one          A nor B     NOR
        //  1001	Both or no one  A XNOR B    XNOR
        //  1010	A or no one		!B          NOT B
        //  1011	Not B alone     A v !B      A OR NOT B
        //  1100	B or no one		!A          NOT A
        //  1101	Not A alone		!A v B      NOT A OR B
        //  1110	Not both        A nand B    NAND
        //  1111	Always			1			TRUE

        public long Never(long a) { return 0; } // Null
        public long Transfer(long a) { return a; }
        public long Not(long a) { return ~a; }
        public long Identity(long a) { return -1; }

        public long Never(long a, long b) { return 0; } // Null
        public long And(long a, long b) { return a & b; }
        public long B_Inhibits_A(long a, long b) { return a & ~b; } // inhibition, div a/b. ab`
        public long Transfer_A(long a, long b) { return a; }                   // transfer
        public long A_Inhibits_B(long a, long b) { return ~a & b; } // inhibition, div b/a. a`b
        public long Transfer_B(long a, long b) { return b; }                   // transfer
        public long Xor(long a, long b) { return a ^ b; }
        public long Or(long a, long b) { return a | b; }
        public long Nor(long a, long b) { return ~(a | b); }
        public long Xnor(long a, long b) { return ~(a ^ b); } // equivalence, ==. (xy)`
        public long Not_B(long a, long b) { return ~b; } // complement
        public long B_Implies_A(long a, long b) { return a | ~b; } // implication (Not B alone)
        public long Not_A(long a, long b) { return ~a; } // complement
        public long A_Implies_B(long a, long b) { return b | ~a; } // implication (Not A alone)
        public long Nand(long a, long b) { return ~(a & b); }
        public long Always(long a, long b) { return -1; }

        public long Equals(long a, long b) { return ~(a ^ b); } // xnor
        public long NotEquals(long a, long b) { return a ^ b; } // xor
        public long GreaterThan(long a, long b) { return ~a & b; } // b > a
        public long LessThan(long a, long b) { return a & ~b; } // a > b
        public long GreaterThanOrEqual(long a, long b) { return ~a | b; } // a implies b
        public long LessThanOrEqual(long a, long b) { return a | ~b; } // b implies a




        public override bool Equals(object obj)
        {
            return obj is Focal other && Equals(other);
        }
        public bool Equals(Focal value)
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

        public FocalPositions(Focal focal)
        {
            StartTickPosition = focal.StartTickPosition;
            EndTickPosition = focal.EndTickPosition;
        }

        public long Length => EndTickPosition - StartTickPosition;
    }
}
