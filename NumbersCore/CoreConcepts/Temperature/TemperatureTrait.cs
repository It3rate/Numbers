namespace NumbersCore.CoreConcepts.Temperature
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.CoreConcepts.Time;
    using NumbersCore.Primitives;

    public class TemperatureTrait : Trait
    {
        private TemperatureTrait() : base("Temperature") { }

        //public static TemperatureTrait CreateIn(Knowledge knowledge) => (TemperatureTrait)knowledge.Brain.AddTrait(new TemperatureTrait("Temperature"));
        public new TemperatureTrait Clone() => (TemperatureTrait)CopyPropertiesTo(new TemperatureTrait());
    }
}
