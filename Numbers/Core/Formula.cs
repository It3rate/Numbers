namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Formula : TransformBase
    {
	    public MathElementKind Kind => MathElementKind.Formula;
	    private static int formulaCounter = 1 + (int)MathElementKind.Formula;

        // account for repeat of formula, use stack to enable back selection
        public Number SelectionRange { get; } // find multiplicand(s) on stack. -n from end, n from start, 0 is all?
        public Number RepeatRange { get; } // find multiplier(s) on stack
        public Number Repeat { get; } // iterations (need to allow evaluation halting, yield)
        public Stack<Transform> TransformStack { get; } = new Stack<Transform>();

        private Number _repeatIndex; // need a trait in the dictionary that just represents values
        public bool CanRepeat() { return true;}

        public override void ApplyStart() { }
        public override void ApplyEnd() { }
        public override void ApplyPartial(long tickOffset) { }

        public Formula(Number repeat, TransformKind kind) : base(repeat, kind)
        {
        }
    }
}
