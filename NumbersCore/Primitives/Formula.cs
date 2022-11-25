using System.Collections.Generic;

namespace NumbersCore.Primitives
{
	public class Formula : TransformBase
    {
	    public override MathElementKind Kind => MathElementKind.Formula;
	    //private static int formulaCounter = 1 + (int)MathElementKind.Formula;
     //   private Number _repeatIndex; // need a trait in the dictionary that just represents values
     //   // account for repeat of formula, use stack to enable back selection
     //   public Number SelectionRange { get; } // find multiplicand(s) on stack. -n from end, n from start, 0 is all?
     //   public Number RepeatRange { get; } // find multiplier(s) on stack
     //   public Number Repeat { get; } // iterations (need to allow evaluation halting, yield)
        public Stack<Transform> TransformStack { get; } = new Stack<Transform>();

        public bool CanRepeat() { return true;}

        public override void ApplyStart() { }
        public override void ApplyEnd() { }
        public override void ApplyPartial(long tickOffset) { }

        public Formula(Number repeat, TransformKind kind) : base(repeat, kind)
        {
        }
    }
}
