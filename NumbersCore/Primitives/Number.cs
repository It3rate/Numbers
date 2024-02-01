
using System;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    public enum Alignment { Aligned, Inverted };//, Zero, Max }
    public class Number : IMathElement
    {
        public MathElementKind Kind => MathElementKind.Number;

        public int Id { get; internal set; }
        private static int _numberCounter = 1 + (int)MathElementKind.Number;
        public static int NextNumberId() => _numberCounter++;
        public int CreationIndex => Id - (int)Kind - 1;

        public Brain Brain => Trait?.Brain;
        public Trait Trait => Domain.Trait;
        public virtual Domain Domain { get; set; }
        public int DomainId
        {
            get => Domain.Id;
            set => Domain = Domain.Trait.DomainStore[value];
        }

        // number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
        public Focal BasisFocal => Domain.BasisFocal;

        public Focal Focal { get; set; }

        public int FocalId => Focal.Id;

        /// <summary>
        /// Determines if this number has an aligned or inverted perspective relative to the domain basis. 
        /// </summary>
        public Alignment Polarity { get; set; }
        public int PolarityDirection => IsAligned ? 1 : -1;
        public bool IsAligned => Polarity == Alignment.Aligned;
        public bool IsInverted => Polarity == Alignment.Inverted;
        public int Direction => BasisFocal.Direction * PolarityDirection;
        protected long StartTickPosition
        {
            get => Focal.StartPosition;
            set => Focal.StartPosition = value;
		}
		protected long EndTickPosition
        {
            get => Focal.EndPosition;
            set => Focal.EndPosition = value;
		}
		public long StartTicks
		{
			get => -StartTickPosition + ZeroTick;
			set => StartTickPosition = ZeroTick - value;
		}
		public long EndTicks
		{
			get => EndTickPosition - ZeroTick;
			set => EndTickPosition = value + ZeroTick;
		}
		public long TickCount => EndTickPosition - StartTickPosition;

		public double StartValue
		{
			get => Value.Start;
			set => Value = new Range(value, Value.End);

		}
		public double EndValue
		{
			get => Value.End;
			set => Value = new Range(Value.Start, value);
		}
		public Range Value 
		{
			get => Domain.GetValueOf(Focal, IsAligned); //Focal.RangeWithBasis(BasisFocal);
			set => Domain.SetValueOf(Focal, value, IsAligned);
        }

        //public bool IsUnitPerspective => IsAligned;// Domain.IsUnitPerspective; // domains can have a pos or neg basis, numbers can conform or counter that with polarity
        //public bool IsUnotPerspective => IsInverted;// Domain.IsUnotPerspective;
        //public Alignment Direction => (Focal.Direction == BasisFocal.Direction) ? Alignment.Aligned : Alignment.Inverted;
        public long ZeroTick => BasisFocal.StartPosition;
        public long BasisTicks => BasisFocal.LengthInTicks;
        public long AbsBasisTicks => BasisFocal.AbsLengthInTicks;

        public bool IsBasis => Domain.BasisNumber.Id == Id;
        public bool IsMinMax => Domain.MinMaxNumber.Id == Id;
        public bool IsDomainNumber => IsBasis || IsMinMax;

        public int StoreIndex { get; set; } // order added to domain

        public Number(Focal focal, Alignment polarity = Alignment.Aligned)
		{
			Focal = focal;
            Polarity = polarity;
		}

        public Range ExpansiveForce
        {
            get
            {
                var v = Value;
                var len = (float) Math.Sqrt(v.Start * v.Start + v.End * v.End);
                var ratio =(v.EndF) / v.AbsLength();
                var lr = ratio <= 0 ? Math.Abs(ratio) : (1f - ratio);
                var rr = ratio <= 0 ? Math.Abs(ratio + 1f) : (ratio);
                return new Range( lr * len, rr * len);
            }
        }

        public Range ValueInRenderPerspective => IsAligned ? new Range(-StartValue, EndValue) : new Range(-EndValue, StartValue);

        public Number SetWith(Number other)
        {
            if (Domain.Id == other.Domain.Id)
            {
                StartTickPosition = other.StartTickPosition;
                EndTickPosition = other.EndTickPosition;
            }
            else
            {
                Value = other.Value;
            }
            Polarity = other.Polarity;
            return other;
        }

        public double RatioFromStart(double pointOnLine)
        {
            var val = Value;
            var len = val.Length;
            return (pointOnLine - val.Start) / len;
        }
        public Alignment InvertPolarity()
        {
            Polarity = Polarity == Alignment.Aligned ? Alignment.Inverted : Alignment.Aligned;
            return Polarity;
        }
        public Number GetInverted(bool addToStore = true)
        {
            var result = Clone(false);
            result.Value *= -1;
            return result;
        }

        public long WholeStartValue => (long) StartValue;
		public long WholeEndValue => (long) EndValue;
		public long RemainderStartValue => Domain.BasisIsReciprocal ? 0 : (long)(Math.Abs(StartValue % 1) * AbsBasisTicks);
		public long RemainderEndValue => Domain.BasisIsReciprocal ? 0 : (long)(Math.Abs(EndValue % 1) * AbsBasisTicks);
		public Range RangeInMinMax => Focal.UnitTRangeIn(Domain.MinMaxFocal);

        public Range FloorRange => new Range(Math.Ceiling(StartValue), Math.Floor(EndValue));
		public Range CeilingRange => new Range(Math.Floor(StartValue), Math.Ceiling(EndValue));
		public Range RoundedRange => new Range(Math.Round(StartValue), Math.Round(EndValue));
		public Range RemainderRange => Value - FloorRange;

		public void PlusTick() => EndTickPosition += 1;
		public void MinusTick() => EndTickPosition -= 1;
        public void AddStartTicks(long ticks) => StartTickPosition += ticks;
        public void AddEndTicks(long ticks) => EndTickPosition += ticks;
        public void AddTicks(long startTicks, long endTicks) { StartTickPosition += startTicks; EndTickPosition += endTicks; }
        public void ShiftTicks(long ticks) { StartTickPosition = StartTickPosition + ticks; EndTickPosition = EndTickPosition + ticks; }

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

		public void ChangeDomain(Domain newDomain)
		{
			if(newDomain != Domain)
			{
				var value = Value;
				Domain = newDomain; 
				Value = value;
			}
		}

		public bool FullyContains(Number toTest, bool includeEndpoints = true) => toTest != null ? Value.FullyContains(toTest.Value, includeEndpoints) : false;
		public Number AlignedDomainCopy(Number toCopy) => AlignToDomain(toCopy, Domain);
		public static Number AlignToDomain(Number target, Domain domain)
		{
			var result = target.Clone();
			result.ChangeDomain(domain);
			return result;
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
        public void InterpolateFromOne(Number target, double t)
        {
	        if (target != null)
	        {
		        var targetValue = target.Value;
		        StartValue = targetValue.Start * t;
		        EndValue = (targetValue.End - 1.0) * t + 1.0;
	        }
        }
        public void InterpolateFromOne(Range range, double t)
        {
	        if (range != null)
	        {
		        StartValue = range.Start * t;
		        EndValue = (range.End - 1.0) * t + 1.0;
	        }
        }
        public static void Interpolate(Number source, Number target, Number t, Number result)
        {
	        var sourceValue = source.Value;
	        var targetValue = target.Value;
            var tValue = t.Value;
            result.StartValue = (targetValue.Start - sourceValue.Start) * tValue.Start + sourceValue.Start;
            result.EndValue = (targetValue.End - sourceValue.End) * tValue.End + sourceValue.End;
        }

        // segment comparison considers unot numbers to also have positive segments in the negative space, and
        // unit numbers to be unot in its negative space. So two unit numbers that have a point on both ends of a range,
        // can be considered an ordinary unot number. eg: [+++>....++>] == [....<---...]
        // This should also handle things like gt lt div0 etc

        //  0000	Never			0			FALSE
        //  0001	Both            A ^ B       AND
        //  0010	Only A          A ^ !B      A AND NOT B
        //  0011	A Maybe B       A           A
        //  0100	Only B			!A ^ B      NOT A AND B
        //  0101	B Maybe A       B           B
        //  0110	One of          A xor B     XOR
        //  0111	At least one    A v B       OR
        //  1000	No one          A nor B     NOR
        //  1001	Both or no one  A XNOR B    XNOR
        //  1010	A or no one		!B          NOT B
        //  1011	Not B alone     A v !B      A OR NOT B
        //  1100	B or no one		!A          NOT A
        //  1101	Not A alone		!A v B      NOT A OR B
        //  1110	Not both        A nand B    NAND
        //  1111	Always			1			TRUE

        /*
		function BooleanOperations()
		{
			this.Never = function(a, b) { return 0; };
			this.And = function(a, b) { return a & b; };
			this.LeftandNotRight = function(a, b) { return a & ~b; };
			this.Left = function(a, b) { return a; };
			this.NotLeftAndRight = function(a, b) { return ~a & b; };
			this.Right = function(a, b) { return b; };
			this.Xor = function(a, b) { return a ^ b; };
			this.Or = function(a, b) { return a | b; };
			this.Nor = function(a, b) { return ~(a | b); };
			this.Xnor = function(a, b) { return ~(a ^ b); };
			this.NotRight = function(a, b) { return ~b; };
			this.LeftOrNotRight = function(a, b) { return a | ~b; };// implies
			this.NotLeft = function(a, b) { return ~a; };
			this.RightOrNotLeft = function(a, b) { return ~a | b; }; // implies
			this.Nand = function(a, b) { return ~(a & b); };
			this.Always = function(a, b) { return 1; };
		}

		*/
        // use segments rather than ints
        // convert values to first param's domain's context
        // result in first params's domain

		public NumberSet Never(Number q) => new NumberSet(Domain, Focal.Never(Focal, q.Focal));
		public void Never(Number q, NumberSet result) => result.Reset(Focal.Never(Focal, q.Focal));

		public NumberSet And(Number q) => new NumberSet(Domain, Focal.And(Focal, q.Focal));
		public void And(Number q, NumberSet result) => result.Reset(Focal.And(Focal, q.Focal));

		public NumberSet B_Inhibits_A(Number q) => new NumberSet(Domain, Focal.B_Inhibits_A(Focal, q.Focal));
		public void B_Inhibits_A(Number q, NumberSet result) => result.Reset(Focal.B_Inhibits_A(Focal, q.Focal));

		public NumberSet Transfer_A(Number q) => new NumberSet(Domain, Focal.Transfer_A(Focal, q.Focal));
		public void Transfer_A(Number q, NumberSet result) => result.Reset(Focal.Transfer_A(Focal, q.Focal));

		public NumberSet A_Inhibits_B(Number q) => new NumberSet(Domain, Focal.A_Inhibits_B(Focal, q.Focal));
		public void A_Inhibits_B(Number q, NumberSet result) => result.Reset(Focal.A_Inhibits_B(Focal, q.Focal));

		public NumberSet Transfer_B(Number q) => new NumberSet(Domain, Focal.Transfer_B(Focal, q.Focal));
		public void Transfer_B(Number q, NumberSet result) => result.Reset(Focal.Transfer_B(Focal, q.Focal));

		public NumberSet Xor(Number q) => new NumberSet(Domain, Focal.Xor(Focal, q.Focal));
		public void Xor(Number q, NumberSet result) => result.Reset(Focal.Xor(Focal, q.Focal));

		public NumberSet Or(Number q) => new NumberSet(Domain, Focal.Or(Focal, q.Focal));
		public void Or(Number q, NumberSet result) => result.Reset(Focal.Or(Focal, q.Focal));

		public NumberSet Nor(Number q) => new NumberSet(Domain, Focal.Nor(Focal, q.Focal));
		public void Nor(Number q, NumberSet result) => result.Reset(Focal.Nor(Focal, q.Focal));

		public NumberSet Xnor(Number q) => new NumberSet(Domain, Focal.Xnor(Focal, q.Focal));
		public void Xnor(Number q, NumberSet result) => result.Reset(Focal.Xnor(Focal, q.Focal));

		public NumberSet Not_B(Number q) => new NumberSet(Domain, Focal.Not_B(Focal, q.Focal));
		public void Not_B(Number q, NumberSet result) => result.Reset(Focal.Not_B(Focal, q.Focal));

		public NumberSet B_Implies_A(Number q) => new NumberSet(Domain, Focal.B_Implies_A(Focal, q.Focal));
		public void B_Implies_A(Number q, NumberSet result) => result.Reset(Focal.B_Implies_A(Focal, q.Focal));

		public NumberSet Not_A(Number q) => new NumberSet(Domain, Focal.Not_A(Focal, q.Focal));
		public void Not_A(Number q, NumberSet result) => result.Reset(Focal.Not_A(Focal, q.Focal));

		public NumberSet A_Implies_B(Number q) => new NumberSet(Domain, Focal.A_Implies_B(Focal, q.Focal));
		public void A_Implies_B(Number q, NumberSet result) => result.Reset(Focal.A_Implies_B(Focal, q.Focal));

		public NumberSet Nand(Number q) => new NumberSet(Domain, Focal.Nand(Focal, q.Focal));
		public void Nand(Number q, NumberSet result) => result.Reset(Focal.Nand(Focal, q.Focal));

		public NumberSet Always(Number q) => new NumberSet(Domain, Focal.Always(Focal, q.Focal));
		public void Always(Number q, NumberSet result) => result.Reset(Focal.Always(Focal, q.Focal));


        public Number Clone(bool addToStore = true)
        {
            var result = new Number(Focal.Clone(), Polarity);
            return Domain.AddNumber(result, addToStore);
        }

		public override bool Equals(object obj)
		{
			return obj is Number other && Equals(other);
		}
		public bool Equals(Number value)
		{
			return StartTickPosition.Equals(value.StartTickPosition) && 
			       EndTickPosition.Equals(value.EndTickPosition) &&
			       Focal.StartPosition.Equals(value.StartTickPosition) &&
			       Focal.EndPosition.Equals(value.EndTickPosition);
		}

		public override int GetHashCode()
		{
			unchecked
			{
                var hashCode = StartTickPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EndTickPosition.GetHashCode();
                hashCode = (hashCode * 17) ^ Focal.StartPosition.GetHashCode();
                hashCode = (hashCode * 13) ^ Focal.EndPosition.GetHashCode();
                return hashCode;
			}
		}
        public override string ToString()
		{
			var v = Value;
            var prefix = IsAligned ? "" : "~";
            var startSuffix = IsAligned ? "i" : "r";
            var endSuffix = IsAligned ? "r" : "i";
            return $"{prefix}[{v.Start:0.00}{startSuffix} : {v.End:0.00}{endSuffix}]";
		}
    }
}
