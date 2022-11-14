using System.Drawing;
using System.Numerics;
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
        public List<int> Markers = new List<int>();

	    public bool ShowGradientNumberLine ;
	    public bool ShowTicks;
	    public bool ShowNumbersAsOffsets;
	    public bool ShowKeyValues;
	    public bool ShowValueMarkers;
	    public bool ShowUnit;
	    public bool ShowUnotArrow;
	    public bool ShowMaxMinValues;
	    public bool ShowDashedValuesOutOfRange;

        public override SKPoint StartPoint
	    {
		    get => DomainSegment.StartPoint;
		    set => Reset(Domain, value, EndPoint);
        }
	    public override SKPoint MidPoint => DomainSegment.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => DomainSegment.EndPoint;
		    set => Reset(Domain, StartPoint, value);
        }

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
	        Markers.Clear();
	        AddUnitMarkers();
	        AddNumberMarkers();
        }

        private void AddUnitMarkers()
        {
	        Markers.Add(Domain.UnitId);
        }
        private void AddNumberMarkers()
        {
	        Markers.AddRange(Domain.NumberIds);
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
		        DrawNumberLineGradient();
                DrawNumberLine();
		        DrawUnit();
		        DrawTicks();
		        DrawIntTicks();
		        DrawMarkers();
                var offset = 1f;
                var step = 1f;
		        var segPens = new[] { Pens.SegPen1, Pens.SegPen2 };

                foreach (var numberId in Domain.NumberIds)
		        {
			        offset += step;
			        var pen = Pens.SegPens[domainIndex % Pens.SegPens.Count];
			        Workspace.NumberMapper(numberId).DrawIfNotUnit(offset, pen);
		        }
	        }
        }

        private void DrawMarkers()
        {
	        foreach (var id in Markers)
	        {
		        var num = Number.NumberStore[id];
		        var ratio = num.Ratio;
		        var isUnit = id == Domain.UnitId;
		        var rem = num.Remainder;
		        DrawMarker(num.StartValue, ratio.Start);
		        DrawMarker(num.EndValue, ratio.End);
            }
        }
        private void DrawMarker(double value, float t)
        {
	        var textPoint = DrawMarkerPointer(t);
	        var txt = Math.Abs(value - (int) value) < 0.1f ? $"{value:0}" : $"{value:0.0}";
	        Renderer.DrawText(textPoint, txt, Pens.LineTextPen);
        }

        private SKPoint DrawMarkerPointer(float t)
        {
	        var w = 5.0f;
	        var markerHW = (float) (1.0 / DomainSegment.Length) * w;

	        var pt = DomainSegment.PointAlongLine(t);
	        var ptPlus = DomainSegment.PointAlongLine(t + markerHW);
            var ptMinus = DomainSegment.PointAlongLine(t - markerHW);
            var p0 = DomainSegment.OrthogonalPoint(pt, -w * 2);
            var p1 = DomainSegment.OrthogonalPoint(ptPlus, -w * 4);
            var p2 = DomainSegment.OrthogonalPoint(ptMinus, -w * 4);
            Renderer.FillPolyline(Pens.MarkerBrush, p0, p1, p2, p0);
            
            var textPoint = DomainSegment.OrthogonalPoint(pt, -w * 5); ;
            return textPoint;
        }

        private void DrawNumberLineGradient()
        {
	        if (ShowGradientNumberLine)
	        {
		        var pnt = CorePens.GetGradientPen(
			        DomainSegment.StartPoint, DomainSegment.EndPoint, Pens.UnotLineColor, Pens.UnitLineColor, 10);
		        Renderer.DrawSegment(DomainSegment, pnt);
	        }
        }

        private void DrawUnit()
        {
	        Workspace.NumberMapper(Domain.UnitId).DrawUnit();
        }
        private void DrawNumberLine()
        {
	        Renderer.DrawSegment(DomainSegment, Renderer.Pens.NumberLinePen);
        }

        private void DrawTicks()
        {
	        if (Domain.Unit.TickCount <= 10)
	        {
		        var segStart = Domain.MaxRange.StartTickValue;
		        var segEnd = Domain.MaxRange.EndTickValue;
                var segLen = (float) Domain.MaxRange.LengthInTicks;
		        for (var i = segStart; i < segEnd; i++)
		        {
			        var t = (i - segStart) / segLen;
			        DrawTick(t, -8, Renderer.Pens.TickPen);
		        }
	        }
        }
        private void DrawIntTicks()
        {
	        var segStart = (float) Domain.MaxRange.StartTickValue;
	        var segLen = (float) Domain.MaxRange.LengthInTicks;
	        var wholeTicks = Domain.WholeNumberTicks();
	        TickPoints.Clear();
	        foreach (var wholeTick in wholeTicks)
	        {
		        var t = (wholeTick - segStart) / segLen;
		        TickPoints.Add(DrawTick(t, -8, Renderer.Pens.TickBoldPen));
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
