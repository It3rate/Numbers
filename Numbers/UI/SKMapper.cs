using System.Security.Permissions;
using Numbers.Core;
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
	    public abstract SKPoint StartPoint { get; }
        public abstract SKPoint MidPoint { get; }
        public abstract SKPoint EndPoint { get; }
        //   public SKPoint[] SalientPoints { get; private set; }
        //public SKSegment[] SalientSegments { get; private set; }
        //public SKPath[] SalientAreas { get; private set; }

        protected CoreRenderer Renderer { get; }
        protected SKCanvas Canvas => Renderer.Canvas;
        protected CorePens Pens => Renderer.Pens;

        public SKMapper(CoreRenderer renderer, IMathElement element)
        {
	        Id = _mapperCounter++;
            Renderer = renderer;
	        MathElement = element;
        }

        //public abstract void Draw();
    }
}
