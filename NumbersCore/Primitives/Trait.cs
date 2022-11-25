using System.Collections.Generic;

namespace NumbersCore.Primitives
{
	/// <summary>
    /// Traits are measurable properties on objects. They can be composed of multiple domains (like mph or dollars/day or lwh) but do not need to be.
    /// </summary>
    public class Trait : IMathElement
    {
	    public Brain MyBrain => Brain.ActiveBrain;
        public MathElementKind Kind => MathElementKind.Trait;
        public int Id { get; }
        public string Name { get; private set; }

        public Dictionary<int, Transform> TransformStore => MyBrain.TransformStore;
        public Dictionary<int, Trait> TraitStore => MyBrain.TraitStore;
        public Dictionary<int, long> PositionStore { get; } = new Dictionary<int, long>(4096);
        public Dictionary<int, FocalRef> FocalStore { get; } = new Dictionary<int, FocalRef>();
        public Dictionary<int, Domain> DomainStore { get; } = new Dictionary<int, Domain>();

        public Trait()
	    {
		    Id = MyBrain.NextTraitId();
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
		    return result;
	    }
	    public Domain AddDomain(FocalRef basis, FocalRef range)
	    {
		    return AddDomain(basis.Id, range.Id);
	    }
	    public IEnumerable<Domain> Domains()
	    {
		    foreach (var domain in DomainStore.Values)
		    {
			    yield return domain;
		    }
	    }
	    public IEnumerable<Transform> Transforms()
	    {
		    foreach (var transform in TransformStore.Values)
		    {
			    yield return transform;
		    }
	    }
    }
}
