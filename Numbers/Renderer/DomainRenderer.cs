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
	    public SKSegment DomainLine { get; private set; }

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
		    DomainLine = new SKSegment(startPoint, endPoint);
	    }

        public void Draw()
        {
	        if (_domain != null)
	        {
		        _renderer.DrawSegment(DomainLine, _pens.GrayPen);

		        var unitRatio = _domain.UnitRatio;
		        var unitSeg = DomainLine.SegmentAlongLine(unitRatio.Start, unitRatio.End);

		        _renderer.DrawTick(unitSeg.StartPoint, _pens.TickBoldPen);
		        _renderer.DrawTick(unitSeg.EndPoint, _pens.TickPen);
		        _renderer.DrawSegment(unitSeg, _pens.HighlightPen);

		        var offset = new SKPoint(0, 0);
		        foreach (var number in _domain.Numbers.Values)
		        {
			        offset.Y += 10;
			        var nr = number.Ratio;
			        var numSeg = DomainLine.SegmentAlongLine(nr.Start, nr.End);
			        numSeg += offset;
			        _renderer.DrawSegment(numSeg, _pens.SegPen);
		        }
	        }
        }


    }
}
