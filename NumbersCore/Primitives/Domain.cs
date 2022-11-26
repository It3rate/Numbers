using System;
using System.Collections.Generic;
using System.Linq;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
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
	    public Brain Brain { get; }

	    public  MathElementKind Kind => MathElementKind.Domain;
        private static int domainCounter = 1 + (int)MathElementKind.Domain;

        public int Id { get; }
        public int CreationIndex => Id - (int)Kind - 1;

        public int TraitId { get; }
        public Trait Trait => Brain.TraitStore[TraitId];
        public List<int> NumberIds { get; } = new List<int>();
        public IEnumerable<Number> Numbers()
        {
	        foreach (var id in NumberIds)
	        {
		        yield return Brain.NumberStore[id];
	        }
        }

        public int BasisNumberId { get; set; }
        public Number BasisNumber => Brain.NumberStore[BasisNumberId];
        public IFocal BasisFocal => Trait.FocalStore[BasisNumber.FocalId];
        public int BasisFocalId => BasisNumber.FocalId;
        public bool BasisIsReciprocal { get; set; }// True when ticks are larger than the unit basis

        public double TickToBasisRatio => BasisIsReciprocal ? BasisFocal.NonZeroLength : 1.0 / BasisFocal.NonZeroLength;

        public int MinMaxNumberId { get; set; }
        public Number MinMaxNumber => Brain.NumberStore[MinMaxNumberId];
        public IFocal MinMaxFocal => Trait.FocalStore[MinMaxFocalId];
        public int MinMaxFocalId => MinMaxNumber.FocalId;
        public Range MinMaxRange => BasisFocal.RangeAsBasis(MinMaxFocal);

        public bool IsUnitPerspective => BasisFocal.IsUnitPerspective;
        public bool IsUnotPerspective => BasisFocal.IsUnotPerspective;

        public Domain(Trait trait, int basisFocalId, int minMaxFocalId)
        {
	        Id = domainCounter++;
	        Brain = trait.Brain;
	        TraitId = trait.Id;
            BasisNumberId = new Number(this, basisFocalId).Id;
            MinMaxNumberId = new Number(this, minMaxFocalId).Id;
            Trait.DomainStore.Add(Id, this);
        }
        public Domain(Trait trait, FocalRef basis, FocalRef minMax) : this(trait, basis.Id, minMax.Id) { }
        public Number CreateNumberByPositions(long start, long end) => new Number(this, start, end);
        public Number CreateNumberByValues(double start, double end) => new Number(this, new Range(start, end));

        public Range GetValueOf(IFocal focal) => focal.GetRangeWithBasis(BasisFocal, BasisIsReciprocal);
        public void SetValueOf(IFocal focal, Range range) => focal.SetWithRangeAndBasis(range, BasisFocal, BasisIsReciprocal);
        public Range GetValueOf(Number num) => GetValueOf(num.Focal);
        public void SetValueOf(Number num, Range range) => SetValueOf(num.Focal,range);

        public Range ClampToInnerBasis(Range range) => range.ClampInner();
        public Range ClampToInnerTick(Range range) => (range / TickToBasisRatio).ClampInner() * TickToBasisRatio;
        public Range RoundToNearestBasis(Range range) => range.Round();
        public Range RoundToNearestTick(Range range) => (range / TickToBasisRatio).Round() * TickToBasisRatio;

        public long RoundToNearestTick(long value) => (long)(Math.Round(value / TickToBasisRatio) * TickToBasisRatio);
        public void RoundToNearestTick(IFocal focal)
        {
	        focal.StartTickPosition = RoundToNearestTick(focal.StartTickPosition);
	        focal.EndTickPosition = RoundToNearestTick(focal.EndTickPosition);
        }

        public FocalRef CreateFocalFromRange(Range range)
        {
	        var result = FocalRef.CreateByValues(Trait, 0, 1);
	        result.SetWithRangeAndBasis(range, BasisFocal, BasisIsReciprocal);
	        return result;
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
			        Brain.NumberStore[kvp.Key].Value = kvp.Value;
                }
	        }
        }
    }
}
