using Numbers.Agent;
using Numbers.Renderer;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using SkiaSharp;

namespace Numbers.Mappers
{
	public abstract class SKMapper
	{
		public int Id => MathElement.Id;

        public Brain Brain => Brain.ActiveBrain;
        public IMathElement MathElement { get; protected set; }
	    public abstract SKPoint StartPoint { get; set; }
	    public abstract SKPoint MidPoint { get; }
        public abstract SKPoint EndPoint { get; set; }

        protected Workspace Workspace { get; }
        protected SKWorkspaceMapper WorkspaceMapper
        {
	        get
	        {
		        Agent.DesktopAgent.Current.WorkspaceMappers.TryGetValue(Workspace.Id, out var mapper);
		        return mapper;
	        }
        }
        protected CoreRenderer Renderer => WorkspaceMapper.Renderer;
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public SKMapper(Workspace workspace, IMathElement element)
        {
	        Workspace = workspace;
	        MathElement = element;
        }

        public abstract SKPath GetHighlightAt(Highlight highlight);

    }
}
