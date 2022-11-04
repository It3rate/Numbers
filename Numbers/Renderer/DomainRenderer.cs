using System.Drawing;
using Numbers.Core;
using SkiaSharp;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DomainRenderer
    {
	    private CoreRenderer _renderer;
        private CorePens _pens;
	    public Domain _domain { get; private set; }
	    public SKPoint StartPoint { get; private set; }
	    public SKPoint EndPoint { get; private set; }

	    public SKSegment DomainSeg;
	    public RatioSeg UnitRatio;
	    public SKSegment UnitSeg;

	    private DomainRenderer(CoreRenderer renderer)
	    {
		    _renderer = renderer;
		    _pens = _renderer.Pens;
	    }
	    public DomainRenderer(CoreRenderer renderer, Domain domain, SKPoint startPoint, SKPoint endPoint) : this(renderer)
	    {
		    Reset(domain, startPoint, endPoint);
	    }

        public void Reset(Domain domain, SKPoint startPoint, SKPoint endPoint)
	    {
		    _domain = domain;
		    StartPoint = startPoint;
		    EndPoint = endPoint;
		    DomainSeg = new SKSegment(startPoint, endPoint);
	        UnitRatio = _domain.UnitRatio;
	        UnitSeg = DomainSeg.SegmentAlongLine(UnitRatio.Start, UnitRatio.End);
	    }

        public void Draw()
        {
	        if (_domain != null)
	        {
		        DrawNumberline();
		        var offset = SKPoint.Empty;
		        var step = DomainSeg.RelativeOffset(10);
		        foreach (var numberId in _domain.Numbers)
		        {
			        var number = Number.NumberStore[numberId];
			        offset += step;
			        var nr = number.Ratio;
			        var numSeg = DomainSeg.SegmentAlongLine(nr.Start, nr.End);
			        numSeg += offset;
			        _renderer.DrawSegment(numSeg, _pens.SegPen);
		        }
	        }
        }

        private void DrawTick(float t, int offset, SKPaint paint)
        {
	        var pts = DomainSeg.PerpendicularLine(t, offset);
            _renderer.DrawLine(pts.Item1, pts.Item2, paint);
        }
        private void DrawNumberline()
        {
	        _renderer.DrawSegment(DomainSeg, _pens.GrayPen);
	        DrawTick(0, -8, _pens.TickBoldPen);
	        DrawTick(1, -8, _pens.TickBoldPen);
	        _renderer.DrawSegment(UnitSeg, _pens.HighlightPen);

	        var segStart = (float)_domain.MaxRange.StartTickValue;
	        var segLen = (float)_domain.MaxRange.LengthInTicks;
            var wholeTicks = _domain.WholeNumberTicks();
	        foreach (var wholeTick in wholeTicks)
	        {
		        var t = (wholeTick - segStart) / segLen;
		        DrawTick(t, -8, _pens.TickPen);
	        }
        }

    }
}
