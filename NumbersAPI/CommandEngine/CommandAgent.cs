using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace NumbersAPI.CommandEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandAgent : IAgent
    {
	    public Brain Brain => Workspace.Brain;
	    public virtual Workspace Workspace { get; }

	    public Stack<Selection> SelectionStack { get; }
	    public Stack<Formula> FormulaStack { get; }
	    public Stack<Number> ResultStack { get; }

	    public CommandAgent(Workspace workspace)
	    {
		    Workspace = workspace;
	    }

        public virtual void ClearAll()
	    {
		    SelectionStack?.Clear();
		    FormulaStack?.Clear();
		    ResultStack?.Clear();
        }
    }
}
