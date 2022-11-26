namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAgent
    {
	    Brain Brain { get; }
	    Workspace Workspace { get; set; }

	    Stack<Selection> SelectionStack { get; }
	    Stack<Formula> FormulaStack { get; }
	    Stack<Number> ResultStack { get; }

	    void ClearAll();
    }
}
