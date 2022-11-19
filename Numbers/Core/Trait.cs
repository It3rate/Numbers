using System.Numerics;
using Numbers.Mind;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Traits are measurable properties on objects. They can be composed of multiple domains (like mph or dollars/day or lwh) but do not need to be.
    /// </summary>
    public class Trait : IMathElement
    {
        public Brain _brain = Brain.BrainA;

	    public MathElementKind Kind => MathElementKind.Trait;
        public int Id { get; }
        public string Name { get; private set; }

        public Dictionary<int, Transform> TransformStore => _brain.TransformStore;
        public Dictionary<int, Trait> TraitStore => _brain.TraitStore;

        public Dictionary<int, long> PositionStore { get; } = new Dictionary<int, long>(4096);
        public Dictionary<int, FocalRef> FocalStore { get; } = new Dictionary<int, FocalRef>();
        public Dictionary<int, Domain> DomainStore { get; } = new Dictionary<int, Domain>();

	    public Trait()
	    {
		    Id = Brain.BrainA.NextTraitId();
            TraitStore.Add(Id, this);
	    }

	    public Transform AddTransform(Selection selection, Number repeats, TransformKind kind)
	    {
            var result = new Transform(selection, repeats, kind);
            TransformStore.Add(result.Id, result);
            return result;
	    }

	    public Domain AddDomain(int basisIndex, int rangeIndex)
	    {
		    var result = new Domain(Id, basisIndex, rangeIndex);
		    DomainStore.Add(result.Id, result);
		    return result;
	    }
	    public Domain AddDomain(FocalRef basis, FocalRef range)
	    {
		    return AddDomain(basis.Id, range.Id);
	    }
    }
}
