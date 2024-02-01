﻿using NumbersCore.Utils;

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
    /// All Numbers are probably NumberSets, with Unit being selected and Unot being unselected - may adjust.
    /// </summary>
    public class NumberSet : IMathElement
    {
	    public MathElementKind Kind => MathElementKind.NumberSet;
	    public int Id { get; internal set; }
	    public int CreationIndex => Id - (int)Kind - 1;

        public Brain Brain => Trait?.Brain;
	    public virtual Trait Trait => Domain?.Trait;
	    public virtual Domain Domain { get; set; }
	    public int DomainId
	    {
		    get => Domain.Id;
		    set => Domain = Domain.Trait.DomainStore[value];
	    }
        private List<Focal> Focals { get; } = new List<Focal>(); // todo: Focals should have no overlap and always be sorted

        public NumberSet(Domain domain, params Focal[] focals)
        {
	        Domain = domain;
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
