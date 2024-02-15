using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Ordered numbers, array style, clamped to base Number size. Optionally can be merged and ordered with bool Operations.
    /// All Numbers are probably NumberChain, with zero based Unit and Unot values cancelling on overlap.
    /// Can represent a boolean evaluation of two numbers that result in non overlapping ordered segments, which share polarity and domain.
    /// Can also represent the start/endpoints of mergable repeats, like 3+3+3 or 3*3*3. 
    /// </summary>
    public class NumberChain : Number, IMathElement
    { 
        // should be able to access and update proportionally, where 8 even subdivisions will remain so even when changing the base number.
        // they can be recorded as on/off/on/off along the line, allowing construction of 2D continuous paths (though this would just be an optimization).
        // knowing if focals are touching (contiguous) matters.
        // all clamping, overlap removal and repears should be done in the focalSet
	    public override MathElementKind Kind => MathElementKind.NumberChain;

        // todo: Focals should have no overlap and always be sorted
        private List<Focal> Focals { get; } = new List<Focal>(); // todo: use growing list that never exceeds max size, reuse focals

        public IEnumerable<Number> InternalNumbers()
        {
            foreach (var focal in Focals)
            {
                var nm = new Number(focal, Polarity);
                nm.Domain = Domain;
                yield return nm;
            }
        }

        public NumberChain(Number targetNumber, params Focal[] focals) : base(targetNumber.Focal, targetNumber.Polarity)
        {
            Domain = targetNumber.Domain;
	        Focals.AddRange(focals);
            RemoveOverlaps();
        }

        public Focal[] GetFocals() => Focals.ToArray();

        public int Count => Focals.Count;
        public void Add(Focal focal) { Focals.Add(focal); RemoveOverlaps(); }
        public void Remove(Focal focal) => Focals.Remove(focal);

        private void ClampToOwnFocal(Focal focal)
        {
            if (focal.StartPosition < Focal.StartPosition)
            {
                focal.StartPosition = Focal.StartPosition;
            }

            if (focal.EndPosition > Focal.EndPosition)
            {
                focal.EndPosition = Focal.EndPosition;
            }

        }
        public void RemoveOverlaps()
        {
            if (Focals.Count > 1)
            {
                List<Focal> result = new List<Focal>();
                // ensure forward pointing
                foreach (var focal in Focals)
                {
                    focal.MakeForward(); 
                }
                // Sort the list by start tick position
                Focals.Sort((a, b) => a.StartPosition.CompareTo(b.StartPosition));

                long baseStart = Focal.StartPosition;
                long baseEnd = Focal.EndPosition;
                long start = Focals[0].StartPosition;
                long end = Focals[0].EndPosition;
                for (int i = 1; i < Focals.Count; i++)
                {
                    var prevFocal = Focals[i - 1];
                    if (Focal.Intersection(Focal, prevFocal) == null)
                    {
                        continue;
                    }

                    // Check for overlap
                    if (Focals[i].StartPosition <= end)
                    {
                        // Overlap, merge the ranges
                        end = Math.Max(end, Focals[i].EndPosition);
                    }
                    else
                    {
                        var f = new Focal(start, end);
                        ClampToOwnFocal(f);
                        // No overlap, add the current non-overlapping range to the result list
                        if (f.LengthInTicks != 0)
                        {
                            result.Add(f);
                        }
                        start = Focals[i].StartPosition;
                        end = Focals[i].EndPosition;
                    }
                }

                if (start < baseEnd)
                {
                    var last = new Focal(start, end);
                    ClampToOwnFocal(last);
                    result.Add(last);
                }

                Focals.Clear();
                Focals.AddRange(result);
            }
            else if(Focals.Count == 1)
            {
                ClampToOwnFocal(Focals[0]);
            }
        }

        public void Reset(params Focal[] focals)
        {
            Focals.Clear();
            Focals.AddRange(focals);
        }
        public Number this[int index] => index < Focals.Count ? Domain.CreateNumber(Focals[index], false) : null;

        public Number SumAll()
        {
            var result = Domain.CreateNumber(new Focal(0,0));
            foreach (var number in InternalNumbers())
            {
                result.Add(number);
            }
            return result;
        }

        public override void Add(Number q) { base.Add(q); Reset(Focal); }
        public override void Subtract(Number q) { base.Subtract(q); Reset(Focal); }
        public override void Multiply(Number q) { base.Multiply(q); Reset(Focal); }
        public override void Divide(Number q) { base.Divide(q); Reset(Focal);}
        public void Not(Number q) { Reset(Focal.UnaryNot(q.Focal)); RemoveOverlaps(); }


        public override NumberChain And(Number q) { Reset(Focal.And(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Or(Number q) { Reset(Focal.Or(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Not_A(Number q) { Reset(Focal.Not_A(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Not_B(Number q) { Reset(Focal.Not_B(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Nand(Number q) { Reset(Focal.Nand(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Nor(Number q) { Reset(Focal.Nor(Focal, q.Focal)); RemoveOverlaps(); return this; }

        public override NumberChain Xnor(Number q) { Reset(Focal.Xnor(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain Xor(Number q) { Reset(Focal.Xor(Focal, q.Focal)); RemoveOverlaps();  return this; }
        public override NumberChain B_Inhibits_A(Number q) { Reset(Focal.B_Inhibits_A(Focal, q.Focal)); RemoveOverlaps(); return this; }
        public override NumberChain A_Inhibits_B(Number q) { Reset(Focal.A_Inhibits_B(Focal, q.Focal)); RemoveOverlaps(); return this; }
    }
}
