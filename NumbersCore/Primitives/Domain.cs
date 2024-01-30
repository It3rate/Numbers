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
	    public  MathElementKind Kind => MathElementKind.Domain;
        private static int domainCounter = 1 + (int)MathElementKind.Domain;
        public int Id { get; internal set; }
        public int CreationIndex => Id - (int)Kind - 1;

	    //public Brain Brain => Trait.Brain;
	    public Trait Trait { get; protected set; }
        public Focal BasisFocal { get; protected set; }
        public bool IsBasisPositive => BasisFocal.Direction == 1;
        public Focal MinMaxFocal { get; protected set; }
        public Number BasisNumber { get; protected set; }
        public Number MinMaxNumber { get; protected set; }

        public readonly Dictionary<int, Number> NumberStore = new Dictionary<int, Number>();
        public readonly Dictionary<int, NumberSet> NumberSetStore = new Dictionary<int, NumberSet>();

        public bool BasisIsReciprocal { get; set; }// True when ticks are larger than the unit basis

        //public int BasisNumberId => BasisNumber.Id;
        //public int BasisFocalId => BasisNumber.BasisFocal.Id;
        //public int MinMaxFocalId => MinMaxNumber.FocalId;
        //public int MinMaxNumberId => MinMaxNumber.Id;

        public Range MinMaxRange => BasisFocal.RangeAsBasis(MinMaxFocal);
        public double TickToBasisRatio => BasisIsReciprocal ? BasisFocal.NonZeroLength : 1.0 / BasisFocal.NonZeroLength;

        public bool IsUnitPerspective => BasisFocal.IsUnitPerspective;
        public bool IsUnotPerspective => BasisFocal.IsUnotPerspective;

        public Domain(Trait trait, Focal basisFocal, Focal minMaxFocal = default)
        {
	        Id = domainCounter++;
	        Trait = trait;
	        BasisFocal = basisFocal;
	        MinMaxFocal = minMaxFocal;
            BasisNumber = CreateNumber(basisFocal);
            MinMaxNumber = minMaxFocal == default ? CreateNumber(Focal.MinMaxFocal) : CreateNumber(minMaxFocal);
            Trait.DomainStore.Add(Id, this);
        }

        public static Domain CreateDomain(string traitName, int unitSize = 8, int rangeSize = 16)
        {
            Trait trait = Trait.GetOrCreateTrait(Brain.ActiveBrain, traitName);
            var unit = Focal.CreateByValues(0, unitSize);
            var range = Focal.CreateByValues(-rangeSize * unitSize, rangeSize * unitSize);
            var domain = trait.AddDomain(unit, range);
            return domain;
        }

        public int[] NumberIds() => NumberStore.Values.Select(num => num.Id).ToArray();
        public Number GetNumber(int numberId)
        {
	        NumberStore.TryGetValue(numberId, out var result);
	        return result;
        }
        public Number CreateNumber(Focal focal, bool addToStore = true)
        {
	        return AddNumber(new Number(focal), addToStore);
        }
        public Number CreateNumber(Range value, bool addToStore = true)
        {
	        var focal = CreateFocalFromRange(value);
	        return AddNumber(new Number(focal), addToStore);
        }
        public Number CreateNumber(long start, long end, bool addToStore = true)
        {
	        var focal = Focal.CreateByValues(start, end);
	        return AddNumber(new Number(focal), addToStore);
        }
        public Number CreateNumberFromFloats(float startF, float endF, bool addToStore = true)
        {
            long start = (long)(-startF * BasisFocal.LengthInTicks);
            long end = (long)(endF * BasisFocal.LengthInTicks);
            var focal = Focal.CreateByValues(start, end);
            return AddNumber(new Number(focal), addToStore);
        }

        public int _nextStoreIndex = 0;
        public Number AddNumber(Number number, bool addToStore = true)
        {
	        number.Domain = this;
	        number.Id = number.Id == 0 ? Number.NextNumberId() : number.Id;
	        if (addToStore)
	        {
		        NumberStore[number.Id] = number;
                number.StoreIndex = _nextStoreIndex++;
            }
	        return number;
        }
        public bool RemoveNumber(Number number)
        {
	        number.Domain = null;
	        return NumberStore.Remove(number.Id);
        }


        private int _numberSetCounter = 1 + (int)MathElementKind.NumberSet;
        public int NextNumberSetId() => _numberSetCounter++ + Id;
        public NumberSet AddNumberSet(NumberSet numberSet, bool addToStore = true)
        {
	        numberSet.Domain = this;
	        numberSet.Id = numberSet.Id == 0 ? NextNumberSetId() : numberSet.Id;
	        if (addToStore)
	        {
                NumberSetStore[numberSet.Id] = numberSet;
	        }
	        return numberSet;
        }
        public bool RemoveNumberSet(NumberSet numberSet)
        {
	        numberSet.Domain = null;
	        return NumberSetStore.Remove(numberSet.Id);
        }

        public Number Zero() => CreateNumber(BasisFocal.StartTickPosition, BasisFocal.StartTickPosition);
        public Number One() => CreateNumber(BasisFocal.StartTickPosition, BasisFocal.EndTickPosition);

        public Range GetValueOf(Focal focal, bool isAligned) => focal.GetRangeWithBasis(BasisFocal, BasisIsReciprocal, isAligned);
        public void SetValueOf(Focal focal, Range range, bool isAligned) => focal.SetWithRangeAndBasis(range, BasisFocal, BasisIsReciprocal, isAligned);
        public Range GetValueOf(Number num) => GetValueOf(num.Focal, num.IsAligned);
        public void SetValueOf(Number num, Range range) => SetValueOf(num.Focal, range, num.IsAligned);

        public Range ClampToInnerBasis(Range range) => range.ClampInner();
        public Range ClampToInnerTick(Range range) => (range / TickToBasisRatio).ClampInner() * TickToBasisRatio;
        public Range RoundToNearestBasis(Range range) => range.Round();
        public Range RoundToNearestTick(Range range) => (range / TickToBasisRatio).Round() * TickToBasisRatio;

        public long RoundToNearestTick(long value) => (long)(Math.Round(value / TickToBasisRatio) * TickToBasisRatio);
        public void RoundToNearestTick(Focal focal)
        {
	        focal.StartTickPosition = RoundToNearestTick(focal.StartTickPosition);
	        focal.EndTickPosition = RoundToNearestTick(focal.EndTickPosition);
        }

        public Focal CreateFocalFromRange(Range range)
        {
	        var result = Focal.CreateByValues(0, 1);
	        result.SetWithRangeAndBasis(range, BasisFocal, BasisIsReciprocal, true);
	        return result;
        }

        public void ClearAll()
        {
	        NumberStore.Clear();
        }

        public IEnumerable<Number> Numbers()
        {
	        foreach (var number in NumberStore.Values)
	        {
		        yield return number;
	        }
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
			        NumberStore[kvp.Key].Value = kvp.Value;
                }
	        }
        }
    }
}
