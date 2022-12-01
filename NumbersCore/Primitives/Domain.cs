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

        public IFocal BasisFocal { get; set; }
        public IFocal MinMaxFocal { get; set; }
        public Number BasisNumber { get; set; }
        public Number MinMaxNumber { get; set; }
        public bool BasisIsReciprocal { get; set; }// True when ticks are larger than the unit basis


        public int BasisNumberId => BasisNumber.Id;
        public int BasisFocalId => BasisNumber.BasisFocal.Id;
        public int MinMaxFocalId => MinMaxNumber.FocalId;
        public int MinMaxNumberId => MinMaxNumber.Id;

        public Range MinMaxRange => BasisFocal.RangeAsBasis(MinMaxFocal);
        public double TickToBasisRatio => BasisIsReciprocal ? BasisFocal.NonZeroLength : 1.0 / BasisFocal.NonZeroLength;

        public bool IsUnitPerspective => BasisFocal.IsUnitPerspective;
        public bool IsUnotPerspective => BasisFocal.IsUnotPerspective;

        public Domain(Trait trait, IFocal basisFocal, IFocal minMaxFocal = default)
        {
	        Id = domainCounter++;
	        Brain = trait.Brain;
	        TraitId = trait.Id;
	        BasisFocal = basisFocal;
	        MinMaxFocal = minMaxFocal;
            BasisNumber = new Number(this, basisFocal);
            MinMaxNumber = minMaxFocal == default ? new Number(this, trait.MaxFocal) : new Number(this, minMaxFocal);
            Trait.DomainStore.Add(Id, this);
        }

        public Number Zero() => new Number(this, BasisFocal.StartTickPosition, BasisFocal.StartTickPosition, false);
        public Number One() => new Number(this, BasisFocal.StartTickPosition, BasisFocal.EndTickPosition, false);

        public Number CreateNumberByPositions(long start, long end, bool addToStore) => new Number(this, start, end, addToStore);
        public Number CreateNumberByValues(double start, double end, bool addToStore) => new Number(this, new Range(start, end), addToStore);

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

        public IFocal CreateFocalFromRange(Range range, bool addToStore)
        {
	        var result = Focal.CreateByValues(Trait, 0, 1, addToStore);
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
