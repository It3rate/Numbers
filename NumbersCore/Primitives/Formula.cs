using System.Collections.Generic;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	public class Formula : IMathElement
    {
        public MathElementKind Kind => MathElementKind.Formula;
        public int Id { get; set; }
        public int CreationIndex => Id - (int)Kind - 1;
        public Brain Brain { get; }

        //private static int formulaCounter = 1 + (int)MathElementKind.Formula;
        //   private Number _repeatIndex; // need a trait in the dictionary that just represents values
        //   // account for repeat of formula, use stack to enable back selection
        //   public Number SelectionRange { get; } // find multiplicand(s) on stack. -n from end, n from start, 0 is all?
        //   public Number RepeatRange { get; } // find multiplier(s) on stack
        //   public Number Repeat { get; } // iterations (need to allow evaluation halting, yield)
        public Stack<Transform> TransformStack { get; } = new Stack<Transform>();

        public bool CanRepeat() { return true;}

        public void ApplyStart() { }
        public void ApplyEnd() { }
        public void ApplyPartial(long tickOffset) { }

        public Formula(Brain brain)
        {
	        Brain = brain;
	        Id = Brain.NextFormulaId();
        }
    }
}
