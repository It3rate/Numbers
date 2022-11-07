﻿using Numbers.Core;
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

	    public override SKPoint StartPoint => DomainSegment.StartPoint;
	    public override SKPoint MidPoint => DomainSegment.Midpoint;
	    public override SKPoint EndPoint => DomainSegment.EndPoint;

        public SKDomainMapper(CoreRenderer renderer, Domain domain, SKPoint startPoint, SKPoint endPoint) : base(renderer, domain)
        {
	        Reset(domain, startPoint, endPoint);
        }
        public void Reset(Domain domain, SKPoint startPoint, SKPoint endPoint)
        {
	        base.MathElement = domain;
	        Domain = domain;
	        DomainSegment = new SKSegment(startPoint, endPoint);
	        UnitRatio = Domain.UnitRatio;
	        UnitSegment = DomainSegment.SegmentAlongLine(UnitRatio.Start, UnitRatio.End);
        }

        private static int domainIndexCounter = 0;
        private int domainIndex = -1;
        public void Draw()
        {
	        if (Domain != null)
	        {
		        if (domainIndex == -1) domainIndex = domainIndexCounter++;
		        DrawUnit();
		        DrawNumberline();
		        var offset = SKPoint.Empty;
		        var step = DomainSegment.RelativeOffset(0);//10);
		        var segPens = new[] { Pens.SegPen1, Pens.SegPen2 };
		        foreach (var numberId in Domain.NumberIds)
		        {
			        offset += step;
			        var pen = Pens.SegPens[domainIndex % Pens.SegPens.Count];
                    Renderer.NumberMapper(numberId).Draw(offset, pen);
		        }
	        }
        }

        private void DrawUnit()
        {
	        Renderer.DrawSegment(UnitSegment, Pens.HighlightPen);
        }

        private void DrawNumberline()
	    {
		    Renderer.DrawSegment(DomainSegment, Renderer.Pens.GrayPen);
		    DrawTick(0, -8, Renderer.Pens.TickBoldPen);
		    DrawTick(1, -8, Renderer.Pens.TickBoldPen);

		    var segStart = (float)Domain.MaxRange.StartTickValue;
		    var segLen = (float)Domain.MaxRange.LengthInTicks;
		    var wholeTicks = Domain.WholeNumberTicks();
		    foreach (var wholeTick in wholeTicks)
		    {
			    var t = (wholeTick - segStart) / segLen;
			    DrawTick(t, -8, Renderer.Pens.TickPen);
		    }
	    }
	    private void DrawTick(float t, int offset, SKPaint paint)
	    {
		    var pts = DomainSegment.PerpendicularLine(t, offset);
		    Renderer.DrawLine(pts.Item1, pts.Item2, paint);
	    }
    }
}
