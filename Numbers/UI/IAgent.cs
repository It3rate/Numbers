using System.Collections.Generic;
using System.Windows.Forms;
using Numbers.Core;

namespace Numbers.UI
{
	public interface IAgent
    {
	    Brain MyBrain { get; }
	    Workspace Workspace { get; }
	    bool IsPaused { get; set; }

        HighlightSet SelBegin { get; }
        HighlightSet SelCurrent { get; }
        HighlightSet SelHighlight { get; }
        HighlightSet SelSelection { get; }

        Stack<Selection> SelectionStack { get; }
	    Stack<Formula> FormulaStack { get; }
	    Stack<Number> ResultStack { get; }

        bool LockBasisOnDrag { get; set; }
	    bool LockTicksOnDrag { get; set; }

        bool MouseDown(MouseEventArgs e);
	    bool MouseMove(MouseEventArgs e);
	    bool MouseUp(MouseEventArgs e);
	    bool KeyDown(KeyEventArgs e);
	    bool KeyUp(KeyEventArgs e);
	    bool MouseDoubleClick(MouseEventArgs e);
	    bool MouseWheel(MouseEventArgs e);

	    void ClearAll();
    }
}
