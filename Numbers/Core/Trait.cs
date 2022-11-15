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

	    public static List<long> ValueStore { get; } = new List<long>(4096);

	    public Dictionary<int, Focal> FocalStore { get; } = new Dictionary<int, Focal>();
        public Dictionary<int, Domain> DomainStore { get; } = new Dictionary<int, Domain>();

	    public Trait()
	    {
		    Id = Brain.BrainA.NextTraitId();
            TraitStore.Add(Id, this);
	    }

	    public int AddValue(long value)
	    {
            ValueStore.Add(value);
            return ValueStore.Count - 1;
	    }

	    public Transform AddTransform(Selection selection, Number repeats, TransformKind kind)
	    {
            var result = new Transform(selection, repeats, kind);
            TransformStore.Add(result.Id, result);
            return result;
	    }


	    public Focal CloneFocal(Focal focal)
	    {
		    return AddFocalByValues(focal.StartTickValue, focal.EndTickValue);
	    }
        public Focal AddFocalByIndexes(int startIndex, int endIndex)
	    {
		    var result = new Focal(startIndex, endIndex);
		    FocalStore.Add(result.Id, result);
		    return result;
        }
	    public Focal AddFocalByValues(long start, long end)
	    {
		    var index = ValueStore.Count;
		    ValueStore.Add(start);
		    ValueStore.Add(end);
		    return AddFocalByIndexes(index, index + 1);
	    }
	    public Focal AddFocalByIndexValue(int startIndex, long end)
	    {
		    var index = ValueStore.Count;
		    ValueStore.Add(end);
		    return AddFocalByIndexes(startIndex, index);
	    }
	    public Focal AddFocalByValueIndex(long start, int endIndex)
	    {
		    var index = ValueStore.Count;
		    ValueStore.Add(start);
		    return AddFocalByIndexes(index, endIndex);
	    }

	    public Domain AddDomain(int unitIndex, int rangeIndex)
	    {
		    var result = new Domain(Id, unitIndex, rangeIndex);
		    DomainStore.Add(result.Id, result);
		    return result;
	    }
	    public Domain AddDomain(Focal unit, Focal range)
	    {
		    return AddDomain(unit.Id, range.Id);
	    }

        // Focal Methods
	    public long Start(Focal focal) => ValueStore[focal.StartId];
	    public long End(Focal focal) => ValueStore[focal.EndId];
	    public long Ticks(Focal focal) => End(focal) - Start(focal);
	    public long RightMost(Focal focal) => focal.Direction == -1 ? End(focal) : Start(focal);
	    public long LeftMost(Focal focal) => focal.Direction == -1 ? Start(focal) : End(focal);

        // _transform Methods
        public UnitFocal Unit(Domain domain) => domain.UnitFocal;
        public long UnitTicks(Domain domain) => domain.UnitFocal.LengthInTicks;
        public long UnitStart(Domain domain) => domain.UnitFocal.StartTickValue;
        public long UnitEnd(Domain domain) => domain.UnitFocal.EndTickValue;
        public long RangeTicks(Domain domain) => domain.MaxRange.LengthInTicks;


    }

}
