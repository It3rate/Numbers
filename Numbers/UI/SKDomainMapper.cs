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

	    public bool ShowGradientNumberLine;
	    public bool ShowTicks;
	    public bool ShowNumberOffsets;
	    public bool ShowKeyValues;
	    public bool ShowValueMarkers = true;
	    public bool ShowUnits;
	    public bool ShowUnitMarkers;
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
	    public SKPoint[] EndPoints => new SKPoint[] {StartPoint, EndPoint};

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

	    public void SetValueByKind(SKPoint newPoint, UIKind kind)
	    {
		    if (kind.IsMajor())
		    {
			    EndPoint = newPoint;
		    }
		    else
		    {
			    StartPoint = newPoint;
		    }
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
			    var offset = ShowNumberOffsets ? 1f : 0f;
			    var step = ShowNumberOffsets ? 1f : 0f;
                var segPens = new[] {Pens.SegPen1, Pens.SegPen2};

			    foreach (var numberId in Domain.NumberIds)
			    {
				    offset += step;
				    var num = Workspace.NumberMapper(numberId);
				    var pen = Pens.SegPens[domainIndex % Pens.SegPens.Count];
				    num.DrawNumber(offset, pen);
				    if (Domain.IsUnitPerspective)
				    {
					    var offsetScale = pen.StrokeWidth / Pens.UnitInlinePen.StrokeWidth;
					    num.DrawNumber(offset * offsetScale, Pens.UnitInlinePen);
				    }
				    else
				    {
					    var offsetScale = pen.StrokeWidth / Pens.UnotInlinePen.StrokeWidth;
					    num.DrawNumber(offset * offsetScale, Pens.UnotInlinePen);
				    }
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
			    DrawMarker(num, true);
			    DrawMarker(num, false);
		    }
	    }

	    private void DrawMarker(Number num, bool isStart)
	    {
		    if ((!ShowValueMarkers && !num.IsUnitOrUnot) || (!ShowUnitMarkers && num.IsUnitOrUnot))
		    {
			    return;
		    }

		    var value = isStart ? -num.StartValue : num.EndValue;
		    var t = isStart ? num.Ratio.Start : num.Ratio.End;
		    var suffix = isStart ? "i" : "";
		    var unitLabel = num.IsUnitOrUnot && isStart ? "0" : num.IsUnit ? "1" : num.IsUnot ? "i" : "";

		    var textPoint = DrawMarkerPointer(t);
		    var txt = unitLabel != "" ? unitLabel : Math.Abs(value - (int) value) < 0.1f ? $"{value:0}{suffix}" : $"{value:0.0}{suffix}";
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

		    var textPoint = DomainSegment.OrthogonalPoint(pt, -w * 5);
		    ;
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
		    if (ShowUnits)
		    {
			    Workspace.NumberMapper(Domain.UnitId).DrawUnit();
            }
	    }

	    private void DrawNumberLine()
	    {
		    Renderer.DrawSegment(DomainSegment, Renderer.Pens.NumberLinePen);
	    }

	    private void DrawTicks()
	    {
		    if (DomainSegment.Length / Domain.MaxRange.LengthInTicks >= 4)
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
		    var segStart = (float)Domain.MaxRange.StartTickValue;
		    var zeroTick = (float)Domain.Unit.StartTickPosition;
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
