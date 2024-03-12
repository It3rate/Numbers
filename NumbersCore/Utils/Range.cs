using System;
using NumbersCore.Primitives;

namespace NumbersCore.Utils
{
    /// <summary>
    /// A segment of two values, with the non aligned one using the inverted polarity.
    /// </summary>
	public struct Range
    {
        public static readonly Range Empty = new Range(0.0, 1.0, true);
        public static readonly Range Zero = new Range(0.0, 0.0);
        public static readonly Range Unit = new Range(0.0, 1.0);
        public static readonly Range Unot = new Range(1.0, 0.0);
        public static readonly Range Umid = new Range(-0.5, 0.5);
        public static readonly Range MaxRange = new Range(double.MaxValue, double.MaxValue);
        public static readonly Range MinRange = new Range(double.MinValue, double.MinValue);
        public static readonly double Tolerance = 0.000000001;

        public Polarity Polarity { get; set; }
        public int PolarityDirection => IsAligned ? 1 : -1;
        public bool IsAligned => Polarity == Polarity.Aligned;
        public bool IsInverted => Polarity == Polarity.Inverted;

        public double Start { get; set; }
        public double End { get;
            set; }
        private readonly bool _hasValue;
        public double UnitValue
        {
            get => IsAligned ? End : Start;
            set { if (IsAligned) { End = value; } else { Start = value; } }
        }
        public double UnotValue
        {
            get => IsAligned ? Start : End;
            set { if (IsAligned) { Start = value; } else { End = value; } }
        }
        public float StartF => (float)Start;
        public float EndF => (float)End;
        public float RenderStart => (float)(IsAligned ? Start : -Start);
        public float RenderEnd => (float)(IsAligned ? End : -End);

        public Range(int start, int end, bool isAligned = true)
        {
            _hasValue = true;
            Start = start;
            End = end;
            Polarity = isAligned ? Polarity.Aligned : Polarity.Inverted;
        }
        public Range(double start, double end, bool isAligned = true)
        {
            _hasValue = true;
            Start = start;
            End = end;
            Polarity = isAligned ? Polarity.Aligned : Polarity.Inverted;
        }
        public Range(Range value, bool isAligned = true)
        {
            _hasValue = true;
            Start = value.Start;
            End = value.End;
            Polarity = isAligned ? Polarity.Aligned : Polarity.Inverted;
        }

        private Range(double start, double end, bool isEmpty, bool isAligned = true) // empty ctor
        {
            _hasValue = !isEmpty;
            Start = start;
            End = end;
            Polarity = isAligned ? Polarity.Aligned : Polarity.Inverted;
        }

        public bool IsEmpty => _hasValue;
        public double Min => Start >= End ? End : Start;
        public double Max => End >= Start ? End : Start;
        public float MinF => (float)Min;
        public float MaxF => (float)Max;
        public double Length => Range.AbsLength(this);
        public double DirectedLength() => Range.DirectedLength(this);
        public double AbsLength() => Range.AbsLength(this);
        public double TAtValue(double value)
        {
            var dist = value - (-Start);
            return dist / DirectedLength();
        }
        public void InvertPolarity()
        {
            Polarity = (Polarity == Polarity.Aligned) ? Polarity.Inverted : Polarity.Aligned;
        }
        public void InvertRange()
        {
            Start = -Start;
            End = -End;
        }
        public void InvertPolarityAndRange()
        {
            Start = -Start;
            End = -End;
            Polarity = (Polarity == Polarity.Aligned) ? Polarity.Inverted : Polarity.Aligned;
        }
        public Range Negation() => Range.Negation(this);
        public Range Conjugate() => Range.Conjugate(this);
        public Range Reciprocal() => Range.Reciprocal(this);
        public Range Square() => Range.Square(this);
        public Range Normalize() => Range.Normalize(this);
        public Range NormalizeTo(Range value) => Range.NormalizeTo(this, value);
        public Range ClampInner() => Range.ClampInner(this);
        public Range ClampOuter() => Range.ClampOuter(this);
        public Range Round() => Range.Round(this);
        public Range PositiveDirection() => PositiveDirection(this);
        public Range NegativeDirection() => NegativeDirection(this);
        public bool IsZero => End == 0 && Start == 0;
        public bool IsZeroLength => (End == Start);
        public bool IsForward => End - Start > Tolerance;
        public bool IsBackward => Start - End > Tolerance;
        public bool IsPoint => Length < Tolerance;
        public double Direction => End >= Start ? 1.0 : -1.0;

        // because everything is segments, can add 'prepositions' (before, after, between, entering, leaving, near etc)
        public bool IsWithin(Range value) => Start >= value.Start && End <= value.End;
        public bool IsBetween(Range value) => Start > value.Start && End < value.End;
        public bool IsBefore(Range value) => Start < value.Start && End < value.Start;
        public bool IsAfter(Range value) => Start > value.End && End > value.End;
        public bool IsBeginning(Range value) => Start <= value.Start && End > value.Start;
        public bool IsEnding(Range value) => Start >= value.End && End > value.End;
        public bool IsTouching(Range value) => (Start >= value.Start && Start <= value.End) || (End >= value.Start && End <= value.End);
        public bool IsNotTouching(Range value) => !IsTouching(value);


        public static Range operator +(Range a, double value) => a + new Range(0, value);
        public static Range operator -(Range a, double value) => a - new Range(0, value);
        public static Range operator *(Range a, double value) => a * new Range(0, value);
        public static Range operator /(Range a, double value) => a / new Range(0, value);

        public static Range PositiveDirection(Range value) => new Range(value.Min, value.Max);
        public static Range NegativeDirection(Range value) => new Range(value.Max, value.Min);
        public static Range Negate(Range value) => -value;
        public static Range Add(Range left, Range right) => left + right;
        public static Range Subtract(Range left, Range right) => left - right;
        public static Range Multiply(Range left, Range right) => left * right;
        public static Range Divide(Range dividend, Range divisor) => dividend / divisor;

        public static Range operator -(Range value) => new Range(-value.Start, -value.End);
        public static Range operator +(Range left, Range right)
        {
            var result = right.Clone();
            result.UnitValue += left.UnitValue;
            result.UnotValue += left.UnotValue;
            return result;
        }
        public static Range operator -(Range left, Range right)
        {
            var result = left.Clone(); 
            result.UnitValue -= right.UnitValue; 
            result.UnotValue -= right.UnotValue;
            return result;
        }
        public static Range operator *(Range left, Range right)
        {
            var result = new Range(left.Start * right.End + left.End * right.Start, left.End * right.End - left.Start * right.Start);
            result.Polarity = left.Polarity;
            result.SolvePolarityWith(right.Polarity); // probably can compute this properly with unit/unot values.
            return result;
        }
        public static Range operator /(Range left, Range right)
        {
            Range result;
            double real1 = left.End;
            double imaginary1 = left.Start;
            double real2 = right.End;
            double imaginary2 = right.Start;
            if (Math.Abs(imaginary2) < Math.Abs(real2))
            {
                double num = imaginary2 / real2;
                result = new Range((imaginary1 - real1 * num) / (real2 + imaginary2 * num), (real1 + imaginary1 * num) / (real2 + imaginary2 * num));
            }
            else
            {
                double num1 = real2 / imaginary2;
                result = new Range((-real1 + imaginary1 * num1) / (imaginary2 + real2 * num1), (imaginary1 + real1 * num1) / (imaginary2 + real2 * num1));
            }
            result.Polarity = left.Polarity;
            result.SolvePolarityWith(right.Polarity);
            return result;
        }
        public void SolvePolarityWith(Polarity right)
        {
            if (Polarity == Polarity.Inverted && right == Polarity.Inverted)
            {
                InvertPolarityAndRange();
            }
            else if (Polarity == Polarity.Aligned && right == Polarity.Inverted)
            {
                InvertPolarity();
            }
        }
        public static double DirectedLength(Range a) => a.End + a.Start;
        public static double AbsLength(Range a) => Math.Abs(a.End + a.Start);

        public static Range Negation(Range a) => new Range(-a.Start, -a.End);
        public static Range Conjugate(Range a) => new Range(a.Start, -a.End);
        public static Range Reciprocal(Range a) => a.End == 0.0 && a.Start == 0.0 ? Range.Zero : Range.Unit / a;
        public static Range Square(Range a) => a * a;
        public static Range Normalize(Range a) => a.IsZeroLength ? new Range(0.5, 0.5) : a / a;
        public static Range NormalizeTo(Range from, Range to) => from.Normalize() * to;

        public static Range ClampInner(Range a) => new Range(Math.Floor(Math.Abs(a.Start)) * Math.Sign(a.Start), Math.Floor(Math.Abs(a.End)) * Math.Sign(a.End));
        public static Range ClampOuter(Range a) => new Range(Math.Ceiling(Math.Abs(a.Start)) * Math.Sign(a.Start), Math.Ceiling(Math.Abs(a.End)) * Math.Sign(a.End));
        public static Range Round(Range a) => new Range(Math.Round(a.Start), Math.Round(a.End));

        public static double Abs(Range a)
        {
	        if (double.IsInfinity(a.End) || double.IsInfinity(a.Start))
		        return double.PositiveInfinity;
	        double num1 = Math.Abs(a.End);
	        double num2 = Math.Abs(a.Start);
	        if (num1 > num2)
	        {
		        double num3 = num2 / num1;
		        return num1 * Math.Sqrt(1.0 + num3 * num3);
	        }
	        if (num2 == 0.0)
		        return num1;
	        double num4 = num1 / num2;
	        return num2 * Math.Sqrt(1.0 + num4 * num4);
        }
        
        public static Range Pow(Range value, Range power)
        {
            if (power == Range.Zero)
                return Range.Unit;
            if (value == Range.Zero)
                return Range.Zero;
            double real1 = value.End;
            double imaginary1 = value.Start;
            double real2 = power.End;
            double imaginary2 = power.Start;
            double num1 = Range.Abs(value);
            double num2 = Math.Atan2(imaginary1, real1);
            double num3 = real2 * num2 + imaginary2 * Math.Log(num1);
            double num4 = Math.Pow(num1, real2) * Math.Pow(Math.E, -imaginary2 * num2);
            return new Range(num4 * Math.Sin(num3), num4 * Math.Cos(num3));
        }
        public static Range Pow(Range value, double power) => Range.Pow(value, new Range(0.0, power));
        private static Range Scale(Range value, double factor) => new Range(factor * value.Start, factor * value.End);

        public bool IsSameDirection(Range range) => Math.Abs(range.Direction + Direction) > Tolerance;
        public bool FullyContains(Range toTest, bool includeEndpoints = true)
        {
	        bool result = false;
	        if (IsSameDirection(toTest))
	        {
		        var pd = PositiveDirection();
		        var pdTest = toTest.PositiveDirection();
		        result = includeEndpoints ? pdTest.IsWithin(pd) : pdTest.IsBetween(pd);
            }
	        return result;
        }

        public float Midpoint() => (EndF - StartF) / 2f + StartF;
        public float SampleRandom(Random rnd) => (EndF - StartF) * (float)rnd.NextDouble() + StartF;
        public float SampleStdDev() => (EndF - StartF) / 2f + StartF; // todo: add standard deviation sampler

        public Range Clone() => new Range(Start, End, IsAligned);

        public static bool operator ==(Range a, Range b) // value type, so no nulls
        {
            return a.Equals(b);
        }
        public static bool operator !=(Range a, Range b) => !(a == b);
        public override bool Equals(object obj)
        {
            return obj is Range other && Equals(other);
        }
        public bool Equals(Range value)
        {
            return Start.Equals(value.Start) && End.Equals(value.End) && Polarity.Equals(value.Polarity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                hashCode = (hashCode * 17) ^ Polarity.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            var prefix = Polarity==Polarity.Inverted ? "~" : "";
            return $"{prefix}[{Start:0.00}->{End:0.00}]";
        }

    }
}
