using Numbers.Agent;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using SkiaSharp;
// using OpenTK.Graphics.ES30;

namespace Numbers.Mappers
{
	public abstract class SKMapper
	{
		public int Id;
        protected static int idCounter = 0;

		public MouseAgent Agent { get; }
		public Brain Brain => Agent.Brain;
		public Workspace Workspace => Agent.Workspace;
		protected SKWorkspaceMapper WorkspaceMapper => Agent.WorkspaceMapper;
        protected CoreRenderer Renderer => Agent.Renderer;
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public IMathElement MathElement { get; protected set; }
        public SKSegment Guideline { get; private set; } = new SKSegment(0,0,1,1);

        public SKPoint StartPoint
        {
	        get => Guideline.StartPoint;
	        set => Reset(value, EndPoint);
        }
        public SKPoint MidPoint => Guideline.Midpoint;
        public SKPoint EndPoint
        {
	        get => Guideline.EndPoint;
	        set => Reset(StartPoint, value);
        }
        public SKPoint[] EndPoints => new SKPoint[] { StartPoint, EndPoint };

        protected SKMapper(MouseAgent agent, IMathElement element, SKSegment guideline = default)
        {
	        Agent = agent;
	        MathElement = element;
	        Guideline = guideline ?? new SKSegment(0, 0, 1, 1);
        }

        public virtual void Reset(SKPoint startPoint, SKPoint endPoint)
        {
	        Guideline.Reset(startPoint, endPoint);
        }
        public virtual void Reset(SKSegment segment)
        {
	        Guideline.Reset(segment.StartPoint, segment.EndPoint);
        }

        //public abstract SKPoint StartPoint { get; set; }
        //public abstract SKPoint MidPoint { get; }
        //   public abstract SKPoint EndPoint { get; set; }



        public abstract SKPath GetHighlightAt(Highlight highlight);

    }
}
