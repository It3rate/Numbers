using System.Collections.Generic;
using System.Windows.Forms;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace Numbers.Agent
{
	public interface IMouseAgent : IAgent
    {
	    bool IsPaused { get; set; }

        HighlightSet SelBegin { get; }
        HighlightSet SelCurrent { get; }
        HighlightSet SelHighlight { get; }
        HighlightSet SelSelection { get; }

        bool MouseDown(MouseEventArgs e);
	    bool MouseMove(MouseEventArgs e);
	    bool MouseUp(MouseEventArgs e);
	    bool KeyDown(KeyEventArgs e);
	    bool KeyUp(KeyEventArgs e);
	    bool MouseDoubleClick(MouseEventArgs e);
	    bool MouseWheel(MouseEventArgs e);
    }
}
