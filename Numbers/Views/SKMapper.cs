using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public abstract class SKMapper
    {
        private static int _mapperCounter = 1;
        public int Id { get; private set; }

        public IMathElement MathElement { get; protected set; }
	    public abstract SKPoint StartPoint { get; set; }
	    public abstract SKPoint MidPoint { get; }
        public abstract SKPoint EndPoint { get; set; }

        //   public SKPoint[] SalientPoints { get; private set; }
        //public SKSegment[] SalientSegments { get; private set; }
        //public SKPath[] SalientAreas { get; private set; }

        protected Workspace Workspace { get; }
        protected SKWorkspaceMapper WorkspaceMapper
        {
	        get
	        {
		        Workspace.MyBrain.WorkspaceMappers.TryGetValue(Workspace.Id, out var mapper);
		        return mapper;
	        }
        }
        protected RendererBase Renderer => WorkspaceMapper.Renderer;
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public SKMapper(Workspace workspace, IMathElement element)
        {
	        Id = element.Id;// _mapperCounter++;
	        Workspace = workspace;
	        MathElement = element;
        }

        public abstract SKPath HighlightAt(float t, SKPoint targetPoint);

        //public abstract void DrawNumber();
    }
}
