
using System.Dynamic;

namespace Numbers.Core
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Numerics;


	public class Number : IMathElement
	{
		public MathElementKind Kind => MathElementKind.Number;
		private static int numberCounter = 1 + (int) MathElementKind.Number;

		public Brain MyBrain => Brain.ActiveBrain;
        public Number this[int i] => MyBrain.NumberStore[i];

		public int Id { get; set; }
		public int DomainId
		{
			get => Domain.Id;
			set => Domain = Domain.MyTrait.DomainStore[value];
		}

		// number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
		public int FocalId { get; set; }

		public Domain Domain { get; set; }
		public Trait Trait => Domain.MyTrait;
		public IFocal Focal => Trait.FocalStore[FocalId];
		public IFocal BasisFocal => Domain.BasisFocal;

		public long ZeroTick => BasisFocal.StartTickPosition;
		public long BasisTicks => BasisFocal.LengthInTicks;
		public long AbsBasisTicks => BasisFocal.AbsLengthInTicks;

		public bool IsBasis => Domain.BasisNumberId == Id;
		public bool IsUnit => IsBasis && Direction == 1;
		public bool IsUnot => IsBasis && Direction == -1;
		public bool IsUnitPerspective => Domain.IsUnitPerspective;
		public bool IsUnotPerspective => !Domain.IsUnitPerspective;
		public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;

		public Number(Domain domain, int focalId)
		{
			Id = numberCounter++;
			Domain = domain;
			FocalId = focalId;
			domain.NumberIds.Add(Id);
			MyBrain.NumberStore.Add(Id, this);
		}

		public Number(Domain domain, Range value) : this(domain, domain.FocalFromRange(value).Id) { }
		public Number(Domain domain, long start, long end) : this(domain, FocalRef.CreateByValues(domain.MyTrait, start, end).Id) { }

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
			get => Value.Start;//Domain.ValueFromStartTickPosition(StartTickPosition); // Value.Start is a little less efficient
			set => Value = new Range(value, Value.End); //StartTicks = Domain.StartTickPositionFrom(value);

		}
		public double EndValue
		{
			get => Value.End;//Domain.ValueFromEndTickPosition(EndTickPosition); // Value.End is a little less efficient
            set => Value = new Range(Value.Start, value); //EndTicks = Domain.EndTickPositionFrom(value);
        }
		public Range Value //*
		{
			get => Focal.RangeWithBasis(BasisFocal);
			set => Focal.SetWithRange(value, BasisFocal);// Focal.Reset(Domain.FocalFromRange(value));
		}
		public Range ValueInFullUnitPerspective => Domain.IsUnitPerspective ? new Range(-StartValue, EndValue) : new Range(StartValue, -EndValue);
		public Range ValueInFullUnotPerspective => Domain.IsUnitPerspective ? new Range(StartValue, -EndValue) : new Range(-StartValue, EndValue);

        public long WholeStartValue => (long)StartValue;
		public long WholeEndValue => (long)EndValue;
		public long RemainderStartValue => Math.Abs(StartTicks % BasisFocal.NonZeroLength); //*
		public long RemainderEndValue => Math.Abs(EndTicks % BasisFocal.NonZeroLength); //*

		public Range FloorRange => new Range(Math.Ceiling(StartValue), Math.Floor(EndValue));
		public Range CeilingRange => new Range(Math.Floor(StartValue), Math.Ceiling(EndValue));
		public Range RoundedRange => new Range(Math.Round(StartValue), Math.Round(EndValue));
		public Range RemainderRange => Value - FloorRange;

		public Range RangeInMinMax => Focal.UnitTRangeIn(Domain.MinMaxFocal); //*

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

        public Number Clone() => new Number(Domain, Focal.Clone().Id);

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
