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
        public EvalFlag TestFlags { get; } = (EvalFlag) 0x7FFFFFFF;
        public EvalFlag ResultFlags { get; } = 0;
        public bool Result { get; }

        public bool Update()
        {
            // compare Numbers for the testFlag matches. (maybe just test all, then & with TestFlags)
	        return Result;
        }
    }
}
