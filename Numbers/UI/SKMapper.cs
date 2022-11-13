using System.Security.Permissions;
using Numbers.Core;
using Numbers.Mind;
using Numbers.Renderer;
using SkiaSharp;

namespace Numbers.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
        protected RendererBase Renderer => Workspace.Renderer;
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public SKMapper(Workspace workspace, IMathElement element)
        {
	        Id = _mapperCounter++;
	        Workspace = workspace;
	        MathElement = element;
        }

        public abstract SKPath HighlightAt(float t, SKPoint targetPoint);

        //public abstract void Draw();
    }
}
