using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	/// <summary>
    /// Selection is a selected set of numbers. This can be manually set, or derived from an operation using an number (eg last two results in fibonacci sequence).
    /// </summary>
    public class Selection : IMathElement
    {
	    public Brain Brain { get; }

        public MathElementKind Kind => MathElementKind.Selection;
	    public int Id { get; }
	    public int CreationIndex => Id - (int)Kind - 1;
        private static int SelectionCounter = 1 + (int)MathElementKind.Selection;

        public int[] NumberIds { get; }
        public int Count => NumberIds.Length;

        //public Number NumberAt(int index) => Number.NumberStore[NumberIds[index]];
        public Number this[int i] => Brain.NumberStore[NumberIds[i]];

        public Selection(Brain brain, params int[] numberIds)
        {
	        Id = SelectionCounter++;
	        NumberIds = numberIds;
        }
        public Selection(params Number[] numbers)
        {
	        if (numbers.Length > 0)
	        {
		        Brain = numbers[0].Brain;
		        NumberIds = new int[numbers.Length];
		        for (int i = 0; i < numbers.Length; i++)
		        {
			        NumberIds[i] = numbers[i].Id;
		        }
            }
        }
    }
}
