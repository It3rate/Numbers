using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	/// <summary>
    /// Traits are measurable properties on objects. They can be composed of multiple domains (like mph or dollars/day or lwh) but do not need to be.
    /// </summary>
    public class Trait : IMathElement
	{
		public Brain Brain { get; }
        public MathElementKind Kind => MathElementKind.Trait;
        public int Id { get; }
        public int CreationIndex => Id - (int)Kind - 1;
        public string Name { get; private set; }

        public Dictionary<int, long> PositionStore { get; } = new Dictionary<int, long>(4096);
        public Dictionary<int, IFocal> FocalStore { get; } = new Dictionary<int, IFocal>();
        public Dictionary<int, Domain> DomainStore { get; } = new Dictionary<int, Domain>();

        public Dictionary<int, Transform> TransformStore => Brain.TransformStore;
        public Dictionary<int, Trait> TraitStore => Brain.TraitStore;

        public Trait(Brain brain, string name = "")
        {
	        Brain = brain;
		    Id = Brain.NextTraitId();
		    Name = name == "" ? "Trait_" + CreationIndex : name;
            TraitStore.Add(Id, this);
        }

        public Transform AddTransform(Selection selection, Number repeats, TransformKind kind)
	    {
            var result = new Transform(selection, repeats, kind);
            TransformStore.Add(result.Id, result);
            return result;
	    }
        private Domain AddDomain(int basisIndex, int rangeIndex)
	    {
		    var result = new Domain(this, basisIndex, rangeIndex);
		    return result;
	    }
	    public Domain AddDomain(IFocal basis, IFocal range)
	    {
		    return AddDomain(basis.Id, range.Id);
	    }
	    public Domain AddDomain(long basisTicks)
	    {
		    return AddDomain(CreateZeroFocal(basisTicks).Id, MaxFocal.Id);
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


	    public Domain DomainAt(int index)
	    {
		    var id = index + (int)MathElementKind.Domain;
		    DomainStore.TryGetValue(id, out var result);
		    return result;
	    }
	    public IFocal FocalAt(int index)
	    {
		    var id = index + (int)MathElementKind.Focal;
		    FocalStore.TryGetValue(id, out var result);
		    return result;
	    }

        public IFocal CreateZeroFocal(long ticks) { return FocalVal.CreateByValues(this, 0, ticks); }
	    public IFocal CreateBalancedFocal(long halfTicks) { return FocalVal.CreateByValues(this, -halfTicks, halfTicks); }
	    private IFocal _maxFocal;
	    public IFocal MaxFocal =>_maxFocal ?? (_maxFocal = FocalVal.CreateByValues(this, long.MinValue, long.MaxValue));
	}
}
