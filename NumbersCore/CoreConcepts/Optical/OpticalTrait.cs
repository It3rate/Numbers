namespace NumbersCore.CoreConcepts.Optical
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class OpticalTrait : Trait
    {
        private OpticalTrait() : base("Optical") { }

        public static OpticalTrait CreateIn(Knowledge knowledge) => (OpticalTrait)knowledge.Brain.AddTrait(new OpticalTrait());
    }
}
