namespace NumbersCore.CoreConcepts.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.CoreConcepts.Counter;
    using NumbersCore.CoreConcepts.Tactile;
    using NumbersCore.Primitives;

    public class SpatialTrait : Trait
    {
        private SpatialTrait() : base("Spatial") { }

        public static SpatialTrait CreateIn(Knowledge knowledge) => (SpatialTrait)knowledge.Brain.AddTrait(new SpatialTrait());
        public override Trait Clone() => CopyPropertiesTo(new SpatialTrait());
    }
}
