namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Selection is a selected set of numbers. This can be manually set, or derived from an operation using an number (eg last two results in fibonacci sequence).
    /// </summary>
    public class Selection
    {
        public int[] NumberIds { get; }
        public int Count => NumberIds.Length;

        //public Number NumberAt(int index) => Number.NumberStore[NumberIds[index]];
        public Number this[int i] => Number.NumberStore[NumberIds[i]];

        public Selection(params int[] numberIds)
        {
	        NumberIds = numberIds;
        }
        public Selection(params Number[] numbers)
        {
	        NumberIds = new int[numbers.Length];
	        for (int i = 0; i < numbers.Length; i++)
	        {
		        NumberIds[i] = numbers[i].Id;
	        }
        }
    }
}
