using System.Numerics;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum Pointing { Left, Right }

    /// <summary>
    /// Traits are measurable properties on objects. They can be composed of multiple domains (like mph or dollars/day or lwh) but do not need to be.
    /// </summary>
    public class Trait
    {
	    public static List<Trait> TraitStore { get; } = new List<Trait>(1024);
	    public static List<long> ValueStore { get; } = new List<long>(4096);

	    public Dictionary<int, Focal> FocalStore { get;} = new Dictionary<int, Focal>();
	    public Dictionary<int, Domain> DomainStore { get; } = new Dictionary<int, Domain>();
	    public Dictionary<int, Transform> TransformStore { get; } = new Dictionary<int, Transform>();
        public int Id;

	    public Trait()
	    {
		    Id = TraitStore.Count;
            TraitStore.Add(this);
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
	    public long RightMost(Focal focal) => focal.Direction == Pointing.Left ? End(focal) : Start(focal);
	    public long LeftMost(Focal focal) => focal.Direction == Pointing.Left ? Start(focal) : End(focal);

        // _transform Methods
        public UnitFocal Unit(Domain domain) => domain.Unit;
        public long UnitTicks(Domain domain) => domain.Unit.LengthInTicks;
        public long UnitStart(Domain domain) => domain.Unit.StartTickValue;
        public long UnitEnd(Domain domain) => domain.Unit.EndTickValue;
        public long RangeTicks(Domain domain) => domain.MaxRange.LengthInTicks;


    }

}
