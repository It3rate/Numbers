using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a boolean type evaluation of two or more numbers.
    /// This type of number can have more than one segment, but segments can never overlap.
    /// All Numbers are probably NumberSets, with zero based Unit and Unot values cancelling on overlap.
    /// </summary>
    public class NumberSet : Number, IMathElement
    { 
        // todo: subclass number, and use base focal as the total segment, then internally divided into the masked sections.'
        // should be able to access internal focals as numbers, and there may in fact be none (A and B with no overlap)
        // should be able to access and update proportionally, where 8 even subdivisions will remain so even when changing the base number.
        // Q. are internal focals able to have polarity? probably not?
        // these focals seem similar to steps when sectioning a transition (repeated adds are steps, repeated mult powers are steps), intervals
        // seems they can be recorded as on/off/on/off along the line, preventing potential overlaps you might get in focals.
        // seems a good way to encode repetition and even counting.
	    public override MathElementKind Kind => MathElementKind.NumberSet;

        // todo: Focals should have no overlap and always be sorted
        private List<Focal> Focals { get; } = new List<Focal>(); 

        public NumberSet(Number targetNumber, params Focal[] focals) : base(targetNumber.Focal, targetNumber.Polarity)
        {
            Domain = targetNumber.Domain;
	        Focals.AddRange(focals);
            RemoveOverlaps();
        }

        public Focal[] GetFocals() => Focals.ToArray();

        public int Count => Focals.Count;
        public void Add(Focal focal) { Focals.Add(focal); RemoveOverlaps(); }
        public void Remove(Focal focal) => Focals.Remove(focal);

        public void RemoveOverlaps()
        {
            if (Focals.Count > 1)
            {
                List<Focal> result = new List<Focal>();
                // Sort the list by start tick position
                Focals.Sort((a, b) => a.StartPosition.CompareTo(b.StartPosition));

                long start = Focals[0].StartPosition;
                long end = Focals[0].EndPosition;
                for (int i = 1; i < Focals.Count; i++)
                {
                    // Check for overlap
                    if (Focals[i].StartPosition <= end)
                    {
                        // Overlap, merge the ranges
                        end = Math.Max(end, Focals[i].EndPosition);
                    }
                    else
                    {
                        // No overlap, add the current non-overlapping range to the result list
                        result.Add(new Focal(start, end));
                        start = Focals[i].StartPosition;
                        end = Focals[i].EndPosition;
                    }
                }
                result.Add(new Focal(start, end));

                Focals.Clear();
                Focals.AddRange(result);
            }
        }

        public void Reset(Focal[] focals)
        {
            Focals.Clear();
            Focals.AddRange(focals);
        }
        public Number this[int index] => index < Focals.Count ? Domain.CreateNumber(Focals[index], false) : null;
        public IEnumerable<Number> Numbers()
        {
	        foreach (var focal in Focals)
	        {
		        yield return Domain.CreateNumber(focal, false);
	        }
        }

        public Number SumAll()
        {
            var result = Domain.CreateNumber(new Focal(0,0));
            foreach (var number in Numbers())
            {
                result.Add(number);
            }
            return result;
        }


    }
}
