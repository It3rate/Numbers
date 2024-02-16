namespace NumbersCore.CoreConcepts.Temperature
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class TemperatureTrait : Trait
    {
        public override string Name => "Temperature";

        public static TemperatureTrait CreateIn(Knowledge knowledge) => (TemperatureTrait)knowledge.Brain.AddTrait(new TemperatureTrait());
    }
}
}
