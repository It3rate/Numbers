using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SKAgentMapper
    {
	    public IAgent Agent { get; set; }
	    public CoreRenderer Renderer { get; set; }
        public SKAgentMapper(IAgent agent, CoreRenderer renderer)
        {
	        Agent = agent;
	        Renderer = renderer;
        }

	    public void Draw()
	    {
		    var sel = Agent.SelHighlight;
		    if (sel.HasHighlight)
		    {
			    var pen = sel.ActiveHighlight.Kind.IsLine() ? Renderer.Pens.HighlightPen : Renderer.Pens.HoverPen;
			    Renderer.Canvas.DrawPath(sel.ActiveHighlight.HighlightPath(), pen);
		    }
        }
    }
}
