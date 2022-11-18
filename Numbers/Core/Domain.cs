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

        public int UnitFocalId { get; set; }
        public IFocal UnitFocal
        {
	        get => Trait.FocalStore[UnitFocalId];
	        private set => UnitFocalId = value.Id;
        }
        public RatioSeg UnitFocalRatio => FocalAsRatio(UnitFocal);
        public bool IsUnitPerspective => UnitFocal.Direction >= 0;

        public int UnitId { get; set; }
        public Number Unit => Workspace.NumberStore[UnitId];

        public int MaxRangeId { get; set; }
        public IFocal MaxRange => Trait.FocalStore[MaxRangeId];
        public Complex MaxRangeValue => FocalAsValue(MaxRange);
        public long MaxRangeLengthTicks => MaxRange.LengthInTicks;
        public double MaxRangeLengthValue => MaxRange.LengthInTicks / (double)UnitFocal.LengthInTicks;

        public Domain(int traitId, int unitFocalId, int maxRangeId)
        {
	        Id = domainCounter++;
            TraitId = traitId;
	        UnitFocalId = unitFocalId;
            MaxRangeId = maxRangeId;
            UnitId = AddNumber(unitFocalId).Id;
        }
        public Domain(Trait trait, FocalRef unit, FocalRef range) : this(trait.Id, unit.Id, range.Id) { }

        public Number AddNumber(FocalRef focal) => AddNumber(focal.Id);
        public Number AddNumber(int focalId)
        {
	        return new Number(this, focalId); // Number class holds the static list of added numbers.
        }
        public IEnumerable<Number> Numbers()
        {
	        foreach (var id in NumberIds)
	        {
		        yield return Workspace.NumberStore[id];
	        }
        }
        public void GetNumberValues(Dictionary<int, Complex> dict)
        {
            dict.Clear();
	        foreach (var num in Numbers())
	        {
		        if (!num.IsUnit)
		        {
			        dict.Add(num.Id, num.Value);
                }
	        }
        }
        public void SetNumberValues(Dictionary<int, Complex> values, params int[] ignoreIds)
        {
	        foreach (var num in values)
	        {
		        if (!ignoreIds.Contains(num.Key))
		        {
			        Workspace.NumberStore[num.Key].Value = num.Value;
                }
	        }
        }


        public long[] WholeNumberTicks()
        {
	        var result = new List<long>();
	        var rangeStart = MaxRange.StartTickPosition;
	        var rangeEnd = MaxRange.EndTickPosition;
            var rangeLen = MaxRange.LengthInTicks;
	        var unitLen = UnitFocal.LengthInTicks;
	        var zero = UnitFocal.StartTickPosition;

	        var tick = rangeStart + ((zero - rangeStart) % unitLen);
	        while (tick <= rangeEnd)
	        {
                result.Add(tick);
                tick += Math.Abs(unitLen);
	        }

	        return result.ToArray();
        }

        public Complex FocalAsValue(IFocal focal)
        {
	        return PositionsAsValue(focal.StartTickPosition, focal.EndTickPosition);
        }
        public Complex PositionsAsValue(long startTick, long endTick)
        {
	        var zeroTick = UnitFocal.StartTickPosition;
	        var len = (double)UnitFocal.NonZeroLength;
	        var sv = (-startTick + zeroTick) / len;
	        var ev = (endTick - zeroTick) / len;
	        return new Complex(ev, sv);
        }
        public RatioSeg FocalAsRatio(IFocal focal)
        {
	        return PositionsAsRatio(focal.StartTickPosition, focal.EndTickPosition);
        }
        public RatioSeg PositionsAsRatio(long startTick, long endTick)
        {
	        var maxRange = MaxRange;
	        var start = (startTick - maxRange.StartTickPosition) / (float)(maxRange.LengthInTicks);
	        var end = (endTick - maxRange.StartTickPosition) / (float)(maxRange.LengthInTicks);
	        return new RatioSeg(start, end);
        }


    }
}
