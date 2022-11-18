using System.Globalization;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public struct Range
    {
        public static readonly Range Empty = new Range(0.0, 1.0, true);
        public static readonly Range Zero = new Range(0.0, 0.0);
        public static readonly Range Unit = new Range(0.0, 1.0);
        public static readonly Range Unot = new Range(1.0, 0.0);
        public static readonly Range Umid = new Range(-0.5, 0.5);
        public static readonly Range Max = new Range(double.MaxValue, double.MaxValue);
        public static readonly Range Min = new Range(double.MinValue, double.MinValue);

        public double Start { get; set; }
        public double End { get; set; }
        private readonly bool _hasValue;

        public float StartF => (float)Start;
        public float EndF => (float)End;

        public Range(int start, int end)
        {
            _hasValue = true;
            Start = start;
            End = end;
        }
        public Range(double start, double end)
        {
            _hasValue = true;
            Start = start;
            End = end;
        }
        public Range(Range value)
        {
            _hasValue = true;
            Start = value.Start;
            End = value.End;
        }

        private Range(double start, double end, bool isEmpty) // empty ctor
        {
            _hasValue = !isEmpty;
            Start = start;
            End = end;
        }

        public bool IsEmpty => _hasValue;

        public Range Clone() => new Range(Start, End);

        public double Length => Range.AbsLength(this);
        public double DirectedLength() => Range.DirectedLength(this);
        public double AbsLength() => Range.AbsLength(this);
        public Range Conjugate() => Range.Conjugate(this);
        public Range Reciprocal() => Range.Reciprocal(this);
        public Range Square() => Range.Square(this);
        public Range Normalize() => Range.Normalize(this);
        public Range NormalizeTo(Range value) => Range.NormalizeTo(this, value);

        public bool IsZero => End == 0 && Start == 0;
        public bool IsZeroLength => (End == Start);
        public bool IsForward => End >= Start;
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


        public static Range operator +(Range a, double value) => new Range(a.End + value, a.Start + value);
        public static Range operator -(Range a, double value) => new Range(a.End - value, a.Start - value);
        public static Range operator *(Range a, double value) => new Range(a.End * value, a.Start * value);
        public static Range operator /(Range a, double value) => new Range(value == 0 ? double.MaxValue : a.End / value, value == 0 ? double.MaxValue : a.Start / value);

        public static Range Negate(Range value) => -value;
        public static Range Add(Range left, Range right) => left + right;
        public static Range Subtract(Range left, Range right) => left - right;
        public static Range Multiply(Range left, Range right) => left * right;
        public static Range Divide(Range dividend, Range divisor) => dividend / divisor;

        public static Range operator -(Range value) => new Range(-value.Start, -value.End);
        public static Range operator +(Range left, Range right) => new Range(left.Start + right.Start, left.End + right.End);
        public static Range operator -(Range left, Range right) => new Range(left.Start - right.Start, left.End - right.End);
        public static Range operator *(Range left, Range right) => new Range(left.Start * right.End + left.End * right.Start, left.End * right.End - left.Start * right.Start);
        public static Range operator /(Range left, Range right)
        {
            double real1 = left.End;
            double imaginary1 = left.Start;
            double real2 = right.End;
            double imaginary2 = right.Start;
            if (Math.Abs(imaginary2) < Math.Abs(real2))
            {
                double num = imaginary2 / real2;
                return new Range((imaginary1 - real1 * num) / (real2 + imaginary2 * num), (real1 + imaginary1 * num) / (real2 + imaginary2 * num));
            }
            double num1 = real2 / imaginary2;
            return new Range((-real1 + imaginary1 * num1) / (imaginary2 + real2 * num1), (imaginary1 + real1 * num1) / (imaginary2 + real2 * num1));
        }

        public static double DirectedLength(Range value) => value.End - value.Start;
        public static double AbsLength(Range value) => Math.Abs(value.End - value.Start);
        public static Range Conjugate(Range a) => new Range(a.End, -a.Start);
        public static Range Reciprocal(Range value) => value.End == 0.0 && value.Start == 0.0 ? Range.Zero : Range.Unit / value;
        public static Range Square(Range a) => new Range(a.Start * a.Start + (a.End * a.End) * -1, 0); // value * value;
        public static Range Normalize(Range value) => value.IsZeroLength ? new Range(0.5, 0.5) : value / value;
        public static Range NormalizeTo(Range from, Range to) => from.Normalize() * to;

        public static double Abs(Range value)
        {
            if (double.IsInfinity(value.End) || double.IsInfinity(value.Start))
                return double.PositiveInfinity;
            double num1 = Math.Abs(value.End);
            double num2 = Math.Abs(value.Start);
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

        public static bool operator ==(Range left, Range right) => left.End == right.End && left.Start == right.Start;
        public static bool operator !=(Range left, Range right) => left.End != right.End || left.Start != right.Start;
        public override bool Equals(object obj)
        {
            return obj is Range other && Equals(other);
        }
        public bool Equals(Range value)
        {
            return Start.Equals(value.Start) && End.Equals(value.End);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Start.GetHashCode();
                hashCode = (hashCode * 397) ^ End.GetHashCode();
                return hashCode;
            }
        }


        public override string ToString() => string.Format((IFormatProvider)CultureInfo.CurrentCulture, "({0}, {1})", new object[2]
        {
            (object) this.Start,
            (object) this.End
        });
    }
}
