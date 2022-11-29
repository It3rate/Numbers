using System.Collections.Generic;
using NumbersCore.Primitives;

namespace NumbersCore.Utils
{
	public interface IAgent
    {
	    Brain Brain { get; }
	    Workspace Workspace { get; }

	    Stack<Selection> SelectionStack { get; }
	    Stack<Formula> FormulaStack { get; }
	    Stack<Number> ResultStack { get; }

	    void ClearAll();
    }
}
