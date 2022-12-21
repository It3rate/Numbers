using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NumberSet
    {
	    public MathElementKind Kind => MathElementKind.NumberSet;  

        public Brain Brain => Trait?.Brain;
	    public virtual Trait Trait => Domain?.Trait;
	    public virtual Domain Domain { get; set; }
	    public int DomainId
	    {
		    get => Domain.Id;
		    set => Domain = Domain.Trait.DomainStore[value];
	    }
        private List<IFocal> Focals { get; } = new List<IFocal>();

        public NumberSet(Domain domain, params IFocal[] focals)
        {
	        Domain = domain;
	        Focals.AddRange(focals);
        }

        public IFocal[] GetFocals() => Focals.ToArray();
        public int Count() => Focals.Count;
        public void Add(IFocal focal) => Focals.Add(focal);
        public void Remove(IFocal focal) => Focals.Remove(focal);

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
