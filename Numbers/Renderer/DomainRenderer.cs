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

	    private SKSegment _domainSeg;
	    private RatioSeg _unitRatio;
	    private SKSegment _unitSeg;

	    public DomainRenderer(CoreRenderer renderer)
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
		    _domainSeg = new SKSegment(startPoint, endPoint);
	        _unitRatio = _domain.UnitRatio;
	        _unitSeg = _domainSeg.SegmentAlongLine(_unitRatio.Start, _unitRatio.End);
	    }

        public void Draw()
        {
	        if (_domain != null)
	        {
		        DrawNumberline();
		        var offset = new SKPoint(0, 0);
		        foreach (var number in _domain.Numbers.Values)
		        {
			        offset.Y += 10;
			        var nr = number.Ratio;
			        var numSeg = _domainSeg.SegmentAlongLine(nr.Start, nr.End);
			        numSeg += offset;
			        _renderer.DrawSegment(numSeg, _pens.SegPen);
		        }
	        }
        }

        private void DrawNumberline()
        {
	        _renderer.DrawSegment(_domainSeg, _pens.GrayPen);
	        _renderer.DrawTick(_unitSeg.StartPoint, _pens.TickBoldPen);
	        _renderer.DrawTick(_unitSeg.EndPoint, _pens.TickPen);
	        _renderer.DrawSegment(_unitSeg, _pens.HighlightPen);

	        var segStart = (float)_domain.MaxRange.StartTickValue;
	        var segLen = (float)_domain.MaxRange.LengthInTicks;
            var wholeTicks = _domain.WholeNumberTicks();
	        foreach (var wholeTick in wholeTicks)
	        {
		        var tickPt = _domainSeg.PointAlongLine((wholeTick - segStart) / segLen);
		        _renderer.DrawTick(tickPt, _pens.TickPen);
	        }
        }

    }
}
