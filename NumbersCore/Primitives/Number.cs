
using System;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	public class Number : IMathElement
	{
		public MathElementKind Kind => MathElementKind.Number;
		public int Id { get; internal set; }
		public int CreationIndex => Id - (int) Kind - 1;

		public Brain Brain => Trait?.Brain;
		public virtual Trait Trait => Domain?.Trait;
        public virtual Domain Domain { get; set; }
        public int DomainId
		{
			get => Domain.Id;
			set => Domain = Domain.Trait.DomainStore[value];
		}

		// number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.

        public virtual IFocal BasisFocal => Domain?.BasisFocal;
		public IFocal Focal { get; set; }

		public int FocalId => Focal.Id;

		public long ZeroTick => BasisFocal.StartTickPosition;
		public long BasisTicks => BasisFocal.LengthInTicks;
		public long AbsBasisTicks => BasisFocal.AbsLengthInTicks;

		public bool IsBasis => Domain.BasisNumber.Id == Id;
		public bool IsUnitPerspective => Domain.IsUnitPerspective;
		public bool IsUnotPerspective => Domain.IsUnotPerspective;
		public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;

		protected internal Number(IFocal focal)
		{
			Focal = focal;
		}

		private long StartTickPosition
		{
			get => Focal.StartTickPosition;
			set => Focal.StartTickPosition = value;
		}
		private long EndTickPosition
		{
			get => Focal.EndTickPosition;
			set => Focal.EndTickPosition = value;
		}
		private long StartTicks
		{
			get => -StartTickPosition + ZeroTick;
			set => StartTickPosition = ZeroTick - value;
		}
		private long EndTicks
		{
			get => EndTickPosition - ZeroTick;
			set => EndTickPosition = value + ZeroTick;
		}
		private long TickCount => EndTickPosition - StartTickPosition;

		public double StartValue
		{
			get => Value.Start; //Domain.ValueFromStartTickPosition(StartTickPosition); // Value.Start is a little less efficient
			set => Value = new Range(value, Value.End); //StartTicks = Domain.StartTickPositionFrom(value);

		}
		public double EndValue
		{
			get => Value.End; //Domain.ValueFromEndTickPosition(EndTickPosition); // Value.End is a little less efficient
			set => Value = new Range(Value.Start, value); //EndTicks = Domain.EndTickPositionFrom(value);
		}
		public Range Value //*
		{
			get => Domain.GetValueOf(Focal); //Focal.RangeWithBasis(BasisFocal);
			set => Domain.SetValueOf(Focal, value);
		}
		public Range ValueInRenderPerspective => new Range(-StartValue, EndValue);

		public long WholeStartValue => (long) StartValue;
		public long WholeEndValue => (long) EndValue;
		public long RemainderStartValue => Domain.BasisIsReciprocal ? 0 : Math.Abs(StartTicks % BasisFocal.NonZeroLength);
		public long RemainderEndValue => Domain.BasisIsReciprocal ? 0 : Math.Abs(EndTicks % BasisFocal.NonZeroLength);
		public Range RangeInMinMax => Focal.UnitTRangeIn(Domain.MinMaxFocal); //*

		public Range FloorRange => new Range(Math.Ceiling(StartValue), Math.Floor(EndValue));
		public Range CeilingRange => new Range(Math.Floor(StartValue), Math.Ceiling(EndValue));
		public Range RoundedRange => new Range(Math.Round(StartValue), Math.Round(EndValue));
		public Range RemainderRange => Value - FloorRange;

		public void PlusTick() => EndTickPosition += 1;
		public void MinusTick() => EndTickPosition -= 1;
        public void AddStartTicks(long ticks) => StartTickPosition += ticks;
        public void AddEndTicks(long ticks) => EndTickPosition += ticks;
		public void AddTicks(long startTicks, long endTicks) { StartTickPosition += startTicks; EndTickPosition += endTicks; }

	// Operations with segments and units allow moving the unit around freely, so for example,
        // you can shift a segment by aligning the unit with start or end,
        // and scale in place by moving the unit to left, right or center (equivalent to affine scale, where you move to zero, scale, then move back)
        // need to have overloads that allow shifting the unit temporarily
        public void Add(Number other)
        {
            // todo: eventually all math on Numbers will be in ticks, allowing preservation of precision etc. Requires syncing of basis, domains.
	        Value += other.Value;
        }
        public void Subtract(Number other)
		{
			Value -= other.Value;
        }
        public void Multiply(Number other)
		{
			Value *= other.Value;
        }
        public void Divide(Number other)
		{
			Value /= other.Value;
        }

        public void InterpolateFromZero(Number t, Number result) => InterpolateFromZero(this, t, result);
        public void InterpolateFrom(Number source, Number t, Number result) => Interpolate(source, this, t, result);
        public void InterpolateTo(Number target, Number t, Number result) => Interpolate(this, target, t, result);
        public static void InterpolateFromZero(Number target, Number t, Number result)
        {
	        var targetValue = target.Value;
	        var tValue = t.Value;
	        result.StartValue = targetValue.Start * tValue.Start;
	        result.EndValue = targetValue.End * tValue.End;
        }
        public static void InterpolateFromOne(Number target, Number t, Number result)
        {
	        var targetValue = target.Value;
	        var tValue = t.Value;
	        result.StartValue = targetValue.Start * tValue.Start;
	        result.EndValue = (targetValue.End - 1.0) * tValue.End + 1.0;
        }
        public static void Interpolate(Number source, Number target, Number t, Number result)
        {
	        var sourceValue = source.Value;
	        var targetValue = target.Value;
            var tValue = t.Value;
            result.StartValue = (targetValue.Start - sourceValue.Start) * tValue.Start + sourceValue.Start;
            result.EndValue = (targetValue.End - sourceValue.End) * tValue.End + sourceValue.End;
        }

        public Number Clone()
        {
	        var result = new Number(Focal.Clone());
	        Domain.AddNumber(result);
	        return result;
        }

        public override string ToString()
		{
			var v = Value;
			return $"[{-v.Start:0.00}->{v.End:0.00}]";
		}
		public override bool Equals(object obj)
		{
			return obj is Number other && Equals(other);
		}
		public bool Equals(Number value)
		{
			return StartTickPosition.Equals(value.StartTickPosition) && 
			       EndTickPosition.Equals(value.EndTickPosition) &&
			       Focal.StartTickPosition.Equals(value.StartTickPosition) &&
			       Focal.EndTickPosition.Equals(value.EndTickPosition);
		}

		public override int GetHashCode()
		{
			unchecked
			{
                var hashCode = StartTickPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EndTickPosition.GetHashCode();
                hashCode = (hashCode * 17) ^ Focal.StartTickPosition.GetHashCode();
                hashCode = (hashCode * 13) ^ Focal.EndTickPosition.GetHashCode();
                return hashCode;
			}
		}
    }
}
