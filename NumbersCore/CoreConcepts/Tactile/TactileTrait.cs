namespace NumbersCore.CoreConcepts.Tactile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class TactileTrait : Trait
    {
        public override string Name => "Tactile";

        public static TactileTrait CreateIn(Knowledge knowledge) => (TactileTrait)knowledge.Brain.AddTrait(new TactileTrait());
    }
}
