using System.Linq;
using System.Numerics;

namespace Numbers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // this constrains the significant figures of a measurement, unit/not is the minimum tick size, and range is the max possible.
    // Feels like this should just be the unit chosen - that works from min tick size, but how to specify a max value? On the trait I guess?
    // the trait then needs to 'know' how measurable it is, and the units are calibrated to that, which seems overspecified
    // (eg 'length' knows a nanometer is min and a light year max, cms and inches calibrate to this. Hmm, no).
    // So each 'situation' has sig-fig/precision metadata. Working in metal units vs working in wood units. A metal length trait and a wood length trait, convertible.
    // This is what domains are. Unit size(s), min tick size, max ranges, confidence/probability data at limits of measure (gaussian precision etc).
    // E.g. changing the domain 'tolerance' could change neat writing into messy.

    // Min size is tick size. Unit is start/end point (only one focal allowed for a unit). MaxRange is bounds in ticks. todo: add conversion methods etc.
    public class Domain
    {
	    private static int domainCounter = 1;

        //public Trait Trait { get; }
        public int Id { get; }
        public int TraitId { get; set; }
        public int UnitId { get; set; }
        public int MaxRangeId { get; set; }
        public Dictionary<int, Number> Numbers { get; } = new Dictionary<int, Number>();

        public Trait Trait => Trait.Traits[TraitId];
        public Focal Unit => Trait.Focals[UnitId]; // todo: a unit must always 'point right' as it is from the perspective of the positive unit
        public Focal MaxRange => Trait.Focals[MaxRangeId];

        public Domain(int traitId, int unitId, int maxRangeId)
        {
            TraitId = traitId;
	        UnitId = unitId;
            MaxRangeId = maxRangeId;
	        Id = domainCounter++;
        }
        public Domain(Trait trait, Focal unit, Focal range) : this(trait.Id, unit.Id, range.Id) { }

        public int AddNumber(Focal focal) => AddNumber(focal.Id);
        public int AddNumber(int focalId)
        {
            var n = new Number(this, focalId);
            Numbers.Add(n.Id, n);
            return n.Id;
        }

    }
}
