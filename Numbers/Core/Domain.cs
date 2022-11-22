﻿using System.Linq;
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
        public int CreationIndex => Id - (int) MathElementKind.Domain - 1;

        public int TraitId { get; }
        public Trait MyTrait => MyBrain.TraitStore[TraitId];
        public List<int> NumberIds { get; } = new List<int>();
        public IEnumerable<Number> Numbers()
        {
	        foreach (var id in NumberIds)
	        {
		        yield return MyBrain.NumberStore[id];
	        }
        }

        public int BasisNumberId { get; set; }
        public Number BasisNumber => MyBrain.NumberStore[BasisNumberId];
        public IFocal BasisFocal => MyTrait.FocalStore[BasisNumber.FocalId];
        public int BasisFocalId => BasisNumber.FocalId;
        public bool BasisIsReciprocal { get; set; }// True when ticks are larger than the unit basis
        public long TickLength => BasisIsReciprocal ? Math.Max(BasisFocal.LengthInTicks, 1) : 1;

        public int MinMaxNumberId { get; set; }
        public Number MinMaxNumber => MyBrain.NumberStore[MinMaxNumberId];
        public IFocal MinMaxFocal => MyTrait.FocalStore[MinMaxFocalId];
        public int MinMaxFocalId => MinMaxNumber.FocalId;
        public Range MinMaxRange => MinMaxFocal.RangeWithBasis(BasisFocal);

        public bool IsUnitPerspective => BasisFocal.Direction == 1;
        public bool IsUnotPerspective => BasisFocal.Direction == -1;

        public Domain(int traitId, int unitFocalId, int minMaxFocalId)
        {
	        Id = domainCounter++;
            TraitId = traitId;
            BasisNumberId = new Number(this, unitFocalId).Id;
            MinMaxNumberId = new Number(this, minMaxFocalId).Id;
            MyTrait.DomainStore.Add(Id, this);
        }
        public Domain(Trait trait, FocalRef unit, FocalRef range) : this(trait.Id, unit.Id, range.Id) { }
        public Number CreateNumberByPositions(long start, long end) => new Number(this, start, end);
        public Number CreateNumberByValues(double start, double end) => new Number(this, new Range(start, end));


        public Range GetValueOf(IFocal focal)
        {
            Range result;
            if (BasisIsReciprocal)
            {
	            result = focal.ReciprocalBasisRange(BasisFocal);
            }
            else
            {
	            result = focal.RangeWithBasis(BasisFocal);
            }
            return result;
        }
        public void SetValueOf(IFocal focal, Range range)
        {
	        focal.SetWithRange(range, BasisFocal);
	        RoundToNearestTick(focal);
        }
        public Range GetValueOf(Number num) => GetValueOf(num.Focal);
        public void SetValueOf(Number num, Range range) => SetValueOf(num.Focal,range);

        public double ValueFromStartTickPosition(long startPos) => (BasisFocal.StartTickPosition - startPos) / (double)BasisFocal.LengthInTicks;
        public double ValueFromEndTickPosition(long endPos) => (endPos - BasisFocal.StartTickPosition) / (double)BasisFocal.LengthInTicks;
        public long StartTickPositionFrom(double value) => (long)Math.Round(-value * BasisFocal.AbsLengthInTicks) + BasisFocal.StartTickPosition;
        public long EndTickPositionFrom(double value) => (long)Math.Round(value * BasisFocal.AbsLengthInTicks) + BasisFocal.StartTickPosition;
        public FocalRef FocalFromRange(Range range)
        {
	        var result = FocalRef.CreateByValues(MyTrait, 0,1);
	        result.SetWithRange(range, BasisFocal);
	        RoundToNearestTick(result);
	        return result;
        }

        public Range ClampToNearestTick(Range range) => new Range( ClampToNearestTick((long)range.Start), ClampToNearestTick((long)range.End));

        public long ClampToNearestTick(long value) => (long)(value / (double)TickLength) * TickLength;
        public long RoundToNearestTick(long value) => (long)Math.Round(value / (double)TickLength) * TickLength;

        public void RoundToNearestTick(IFocal focal)
        {
	        focal.StartTickPosition = RoundToNearestTick(focal.StartTickPosition);
	        focal.EndTickPosition = RoundToNearestTick(focal.EndTickPosition);
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
