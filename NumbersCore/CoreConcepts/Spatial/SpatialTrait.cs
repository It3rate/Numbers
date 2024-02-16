namespace NumbersCore.CoreConcepts.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class SpatialTrait : Trait
    {
        public override string Name => "Spatial";

        public static SpatialTrait CreateIn(Knowledge knowledge) => (SpatialTrait)knowledge.Brain.AddTrait(new SpatialTrait());
    }
}
