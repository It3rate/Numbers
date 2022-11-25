using Numbers.Agent;
using Numbers.Renderer;

namespace Numbers.Mappers
{
	public class SKAgentMapper
    {
	    public Agent.Agent Agent { get; set; }
	    public CoreRenderer Renderer { get; set; }

	    public SKAgentMapper(Agent.Agent agent, CoreRenderer renderer)
        {
	        Agent = agent;
	        agent.AgentMapper = this;
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
