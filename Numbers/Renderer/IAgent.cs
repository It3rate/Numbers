using System.Windows.Forms;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

	interface IAgent
    {
	    void Clear();

	    bool MouseDown(MouseEventArgs e);
	    bool MouseMove(MouseEventArgs e);
	    bool MouseUp(MouseEventArgs e);
	    bool KeyDown(KeyEventArgs e);
	    bool KeyUp(KeyEventArgs e);
	    bool MouseDoubleClick(MouseEventArgs e);
	    bool MouseWheel(MouseEventArgs e);
    }
}
