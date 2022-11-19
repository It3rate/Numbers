using System.Linq;
using System.Numerics;
using Numbers.Core;

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
    // This is what domains are. BasisFocal size(s), min tick size, max ranges, confidence/probability data at limits of measure (gaussian precision etc).
    // E.g. changing the domain 'tolerance' could change neat writing into messy.

    // Min size is tick size. BasisFocal is start/end point (only one focal allowed for a unit). MinMaxFocal is bounds in ticks. todo: add conversion methods etc.
    public class Domain : IMathElement
    {
	    public Brain MyBrain => Brain.ActiveBrain;

	    public  MathElementKind Kind => MathElementKind.Domain;
        private static int domainCounter = 1 + (int)MathElementKind.Domain;

        public int Id { get; }

        public int TraitId { get; }
        public Trait MyTrait => MyBrain.TraitStore[TraitId];

        public int BasisNumberId { get; set; }
        public Number BasisNumber => MyBrain.NumberStore[BasisNumberId];
        public IFocal BasisFocal => MyTrait.FocalStore[BasisNumber.FocalId];

        public List<int> NumberIds { get; } = new List<int>();
        public IEnumerable<Number> Numbers()
        {
	        foreach (var id in NumberIds)
	        {
		        yield return MyBrain.NumberStore[id];
	        }
        }

        public bool IsUnitPerspective => BasisFocal.Direction >= 0;

        public int MinMaxFocalId { get; set; }
        public IFocal MinMaxFocal => MyTrait.FocalStore[MinMaxFocalId];
        public Range MinMaxRange => MinMaxFocal.RangeInBasis(BasisFocal);
        //public Range MinMaxRange => MinMaxFocal.UnitTRangeIn(BasisFocal);
        //public Range MinMaxRange => BasisFocal.UnitTRangeIn(MinMaxFocal);

        public Domain(int traitId, int unitFocalId, int minMaxFocalId)
        {
	        Id = domainCounter++;
            TraitId = traitId;
            MinMaxFocalId = minMaxFocalId;
            BasisNumberId = new Number(this, unitFocalId).Id; 
        }
        public Domain(Trait trait, FocalRef unit, FocalRef range) : this(trait.Id, unit.Id, range.Id) { }

        public double ValueFromStartTickPosition(long startPos) => (BasisFocal.StartTickPosition - startPos) / (double)BasisFocal.NonZeroLength;
        public double ValueFromEndTickPosition(long endPos) => (endPos - BasisFocal.StartTickPosition) / (double)BasisFocal.NonZeroLength;
        public long StartTickPositionFrom(double value) => (long)-Math.Round(value * BasisFocal.LengthInTicks) + BasisFocal.StartTickPosition;
        public long EndTickPositionFrom(double value) => (long)Math.Round(value * BasisFocal.LengthInTicks) + BasisFocal.StartTickPosition;
        public FocalRef FocalFromRange(Range range)
        {
	        var bf = BasisFocal;
	        var len = bf.LengthInTicks;
	        var start = (long)-Math.Round(range.Start * len) + bf.StartTickPosition;
	        var end = (long)Math.Round(range.End * len) + bf.StartTickPosition;
            return FocalRef.CreateByValues(MyTrait, start, end);
        }

        public void SaveNumberValues(Dictionary<int, Range> dict, params int[] ignoreIds)
        {
            dict.Clear();
	        foreach (var num in Numbers())
	        {
		        if (!ignoreIds.Contains(num.Id))
		        {
			        dict.Add(num.Id, num.Value);
                }
	        }
        }
        public void RestoreNumberValues(Dictionary<int, Range> values, params int[] ignoreIds)
        {
	        foreach (var kvp in values)
	        {
		        if (!ignoreIds.Contains(kvp.Key))
		        {
			        MyBrain.NumberStore[kvp.Key].Value = kvp.Value;
                }
	        }
        }
    }
}
