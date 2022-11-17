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
     //   public RatioSeg UnitRatio;
	    //public SKSegment UnitSegment;
	    public RatioSeg UnitRatio => Domain.UnitFocalRatio;
	    private SKNumberMapper UnitMapper => WorkspaceMapper.NumberMapper(Domain.UnitId);
        public SKSegment UnitSegment => UnitMapper.NumberSegment;
	    public List<SKPoint> TickPoints = new List<SKPoint>();
	    public List<int> Markers = new List<int>();

	    public SKSegment DisplayLine { get; private set; }

	    public Complex DisplayRange()
	    {
		    var us = UnitSegment;
		    var usLen = UnitSegment.Length;
		    var dlLen = DisplayLine.Length;
            var (zeroT, zeroPt) = DisplayLine.TFromPoint(us.StartPoint, false);
            var start = zeroT * dlLen;
            var end = (1.0f - zeroT) * dlLen;

            return new Complex(end / usLen, start / usLen);
	    }
	    public RatioSeg DisplayRatio()
	    {
		    var dv = Domain.MaxRangeValue;
		    var dr = DisplayRange();
		    var len = Domain.MaxRangeLengthValue;
		    var sv = (dv.Imaginary - dr.Imaginary) / len;
		    var ev = (len - (dv.Real - dr.Real)) / len;
            return new RatioSeg((float) sv, (float)ev); 
	    }

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
	    public bool ShowFractions;

        public override SKPoint StartPoint
	    {
		    get => DisplayLine.StartPoint;
		    set => Reset(Domain, value, EndPoint);
	    }
	    public override SKPoint MidPoint => DisplayLine.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => DisplayLine.EndPoint;
		    set => Reset(Domain, StartPoint, value);
	    }
	    public SKPoint[] EndPoints => new SKPoint[] {StartPoint, EndPoint};

	    private static int domainIndexCounter = 0;
	    private int domainIndex = -1;

	    public SKDomainMapper(Workspace workspace, Domain domain, SKPoint startPoint, SKPoint endPoint, float unitStartT, float unitWidthT) : base(workspace, domain)
	    {
		    //DisplayRange = displayRange == default ? domain.FocalAsValue(domain.MaxRange) : displayRange;
		    base.MathElement = domain;
		    Domain = domain;
		    DisplayLine = new SKSegment(startPoint, endPoint);
		    var unit = Domain.Unit;
		    //var rs = RatioInDisplay(unit);
		    var unitMapper = WorkspaceMapper.NumberMapper(unit.Id);
		    unitMapper.NumberSegment = DisplayLine.SegmentAlongLine(unitStartT, unitStartT + unitWidthT);

		    Reset(domain, startPoint, endPoint);
	    }

	    public void Reset(Domain domain, SKPoint startPoint, SKPoint endPoint)
	    {
		    base.MathElement = domain;
		    Domain = domain;
		    DisplayLine = new SKSegment(startPoint, endPoint);
		    //UnitMapper = WorkspaceMapper.NumberMapper(Domain.UnitId);
            //UnitRatio = Domain.UnitFocalRatio;
            //UnitSegment = DisplayLine.SegmentAlongLine(UnitRatio.Start, UnitRatio.End); // todo: base on visible section of domain line
		    Markers.Clear();
		    AddUnitMarkers();
		    AddNumberMarkers();
	    }

	    private RatioSeg RatioInDisplay(Number num)
	    {
		    var displayRatio = DisplayRatio();
		    var displayLen = displayRatio.Start + displayRatio.End;
		    var numRatio = num.Ratio;
		    return new RatioSeg((numRatio.Start - displayRatio.Start) / displayLen, (numRatio.End - displayRatio.Start) / displayLen);
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
		    return Renderer.GetCirclePath(targetPoint);
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
		    //UnitMapper = WorkspaceMapper.NumberMapper(Domain.UnitId);

            if (Domain != null)
		    {
			    if (domainIndex == -1) domainIndex = domainIndexCounter++;
			    DrawNumberLineGradient();
			    DrawNumberLine();
			    DrawUnit();
			    DrawTicks();
			    DrawMarkers();
			    var offset = ShowNumberOffsets ? 1f : 0f;
			    var step = ShowNumberOffsets ? 1f : 0f;
                var segPens = new[] {Pens.SegPen1, Pens.SegPen2};

			    foreach (var numberId in Domain.NumberIds)
			    {
				    offset += step;
				    var num = WorkspaceMapper.NumberMapper(numberId);
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

	    private void DrawUnit()
	    {
		    if (ShowUnits)
		    {
			    WorkspaceMapper.NumberMapper(Domain.UnitId).DrawUnit();
            }
	    }

	    private void DrawNumberLineGradient()
	    {
		    if (ShowGradientNumberLine)
		    {
			    var pnt = CorePens.GetGradientPen(
				    DisplayLine.StartPoint, DisplayLine.EndPoint, Pens.UnotLineColor, Pens.UnitLineColor, 10);
			    Renderer.DrawSegment(DisplayLine, pnt);
		    }
		    else if(!ShowUnits) // if not showing units at least color the line
		    {
			    var pnt = CorePens.GetGradientPen(
				    DisplayLine.StartPoint, DisplayLine.EndPoint, Pens.UnotLineColor, Pens.UnitLineColor, 3);
			    Renderer.DrawSegment(DisplayLine, pnt);

            }
	    }

	    private void DrawMarkers()
	    {
		    foreach (var id in Markers)
		    {
			    var num = Workspace.NumberStore[id];
			    //var ratio = RatioInDisplay(num);//num.Ratio;//
			    //var isUnit = id == Domain.UnitId;
			    //var rem = num.Remainder;
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

		    var value = isStart ? num.StartValue : num.EndValue;
            var val = num.ValueInUnitPerspective;
            var t = (float)(isStart ? val.Imaginary : val.Real);
            //var t = isStart ? dr.Start : dr.End;
		    var suffix = isStart ? "i" : "";
		    var unitLabel = num.IsUnitOrUnot && isStart ? "0" : num.IsUnit ? "1" : num.IsUnot ? "i" : "";

		    var textPoint = DrawMarkerPointer(t);

		    var txt = "";
		    var txtPaint = isStart ? Pens.UnotMarkerText : Pens.UnitMarkerText;
		    if (ShowFractions)
		    {
			    var whole = isStart ? num.WholePartStart.ToString() : num.WholePartEnd.ToString();
			    string fraction = "";
			    if (num.DenominatorPart != 0)
			    {
				    var numerator = isStart ? num.NumeratorPartStart : num.NumeratorPartEnd;
				    if (numerator != 0)
				    {
					    fraction = " " + numerator.ToString() + "/" + num.DenominatorPart.ToString();
                    }
			    }
			    txt = whole + fraction + suffix;
		    }
		    else
		    {
				txt = unitLabel != "" ? unitLabel : Math.Abs(value - (int) value) < 0.1f ? $"{value:0}{suffix}" : $"{value:0.0}{suffix}";
		    }

		    if (isStart)
            {
				Renderer.DrawText(textPoint, txt, txtPaint, Pens.TextBackgroundPen);
		    }
		    else
		    {
                Renderer.DrawText(textPoint, txt, txtPaint, Pens.TextBackgroundPen);
            }
	    }

	    private SKPoint DrawMarkerPointer(float t)
	    {
		    var w = 5.0f;
		    var unitSeg = UnitSegment;
		    var markerHW = (float) (1.0 / UnitSegment.Length) * w;
		    var pt = unitSeg.PointAlongLine(t);
		    var ptPlus = unitSeg.PointAlongLine(t + markerHW);
		    var ptMinus = unitSeg.PointAlongLine(t - markerHW);
		    var p0 = unitSeg.OrthogonalPoint(pt, -w * 2);
		    var p1 = unitSeg.OrthogonalPoint(ptPlus, -w * 4);
		    var p2 = unitSeg.OrthogonalPoint(ptMinus, -w * 4);
		    Renderer.FillPolyline(Pens.MarkerBrush, p0, p1, p2, p0);

		    var textPoint = unitSeg.OrthogonalPoint(pt, -w * 5);
		    
		    return textPoint;
	    }

	    private void DrawNumberLine()
	    {
		    Renderer.DrawSegment(DisplayLine, Renderer.Pens.NumberLinePen);
	    }

	    public long[] WholeNumberTicks()
	    {
		    var result = new List<long>();
		    var dr = DisplayRange();
		    var start = (int)Math.Ceiling(-dr.Imaginary);
		    var end = (int)Math.Floor(dr.Real);
		    for (var i = start; i <= end; i++)
		    {
			    result.Add(i);
		    }
		    return result.ToArray();
	    }
        private void DrawTicks()
	    {
            var dr = DisplayRange();
            var wholeTicks = WholeNumberTicks();

		    var tickCount = (float)Domain.Unit.TickCount;
		    var tickLen = UnitSegment.Length / tickCount;
		    var showMinorTicks = tickLen >= 3;

            TickPoints.Clear();
            foreach (var wholeTick in wholeTicks)
            {
                TickPoints.Add(DrawTick(wholeTick, -8, Renderer.Pens.TickBoldPen));
		    }

            if (showMinorTicks)
            {
	            for (var i = wholeTicks[0] - 1; i < wholeTicks[wholeTicks.Length - 1] + 1; i++)
	            {
		            for (int j = 0; j < tickCount; j++)
		            {
			            var t = i + j / tickCount;
			            if (t > -dr.Imaginary && t < dr.Real)
			            {
				            DrawTick(t, -8, Renderer.Pens.TickPen);
			            }
		            }
	            }
            }
        }

	    private SKPoint DrawTick(float t, int offset, SKPaint paint)
	    {
		    var pts = UnitSegment.PerpendicularLine(t, offset);
		    Renderer.DrawLine(pts.Item1, pts.Item2, paint);
		    return pts.Item1;
	    }
    }
}
