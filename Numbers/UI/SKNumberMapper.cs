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

    public class SKNumberMapper : SKMapper
    {
        public Number Number { get; }
	    public SKSegment NumberSegment { get; private set; }

	    private SKDomainMapper DomainMapper => Workspace.DomainMapper(Number.Domain.Id);

	    public override SKPoint StartPoint => NumberSegment.StartPoint;
	    public override SKPoint MidPoint => NumberSegment.Midpoint;
	    public override SKPoint EndPoint => NumberSegment.EndPoint;

        public SKSegment DomainSegment => DomainMapper.DomainSegment;

	    public SKNumberMapper(Workspace workspace, Number number) : base(workspace, number)
	    {
		    Number = number;
	    }

        public void Draw(SKPoint offset, SKPaint paint)
	    {
		    var nr = Number.Ratio;
		    NumberSegment = DomainSegment.SegmentAlongLine(nr.Start, nr.End);
		    NumberSegment += offset;
		    Renderer.DrawDirectedLine(NumberSegment, paint);
	    }

        public void SetStartValueByPoint(SKPoint newPoint)
        {
	        var dm = DomainMapper.DomainSegment;
	        var pt = dm.ProjectPointOnto(newPoint);
	        var (t, _) = dm.TFromPoint(pt, false);
	        Number.StartT = 1.0 - t;
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
	        var dm = DomainMapper.DomainSegment;
	        var pt = dm.ProjectPointOnto(newPoint);
	        var (t, _) = dm.TFromPoint(pt, false);
	        Number.EndT = t;
        }

        public override SKPath HighlightAt(float t, SKPoint targetPoint)
        {
	        var pt = NumberSegment.PointAlongLine(t);
	        return Renderer.GetCirclePath(pt);
        }
    }
}
