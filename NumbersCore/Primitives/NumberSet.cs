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
        private List<IFocal> Focals { get; } = new List<IFocal>(); // todo: Focals should have no overlap and always be sorted

        public NumberSet(Domain domain, params IFocal[] focals)
        {
	        Domain = domain;
	        Focals.AddRange(focals);
        }

        public IFocal[] GetFocals() => Focals.ToArray();
        public int Count => Focals.Count;
        public void Add(IFocal focal) => Focals.Add(focal);
        public void Remove(IFocal focal) => Focals.Remove(focal);

        public void Reset(IFocal[] focals)
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


    }
}
