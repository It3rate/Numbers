namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum BoolState
    {
        FalseNegative,
        TruePositive,
        FalsePositive,
        TrueNegative,
        Underflow,
        Overflow,
        Unknown,
    }
    public static class BoolStateExtension
    {
        public static BoolState Obverse(this BoolState state)
        {
            switch (state)
            {
                case BoolState.FalseNegative: return BoolState.FalsePositive;
                case BoolState.FalsePositive: return BoolState.FalseNegative;
                case BoolState.TrueNegative: return BoolState.TruePositive;
                case BoolState.TruePositive: return BoolState.TrueNegative;
                case BoolState.Underflow: return BoolState.Underflow;
                case BoolState.Overflow: return BoolState.Overflow;
            }
            return BoolState.Unknown;
        }
        public static BoolState Negate(this BoolState state)
        {
            switch (state)
            {
                case BoolState.FalseNegative: return BoolState.TrueNegative;
                case BoolState.TrueNegative: return BoolState.FalseNegative;
                case BoolState.FalsePositive: return BoolState.TruePositive;
                case BoolState.TruePositive: return BoolState.FalsePositive;
                case BoolState.Underflow: return BoolState.Overflow;
                case BoolState.Overflow: return BoolState.Underflow;
            }
            return BoolState.Unknown;
        }
        public static bool BoolValue(this BoolState state) => state == BoolState.FalsePositive ? false : true;
        public static bool IsBool(this BoolState state) => state == BoolState.FalsePositive || state == BoolState.TruePositive;
        public static bool IsOutOfRange(this BoolState state) => state == BoolState.Underflow || state == BoolState.Overflow;
        public static bool AreBool(params BoolState[] states)
        {
            var result = true;
            foreach(var state in states)
            {
                if (!state.IsBool())
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
        public static BoolState Invert(this BoolState state)
        {
            var result = BoolState.Unknown;
            switch (state)
            {
                case BoolState.FalsePositive:
                    result = BoolState.TruePositive;
                    break;
                case BoolState.TruePositive:
                    result = BoolState.FalsePositive;
                    break;
                case BoolState.Underflow:
                    result = BoolState.Overflow;
                    break;
                case BoolState.Overflow:
                    result = BoolState.Underflow;
                    break;
            }
            return result;
        }
    }
}
