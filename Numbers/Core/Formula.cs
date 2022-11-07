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
        public List<Transform> Transforms { get; } = new List<Transform>();

        public override void ApplyStart() { }
        public override void ApplyEnd() { }
        public override void ApplyPartial(long tickOffset) { }

        public Formula(Number repeat, TransformKind kind) : base(repeat, kind)
        {
        }
    }
}
