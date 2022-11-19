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
		    if (Agent.SelHighlight.HasHighlight)
		    {
			    Renderer.Canvas.DrawPath(Agent.SelHighlight.ActiveHighlight.HighlightPath(), Renderer.Pens.HoverPen);
		    }
        }
    }
}
