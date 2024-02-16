using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Counter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This domain can characterize all counters (start, end?, step, events, increment/decrement methods, completion Tests).
    /// The domain can apply Increment() to all numbers it contains in one step.
    /// It can enforce the types of numbers that it can contain (maybe require them to have an Increment method?)
    /// </summary>
    public class CounterDomain : Domain
    {
        public static CounterDomain UpCounterDomain { get; } = new CounterDomain(Focal.CreateZeroFocal(1), new Focal(0, long.MaxValue), "CounterUp");
        public static CounterDomain UpDownDomain { get; } = new CounterDomain(Focal.CreateZeroFocal(1), Focal.MinMaxFocal, "CounterUpDown");

        public long Start { get; set; } = 0;
        public long End { get; set; } = long.MaxValue;
        public long Step { get; set; } = 1;
        public CounterDomain(Focal basisFocal, Focal maxFocal, string name) : base(CounterTrait.Instance, basisFocal, maxFocal, name)
        {
            IsVisible = false;
        }

        public long[] Increment()
        {
            var result = new List<long>();
            foreach(var num in NumberStore.Values)
            {
                var ep = num.Focal.EndPosition;
                if(ep < End && ep + Step > End)
                {
                    num.Focal.EndPosition = End;
                    // End event for num
                }
                else
                {
                    num.Focal.EndPosition = ep + Step;
                }
                result.Add(num.Focal.EndPosition);
            }
            return result.ToArray();
        }
        public override Number CreateDefaultNumber(bool addToStore = true)
        {
            var num = new Number(new Focal(0, 1));
            return AddNumber(num, addToStore);
        }

        public void Reset()
        {
            foreach (var num in NumberStore.Values)
            {
                num.Focal.EndPosition = 0;
            }
        }
    }
}
