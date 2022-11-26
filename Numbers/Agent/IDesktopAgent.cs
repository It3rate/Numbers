using System.Collections.Generic;
using System.Windows.Forms;
using NumbersCore.Primitives;

namespace Numbers.Agent
{
	public interface IDesktopAgent : IAgent
    {
	    bool IsPaused { get; set; }

        HighlightSet SelBegin { get; }
        HighlightSet SelCurrent { get; }
        HighlightSet SelHighlight { get; }
        HighlightSet SelSelection { get; }

        bool LockBasisOnDrag { get; set; }
	    bool LockTicksOnDrag { get; set; }

        bool MouseDown(MouseEventArgs e);
	    bool MouseMove(MouseEventArgs e);
	    bool MouseUp(MouseEventArgs e);
	    bool KeyDown(KeyEventArgs e);
	    bool KeyUp(KeyEventArgs e);
	    bool MouseDoubleClick(MouseEventArgs e);
	    bool MouseWheel(MouseEventArgs e);
    }
}
