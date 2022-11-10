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

    public class SKDomainMapper : SKMapper
    {
	    public Domain Domain { get; private set; }
	    public SKSegment DomainSegment { get; private set; }
	    public RatioSeg UnitRatio;
	    public SKSegment UnitSegment;
	    public List<SKPoint> TickPoints = new List<SKPoint>();

        public override SKPoint StartPoint => DomainSegment.StartPoint;
	    public override SKPoint MidPoint => DomainSegment.Midpoint;
	    public override SKPoint EndPoint => DomainSegment.EndPoint;

        private static int domainIndexCounter = 0;
        private int domainIndex = -1;

        public SKDomainMapper(Workspace workspace, Domain domain, SKPoint startPoint, SKPoint endPoint) : base(workspace, domain)
        {
	        Reset(domain, startPoint, endPoint);
        }
        public void Reset(Domain domain, SKPoint startPoint, SKPoint endPoint)
        {
	        base.MathElement = domain;
	        Domain = domain;
	        DomainSegment = new SKSegment(startPoint, endPoint);
	        UnitRatio = Domain.UnitFocalRatio;
	        UnitSegment = DomainSegment.SegmentAlongLine(UnitRatio.Start, UnitRatio.End);
        }

        public override SKPath HighlightAt(float t, SKPoint targetPoint)
        {
	        var pt = DomainSegment.PointAlongLine(t);
	        return Renderer.GetCirclePath(pt);
        }

        public void Draw()
        {
	        if (Domain != null)
	        {
		        if (domainIndex == -1) domainIndex = domainIndexCounter++;
		        //DrawUnit();
		        DrawNumberline();
		        var offset = SKPoint.Empty;
		        var step = DomainSegment.RelativeOffset(0);//10);
		        var segPens = new[] { Pens.SegPen1, Pens.SegPen2 };
		        foreach (var numberId in Domain.NumberIds)
		        {
			        offset += step;
			        var pen = Pens.SegPens[domainIndex % Pens.SegPens.Count];
			        Workspace.NumberMapper(numberId).Draw(offset, pen);
		        }
	        }
        }

        private void DrawNumberline()
	    {
		    Renderer.DrawSegment(DomainSegment, Renderer.Pens.GrayPen);
		    //DrawTick(0, -8, Renderer.Pens.TickBoldPen);
		    //DrawTick(1, -8, Renderer.Pens.TickBoldPen);

		    var segStart = (float)Domain.MaxRange.StartTickValue;
		    var segLen = (float)Domain.MaxRange.LengthInTicks;
		    var wholeTicks = Domain.WholeNumberTicks();
		    TickPoints.Clear();
		    foreach (var wholeTick in wholeTicks)
		    {
			    var t = (wholeTick - segStart) / segLen;
			    TickPoints.Add(DrawTick(t, -8, Renderer.Pens.TickPen));
		    }
	    }
	    private SKPoint DrawTick(float t, int offset, SKPaint paint)
	    {
		    var pts = DomainSegment.PerpendicularLine(t, offset);
		    Renderer.DrawLine(pts.Item1, pts.Item2, paint);
		    return pts.Item1;
	    }
    }
}
