﻿
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

        public virtual long StartPosition { get; set; }
        public virtual long EndPosition { get; set; }
        public virtual long InvertedEndPosition => StartPosition - LengthInTicks;

        public int Direction => StartPosition <= EndPosition ? 1 : -1;
        public bool IsPositiveDirection => Direction >= 0;

        // todo: add a digits per tick value to disconnect tick size from underlying representation
        public long LengthInTicks => EndPosition - StartPosition;
        public long AbsLengthInTicks => Math.Abs(LengthInTicks);
        public long NonZeroLength => LengthInTicks == 0 ? 1 : LengthInTicks;
        public virtual long AlignedEndPosition(bool isAligned) => isAligned ? EndPosition : InvertedEndPosition;
        public virtual long AlignedLengthInTicks(bool isAligned) => AlignedEndPosition(isAligned) - StartPosition;
        public long AlignedNonZeroLength(bool isAligned)
        {
            var result = AlignedLengthInTicks(isAligned);
            return result == 0 ? (isAligned ? 1 : -1) : result;
        }

        /// <summary>
        /// Focals are pre-number segments, not value interpretations.
        /// Whatever orientation they have is considered the aligned direction, and the inverted value is the 'other' direction.
        /// Even a negative basis focal is still 'unit', and its invert (positive in this case) is unot. The basis focal decides the aligned direction.
        /// </summary>
        private Focal()
        {
            Id = _focalCounter++;
        }
        public Focal(long startTickPosition, long endTickPosition) : this()
        {
            StartPosition = startTickPosition;
            EndPosition = endTickPosition;
        }


        public static Focal CreateZeroFocal(long ticks) { return new Focal(0, ticks); }
        public static Focal CreateBalancedFocal(long halfTicks) { return new Focal(-halfTicks, halfTicks); }
        private static Focal _minMaxFocal;
        public static Focal MinMaxFocal => _minMaxFocal ?? (_minMaxFocal = new Focal(long.MinValue, long.MaxValue));
        private static Focal _upMaxFocal;
        public static Focal UpMaxFocal => _upMaxFocal ?? (_upMaxFocal = new Focal(0, long.MaxValue));






        public void Reset(long start, long end)
        {
            StartPosition = start;
            EndPosition = end;
        }
        public void Reset(Focal focal)
        {
            Reset(focal.StartPosition, focal.EndPosition);
        }
        public void InvertBasis()
        {
            EndPosition = StartPosition - LengthInTicks;
        }

        // todo: rework reciprocal to live in a tickSize property in Numbers.
        public Range GetRangeWithBasis(Focal basis, bool isReciprocal, bool isAligned)
        {
            var len = (double)basis.NonZeroLength * (isAligned ? 1 : -1); //AlignedNonZeroLength(isAligned);// 
            var start = (StartPosition - basis.StartPosition) / len;
            var end = (EndPosition - basis.StartPosition) / len;
            if (isReciprocal)
            {
                start = Math.Round(start) * Math.Abs(len);
                end = Math.Round(end) * Math.Abs(len);
            }
            var result = new Range(-start, end, isAligned);
            return result;
        }
        public void SetWithRangeAndBasis(Range range, Focal basis, bool isReciprocal)
        {
            var len = (double)basis.NonZeroLength * (range.IsAligned ? 1 : -1);
            var z = basis.StartPosition;
            var start = z - range.Start * len;
            var end = z + range.End * len;
            if (isReciprocal)
            {
                start = Math.Round(start) / Math.Abs(len);
                end = Math.Round(end) / Math.Abs(len);
            }
            StartPosition = (long)Math.Round(start);
            EndPosition = (long)Math.Round(end);
        }
        public Range RangeAsBasis(Focal nonBasis) => nonBasis.GetRangeWithBasis(this, false, true);
        public Range UnitTRangeIn(Focal basis)
        {
            var len = (double)Math.Abs(basis.NonZeroLength);
            var start = (StartPosition - basis.StartPosition) / len;
            var end = (EndPosition - basis.StartPosition) / len;
            return new Range(start, end);
        }
        public long Min => StartPosition <= EndPosition ? StartPosition : EndPosition;
        public long Max => StartPosition >= EndPosition ? StartPosition : EndPosition;
        public Focal Negated => new Focal(-StartPosition, -EndPosition);

        public static long MinPosition(Focal p, Focal q) => Math.Min(p.Min, q.Min);
        public static long MaxPosition(Focal p, Focal q) => Math.Max(p.Max, q.Max);
        public static long MinStart(Focal p, Focal q) => Math.Min(p.StartPosition, q.StartPosition);
        public static long MaxStart(Focal p, Focal q) => Math.Max(p.StartPosition, q.StartPosition);
        public static long MinEnd(Focal p, Focal q) => Math.Min(p.EndPosition, q.EndPosition);
        public static long MaxEnd(Focal p, Focal q) => Math.Max(p.EndPosition, q.EndPosition);
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
	        if (p.StartPosition == 0 && p.EndPosition == long.MaxValue)
	        {
		        return new Focal[] { };
	        }
	        // If p starts at the beginning of the time frame and ends before the end, the "not A" relationship consists of a single interval from p.EndTickPosition + 1 to the end of the time frame
	        else if (p.StartPosition == 0)
	        {
		        return new Focal[] { new Focal(p.EndPosition + 1, long.MaxValue) };
	        }
	        // If p starts after the beginning of the time frame and ends at the end, the "not A" relationship consists of a single interval from the beginning of the time frame to p.StartTickPosition - 1
	        else if (p.EndPosition == long.MaxValue)
	        {
		        return new Focal[] { new Focal(0, p.StartPosition - 1) };
	        }
	        // If p starts and ends within the time frame, the "not A" relationship consists of two intervals: from the beginning of the time frame to p.StartTickPosition - 1, and from p.EndTickPosition + 1 to the end of the time frame
	        else
	        {
		        return new Focal[] { new Focal(0, p.StartPosition - 1), new Focal(p.EndPosition + 1, long.MaxValue) };
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
	        if (p.EndPosition < q.StartPosition - 1 || q.EndPosition < p.StartPosition - 1)
	        {
		        return new Focal[] { p };
	        }
	        else
	        {
		        return new Focal[] { new Focal(p.StartPosition, q.StartPosition - 1) };
	        }
        }
        public static Focal[] Transfer_A(Focal p, Focal q)
        {
	        return new Focal[] { p };
        }
        public static Focal[] A_Inhibits_B(Focal p, Focal q)
        {
	        if (p.EndPosition < q.StartPosition - 1 || q.EndPosition < p.StartPosition - 1)
	        {
		        return new Focal[] { q };
	        }
	        else
	        {
		        return new Focal[] { new Focal(q.StartPosition, p.StartPosition - 1) };
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
	        if (p.EndPosition < q.StartPosition - 1 || q.EndPosition < p.StartPosition - 1)
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
	        if (p.EndPosition < q.StartPosition - 1 || q.EndPosition < p.StartPosition - 1)
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
                    new Focal(long.MinValue, overlap.StartPosition),
                    new Focal(overlap.EndPosition, long.MaxValue)
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




        public Focal Clone()
        {
            return new Focal(StartPosition, EndPosition);
        }
        public override bool Equals(object obj)
        {
            return obj is Focal other && Equals(other);
        }
        public bool Equals(Focal value)
        {
            return StartPosition.Equals(value.StartPosition) && EndPosition.Equals(value.EndPosition);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StartPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EndPosition.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => $"[{StartPosition} : {EndPosition}]";
    }

}
