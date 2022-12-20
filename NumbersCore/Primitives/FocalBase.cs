
using System;
using System.Collections.Generic;
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

        public static long MinStart(IFocal p, IFocal q) => MinStart(p, q);
        public static long MaxStart(IFocal p, IFocal q) => Math.Max(p.StartTickPosition, q.StartTickPosition);
        public static long MinEnd(IFocal p, IFocal q) => Math.Min(p.EndTickPosition, q.EndTickPosition);
        public static long MaxEnd(IFocal p, IFocal q) => Math.Max(p.EndTickPosition, q.EndTickPosition);

        // gtpChat generated

        public static IFocal[] Zero(IFocal p, IFocal q)
        {
            return new IFocal[0];
        }
        public static IFocal[] And(IFocal p, IFocal q)
        {
            if (p.EndTickPosition < q.StartTickPosition || q.EndTickPosition < p.StartTickPosition)
            {
                return new IFocal[0];
            }
            else
            {
                return new IFocal[] { new Focal(MaxStart(p, q), MinEnd(p, q)) };
            }
        }

        public static IFocal[] Xor(IFocal p, IFocal q)
        {
            // Return the symmetric difference of the two input segments as a new array of segments
            List<IFocal> result = new List<IFocal>();
            IFocal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the segments do not intersect, return the segments as separate non-overlapping segments
                result.Add(p);
                result.Add(q);
            }
            else
            {
                // If the segments intersect, return the complement of the intersection in each segment
                IFocal[] complement1 = Nor(p, andResult[0]);
                IFocal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static IFocal[] Or(IFocal p, IFocal q)
        {
            if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
            {
                return new IFocal[] { p, q };
            }
            else
            {
                return new IFocal[] { new Focal(MinStart(p, q), MaxEnd(p, q)) };
            }
        }
        public static IFocal[] Nor(IFocal p, IFocal q)
        {
            // Return the complement of the union of the two input IFocals as a new array of IFocals
            List<IFocal> result = new List<IFocal>();
            IFocal[] orResult = Or(p, q);
            if (orResult.Length == 0)
            {
                // If the IFocals do not overlap, return both IFocals as separate non-overlapping IFocals
                result.Add(p);
                result.Add(q);
            }
            else
            {
                // If the IFocals overlap, return the complement of the union in each IFocal
                IFocal[] complement1 = Nor(p, orResult[0]);
                IFocal[] complement2 = Nor(q, orResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static IFocal[] Xnor(IFocal p, IFocal q)
        {
            // Return the complement of the symmetric difference of the two input IFocals as a new array of IFocals
            List<IFocal> result = new List<IFocal>();
            IFocal[] xorResult = Xor(p, q);
            if (xorResult.Length == 0)
            {
                // If the IFocals are equal, return p as a single IFocal
                result.Add(p);
            }
            else
            {
                // If the IFocals are not equal, return the complement of the symmetric difference in each IFocal
                IFocal[] complement1 = Nor(p, xorResult[0]);
                IFocal[] complement2 = Nor(q, xorResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }

        public static IFocal[] Nand(IFocal p, IFocal q)
        {
            // Return the complement of the intersection of the two input IFocals as a new array of IFocals
            List<IFocal> result = new List<IFocal>();
            IFocal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the IFocals do not intersect, return the union of the IFocals as a single IFocal
                result.Add(new Focal(MinStart(p, q), MaxEnd(p, q)));
            }
            else
            {
                // If the IFocals intersect, return the complement of the intersection in each IFocal
                IFocal[] complement1 = Nor(p, andResult[0]);
                IFocal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static IFocal[] Always(IFocal p, IFocal q)
        {
            return new IFocal[] { new Focal(long.MinValue, long.MaxValue) };
        }

        public static IFocal[] Not(IFocal p, IFocal q)
        {
            if (p.EndTickPosition < q.StartTickPosition - 1 || q.EndTickPosition < p.StartTickPosition - 1)
            {
                // p and q do not overlap, return them both
                return new IFocal[] { p, q };
            }
            else
            {
                // p and q overlap, return the "not" of the overlap
                if (p.StartTickPosition < q.StartTickPosition)
                {
                    // p starts before q, return segment from p.Start to q.Start
                    return new IFocal[] { new Focal(p.StartTickPosition, q.StartTickPosition - 1) };
                }
                else
                {
                    // q starts before p, return segment from q.End to p.End
                    return new IFocal[] { new Focal(q.EndTickPosition + 1, p.EndTickPosition) };
                }
            }
        }


        public static IFocal[] OrNot(IFocal p, IFocal q)
        {
            // Return the union of the two input IFocals, with the complement of q in p
            List<IFocal> result = new List<IFocal>();
            IFocal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the IFocals do not intersect, return the union of the IFocals as a single IFocal
                result.Add(new Focal(MinStart(p, q), MaxEnd(p, q)));
            }
            else
            {
                // If the IFocals intersect, return the complement of the intersection in p and q
                IFocal[] complement1 = Nor(p, andResult[0]);
                IFocal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static IFocal[] Transfer(IFocal p, IFocal q)
        {
            // Return the complement of the intersection of the two input IFocals as a new array of IFocals
            return Nor(p, q);
        }
        public static IFocal[] Implication(IFocal p, IFocal q)
        {
            // Return the union of the two input IFocals, with the complement of p in q
            List<IFocal> result = new List<IFocal>();
            IFocal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the IFocals do not intersect, return the union of the IFocals as a single IFocal
                result.Add(new Focal(MinStart(p, q), MaxEnd(p, q)));
            }
            else
            {
                // If the IFocals intersect, return the complement of the intersection in p and q
                IFocal[] complement1 = Nor(p, andResult[0]);
                IFocal[] complement2 = Nor(q, andResult[0]);
                result.AddRange(complement1);
                result.AddRange(complement2);
            }
            return result.ToArray();
        }
        public static IFocal[] AndNot(IFocal p, IFocal q)
        {
            // Return the difference of p and q as a new array of IFocals
            List<IFocal> result = new List<IFocal>();
            IFocal[] andResult = And(p, q);
            if (andResult.Length == 0)
            {
                // If the IFocals do not intersect, return p as a single IFocal
                result.Add(p);
            }
            else
            {
                // If the IFocals intersect, return the complement of the intersection in p
                IFocal[] complement = Nor(p, andResult[0]);
                result.AddRange(complement);
            }
            return result.ToArray();
        }


        public long Zero(long a, long b) { return 0; } // Null
        public long And(long a, long b) { return a & b; }
        public long A_GreaterThan_B(long a, long b) { return a & ~b; } // inhibition, div a/b. ab`
        public long Transfer_A(long a, long b) { return a; }                   // transfer
        public long B_GreaterThan_A(long a, long b) { return ~a & b; } // inhibition, div b/a. a`b
        public long Transfer_B(long a, long b) { return b; }                   // transfer
        public long Xor(long a, long b) { return a ^ b; }
        public long Or(long a, long b) { return a | b; }

        public long Nor(long a, long b) { return ~(a | b); }
        public long Xnor(long a, long b) { return ~(a ^ b); } // equivalence, ==. (xy)`
        public long Not_B(long a, long b) { return ~b; } // complement
        public long A_Implies_B(long a, long b) { return a | ~b; } // implication (Not B alone)
        public long Not_A(long a, long b) { return ~a; } // complement
        public long B_Implies_A(long a, long b) { return b | ~a; } // implication (Not A alone)
        public long Nand(long a, long b) { return ~(a & b); }
        public long Identity(long a, long b) { return -1; }





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
