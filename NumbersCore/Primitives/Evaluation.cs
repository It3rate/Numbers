namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum EvalFlag
    {
        None = 0,
        Trait,
        Domain,
        BasisFocal,
        BasisOrigin,
        BasisResolution,
        MinMax,
        Length,
        HasOverlap,
        Equal,
        Contains,
        ContainedBy,
        WhollyGreater,
        WhollyLess,
        PartiallyGreater,
        PartiallyLess,
        Wrapped,
        Bounced,
        Overflowed,
    }
    public class Evaluation
    {
        // Flags:
        // same trait, domain, basis, resolution, minmax
        // direction (same as basis, same abs)
        // length
        // overlap
        // per endpoint - gt, lt, eq, per basis/abs
        // containment
        // same mod, whole
        // has wrapped/bounced/overflowed

        public Number Source { get; }
        public Number Target { get; }
        public NumberSet Result { get; }
        public EvalFlag TestFlags { get; } = (EvalFlag) 0x7FFFFFFF;
        public EvalFlag ResultFlags { get; private set; }

        public Evaluation(Number source, Number target)
        {
	        Source = source;
	        Target = target;
        }

        /// <summary>
        /// Tests all the flags set in TestFlags, sets the ResultFlags to the set bits.
        /// </summary>
        /// <returns>Returns true if any of the flags are set.</returns>
        public bool EvaluateFlags()
        {
            // compare Numbers for the testFlag matches. (maybe just test all, then & with TestFlags)
            ResultFlags = EvalFlag.None;

            ResultFlags |= TargetContainsSource() ? EvalFlag.Contains : 0;

            return (int)ResultFlags > 0;
        }


        public bool TargetContainsSource() => Target?.FullyContains(Source) ?? false;
    }
}
