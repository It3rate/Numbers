using System.Linq;
using System.Numerics;
using Numbers.Core;
using Numbers.Mind;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    // this constrains the significant figures of a measurement, unit/not is the minimum tick size, and range is the max possible.
    // Feels like this should just be the unit chosen - that works from min tick size, but how to specify a max value? On the trait I guess?
    // the trait then needs to 'know' how measurable it is, and the units are calibrated to that, which seems overspecified
    // (eg 'length' knows a nanometer is min and a light year max, cms and inches calibrate to this. Hmm, no).
    // So each 'situation' has sig-fig/precision metadata. Working in metal units vs working in wood units. A metal length trait and a wood length trait, convertible.
    // This is what domains are. UnitFocal size(s), min tick size, max ranges, confidence/probability data at limits of measure (gaussian precision etc).
    // E.g. changing the domain 'tolerance' could change neat writing into messy.

    // Min size is tick size. UnitFocal is start/end point (only one focal allowed for a unit). MaxRange is bounds in ticks. todo: add conversion methods etc.
    public class Domain : IMathElement
    {
	    private Brain _brain => Brain.BrainA;

	    public  MathElementKind Kind => MathElementKind.Domain;
        private static int domainCounter = 1 + (int)MathElementKind.Domain;

        public int Id { get; }
        public List<int> NumberIds { get; } = new List<int>();

        public int TraitId { get; set; }
        public Trait Trait => _brain.TraitStore[TraitId];

	    private int _unitFocalFocalId;
        public int UnitFocalId
        {
	        get => _unitFocalFocalId;
	        set { _unitFocalFocalId = value; UnitFocal = new UnitFocal(Trait.FocalStore[UnitFocalId]);}
        }
        public UnitFocal UnitFocal { get; private set; }
        public RatioSeg UnitFocalRatio => UnitFocal.RatioIn(this);
        public bool IsUnitPerspective => UnitFocal.Direction >= 0;


        public int UnitId { get; set; }
        public Number Unit => Number.NumberStore[UnitId];

        public int MaxRangeId { get; set; }
        public Focal MaxRange => Trait.FocalStore[MaxRangeId];
        public double MaxRangeValue => MaxRange.LengthInTicks / (double)UnitFocal.LengthInTicks;

        public Domain(int traitId, int unitFocalId, int maxRangeId)
        {
	        Id = domainCounter++;
            TraitId = traitId;
	        UnitFocalId = unitFocalId;
            MaxRangeId = maxRangeId;
            UnitId = AddNumber(unitFocalId).Id;
        }
        public Domain(Trait trait, Focal unit, Focal range) : this(trait.Id, unit.Id, range.Id) { }

        public Number AddNumber(Focal focal) => AddNumber(focal.Id);
        public Number AddNumber(int focalId)
        {
	        return new Number(this, focalId); // Number class holds the static list of added numbers.
        }
        public IEnumerable<Number> Numbers()
        {
	        foreach (var id in NumberIds)
	        {
		        yield return Number.NumberStore[id];
	        }
        }
        public Dictionary<int, Complex> GetNumberValues()
        {
            var result = new Dictionary<int, Complex>();
	        foreach (var num in Numbers())
	        {
		        if (!num.IsUnit)
		        {
			        result.Add(num.Id, num.Value);
                }
	        }
	        return result;
        }
        public void SetNumberValues(Dictionary<int, Complex> values)
        {
	        foreach (var num in values)
	        {
		        Number.NumberStore[num.Key].Value = num.Value;
	        }
        }

        public long[] WholeNumberTicks()
        {
	        var result = new List<long>();
	        var rangeStart = MaxRange.StartTickValue;
	        var rangeEnd = MaxRange.EndTickValue;
            var rangeLen = MaxRange.LengthInTicks;
	        var unitLen = UnitFocal.LengthInTicks;
	        var zero = UnitFocal.StartTickValue;

	        var tick = rangeStart + ((zero - rangeStart) % unitLen);
	        while (tick <= rangeEnd)
	        {
                result.Add(tick);
                tick += Math.Abs(unitLen);
	        }

	        return result.ToArray();
        }


    }
}
