using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public abstract class SKMapper
    {
        private static int _mapperCounter = 1;
        public int Id { get; private set; }

        public Brain MyBrain => Brain.ActiveBrain;
        public IMathElement MathElement { get; protected set; }
	    public abstract SKPoint StartPoint { get; set; }
	    public abstract SKPoint MidPoint { get; }
        public abstract SKPoint EndPoint { get; set; }

        protected Workspace Workspace { get; }
        protected SKWorkspaceMapper WorkspaceMapper
        {
	        get
	        {
		        Workspace.MyBrain.WorkspaceMappers.TryGetValue(Workspace.Id, out var mapper);
		        return mapper;
	        }
        }
        protected CoreRenderer Renderer => WorkspaceMapper.Renderer;
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public SKMapper(Workspace workspace, IMathElement element)
        {
	        Id = element.Id;// _mapperCounter++;
	        Workspace = workspace;
	        MathElement = element;
        }

        public abstract SKPath GetHighlightAt(Highlight highlight);

    }
}
