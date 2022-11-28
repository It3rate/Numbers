using Numbers.Agent;
using Numbers.Renderer;
using NumbersCore.Primitives;

namespace Numbers.Mappers
{
	public class SKAgentMapper 
    {
	    public CoreRenderer Renderer { get; set; }
	    public DesktopAgent Agent { get; }
	    public Workspace Workspace => Agent.Workspace;
	    public SKWorkspaceMapper WorkspaceMapper => Agent.WorkspaceMapper;

        public SKAgentMapper(DesktopAgent desktopAgent, CoreRenderer renderer)
        {
	        Agent = desktopAgent;
	        desktopAgent.AgentMapper = this;
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
