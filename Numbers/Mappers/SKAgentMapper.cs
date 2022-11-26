using Numbers.Agent;
using Numbers.Renderer;

namespace Numbers.Mappers
{
	public class SKAgentMapper
    {
	    public Agent.DesktopAgent DesktopAgent { get; set; }
	    public CoreRenderer Renderer { get; set; }

	    public SKAgentMapper(Agent.DesktopAgent desktopAgent, CoreRenderer renderer)
        {
	        DesktopAgent = desktopAgent;
	        desktopAgent.AgentMapper = this;
            Renderer = renderer;
        }

	    public void Draw()
	    {
		    var sel = DesktopAgent.SelHighlight;
		    if (sel.HasHighlight)
		    {
			    var pen = sel.ActiveHighlight.Kind.IsLine() ? Renderer.Pens.HighlightPen : Renderer.Pens.HoverPen;
			    Renderer.Canvas.DrawPath(sel.ActiveHighlight.HighlightPath(), pen);
		    }
        }
    }
}
