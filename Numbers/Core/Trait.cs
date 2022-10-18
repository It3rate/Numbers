using System.Numerics;

namespace Numbers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum Pointing { Left, Right }

    public class Trait
    {
	    public static List<Trait> Traits { get; } = new List<Trait>(1024);
	    public static List<long> Values { get; } = new List<long>(4096);

	    public Dictionary<int, Focal> Focals { get;} = new Dictionary<int, Focal>();
	    public Dictionary<int, Domain> Domains { get; } = new Dictionary<int, Domain>();
	    public int Id;

	    public Trait()
	    {
		    Id = Traits.Count;
            Traits.Add(this);
	    }

	    public int AddValue(long value)
	    {
            Values.Add(value);
            return Values.Count - 1;
	    }

	    public Focal AddFocalByIndexes(int startIndex, int endIndex)
	    {
		    var result = new Focal(startIndex, endIndex);
		    Focals.Add(result.Id, result);
		    return result;
        }
	    public Focal AddFocalByValues(long start, long end)
	    {
		    var index = Values.Count;
		    Values.Add(start);
		    Values.Add(end);
		    return AddFocalByIndexes(index, index + 1);
	    }
	    public Focal AddFocalByIndexValue(int startIndex, long end)
	    {
		    var index = Values.Count;
		    Values.Add(end);
		    return AddFocalByIndexes(startIndex, index);
	    }
	    public Focal AddFocalByValueIndex(long start, int endIndex)
	    {
		    var index = Values.Count;
		    Values.Add(start);
		    return AddFocalByIndexes(index, endIndex);
	    }

	    public Domain AddDomain(int unitIndex, int rangeIndex)
	    {
		    var result = new Domain(Id, unitIndex, rangeIndex);
		    Domains.Add(result.Id, result);
		    return result;
	    }
	    public Domain AddDomain(Focal unit, Focal range)
	    {
		    return AddDomain(unit.Id, range.Id);
	    }

        // Focal Methods
	    public long Start(Focal focal) => Values[focal.StartId];
	    public long End(Focal focal) => Values[focal.EndId];
	    public long Ticks(Focal focal) => End(focal) - Start(focal);
	    public Pointing Direction(Focal focal) => Start(focal) < End(focal) ? Pointing.Left : Pointing.Right;
	    public long RightMost(Focal focal) => Direction(focal) == Pointing.Left ? End(focal) : Start(focal);
	    public long LeftMost(Focal focal) => Direction(focal) == Pointing.Left ? Start(focal) : End(focal);

        // Domain Methods

        public bool IsUnitPerspective(Domain domain) => Direction(Unit(domain)) == Pointing.Right;
        public Focal Unit(Domain domain) => domain.Unit;
        public long UnitTicks(Domain domain) => domain.Unit.LengthTicks;
        public long UnitStart(Domain domain) => domain.Unit.StartTickValue;
        public long UnitEnd(Domain domain) => domain.Unit.EndTickValue;
        public long RangeTicks(Domain domain) => domain.MaxRange.LengthTicks;


    }

}
