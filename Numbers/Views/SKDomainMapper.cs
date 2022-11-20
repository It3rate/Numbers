using System;
using System.Collections.Generic;
using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public class SKDomainMapper : SKMapper
	{
	    public Domain Domain { get; private set; }

	    private SKNumberMapper UnitMapper => WorkspaceMapper.NumberMapper(Domain.BasisNumberId);
	    public Number UnitNumber => Domain.BasisNumber;
	    public SKSegment UnitSegment => UnitMapper.NumberSegment;
	    public int UnitSign => UnitNumber.Direction;
	    public Range UnitRangeOnDomainLine => DisplayLine.RatiosAsBasis(UnitSegment);
	    public int UnitDirectionOnDomainLine => DisplayLine.CosineSimilarity(UnitSegment) >= 0 ? 1 : -1;

        public List<SKPoint> TickPoints = new List<SKPoint>();
	    public List<int> ValidNumberIds = new List<int>();
	    public SKSegment DisplayLine { get; private set; }
	    public Range DisplayLineRange => UnitSegment.RatiosAsBasis(DisplayLine);

	    public bool ShowGradientNumberLine;
	    public bool ShowTicks;
	    public bool ShowNumberOffsets;
	    public bool ShowKeyValues;
	    public bool ShowValueMarkers = true;
	    public bool ShowUnits;
	    public bool ShowBasisMarkers;
	    public bool ShowUnotArrow;
	    public bool ShowMaxMinValues;
	    public bool ShowDashedValuesOutOfRange;
	    public bool ShowFractions;

        public override SKPoint StartPoint
	    {
		    get => DisplayLine.StartPoint;
		    set => Reset(value, EndPoint);
	    }
	    public override SKPoint MidPoint => DisplayLine.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => DisplayLine.EndPoint;
		    set => Reset(StartPoint, value);
	    }
	    public SKPoint[] EndPoints => new SKPoint[] {StartPoint, EndPoint};

        public SKDomainMapper(Workspace workspace, Domain domain, SKSegment displayLine, SKSegment unitSegment) : base(workspace, domain)
	    {
		    base.MathElement = domain;
		    Domain = domain;
		    DisplayLine = displayLine;
		    var unit = Domain.BasisNumber;
		    var unitMapper = WorkspaceMapper.NumberMapper(unit.Id);
		    unitMapper.NumberSegment = new SKSegment(displayLine.ProjectPointOnto(unitSegment.StartPoint), displayLine.ProjectPointOnto(unitSegment.EndPoint));
		    //unitMapper.NumberSegment = DisplayLine.SegmentAlongLine(unitStartT, unitStartT + unitWidthT);
		    Reset(displayLine.StartPoint, displayLine.EndPoint);
	    }

	    public void Reset(SKPoint startPoint, SKPoint endPoint)
	    {
		    DisplayLine = new SKSegment(startPoint, endPoint);
            AddValidNumbers();
	    }
	    private void AddValidNumbers()
	    {
		    ValidNumberIds.Clear();
            if (ShowBasisMarkers)
		    {
			    ValidNumberIds.Add(Domain.BasisNumberId);
            }

		    if (ShowValueMarkers)
		    {
			    foreach (var id in Domain.NumberIds)
			    {
				    if (Workspace.IsElementActive(id) && id != Domain.BasisNumberId && id != Domain.MinMaxNumberId)
				    {
					    ValidNumberIds.Add(id);
				    }
			    }
            }
	    }
        public void SetValueByKind(SKPoint newPoint, UIKind kind)
        {
	        var unitRatio = UnitRangeOnDomainLine;
            if (kind.IsMajor())
		    {
			    EndPoint = newPoint;
		    }
		    else
		    {
			    StartPoint = newPoint;
		    }
            UnitMapper.NumberSegment = DisplayLine.SegmentAlongLine(unitRatio);
        }

        public void Draw()
	    {
            if (Domain != null)
		    {
			    DrawNumberLine();
			    DrawUnit();
			    DrawTicks();
			    DrawMarkers();
                DrawNumbers();
		    }
	    }
        private void DrawUnit()
	    {
		    if (ShowUnits)
		    {
			    WorkspaceMapper.NumberMapper(Domain.BasisNumberId).DrawUnit();
            }
	    }
	    private void DrawMarkers()
	    {
            foreach (var id in ValidNumberIds)
		    {
			    var num = MyBrain.NumberStore[id];
			    DrawMarker(num, true);
			    DrawMarker(num, false);
		    }
	    }
	    private void DrawMarker(Number num, bool isStart)
	    {
		    if ((!ShowValueMarkers && !num.IsBasis) || (!ShowBasisMarkers && num.IsBasis))
		    {
			    return;
		    }

		    var value = isStart ? num.StartValue : num.EndValue;
		    var t = (float)(isStart ? -value : value);
		    var suffix = isStart ? "i" : "";
		    var unitLabel = num.IsBasis && isStart ? "0" : num.IsUnit ? "1" : num.IsUnot ? "i" : "";

		    var textPoint = DrawMarkerPointer(t);

		    var txt = "";
		    var txtPaint = isStart ? Pens.UnotMarkerText : Pens.UnitMarkerText;
		    if (ShowFractions)
		    {
			    var whole = isStart ? num.RoundedStartValue.ToString() : num.RoundedEndValue.ToString();
			    string fraction = "";
			    if (num.AbsBasisTicks != 0)
			    {
				    var numerator = isStart ? num.RemainderStartValue : num.RemainderEndValue;
				    if (numerator != 0)
				    {
					    fraction = " " + numerator.ToString() + "/" + num.AbsBasisTicks.ToString();
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
		    var sign = UnitDirectionOnDomainLine;
            var w = 5.0f * sign;
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
		    var sign = UnitDirectionOnDomainLine;
		    var gsp = sign == 1 ? DisplayLine.StartPoint : DisplayLine.EndPoint;
		    var gep = sign == 1 ? DisplayLine.EndPoint : DisplayLine.StartPoint;
            if (ShowGradientNumberLine)
		    {
			    var pnt = CorePens.GetGradientPen(gsp, gep, Pens.UnotLineColor, Pens.UnitLineColor, 10);
			    Renderer.DrawSegment(DisplayLine, pnt);
		    }
		    else if (!ShowUnits) // if not showing units at least color the line
		    {
			    var pnt = CorePens.GetGradientPen( gsp, gep, Pens.UnotLineColor, Pens.UnitLineColor, 3);
			    Renderer.DrawSegment(DisplayLine, pnt);
		    }

            Renderer.DrawSegment(DisplayLine, Renderer.Pens.NumberLinePen);
	    }
        private void DrawTicks()
	    {
            var dr = DisplayLineRange;
            var sign = UnitDirectionOnDomainLine;
            TickPoints.Clear();
            for(int i = (int)dr.Start; i != (int)dr.End + sign; i += sign)
            {
	            TickPoints.Add(DrawTick(i, -8 * sign, Renderer.Pens.TickBoldPen));
            }

		    var tickCount = (float)Domain.BasisNumber.AbsBasisTicks;
		    var tickLen = UnitSegment.Length / tickCount;
		    var showMinorTicks = Math.Abs(tickLen) >= 3;
            if (showMinorTicks)
            {
				var minorDr = dr * tickCount;
	            for (int i = (int)minorDr.Min; i <= (int)minorDr.Max; i++)
	            {
		            DrawTick((i / tickCount), -8 * sign, Renderer.Pens.TickPen);
	            }
            }
        }
        private SKPoint DrawTick(float t, int offset, SKPaint paint)
        {
	        var pts = UnitSegment.PerpendicularLine(t, offset);
	        Renderer.DrawLine(pts.Item1, pts.Item2, paint);
	        return pts.Item1;
        }

        private void DrawNumbers()
        {
	        var offset = ShowNumberOffsets ? 1f : 0f;
	        var step = ShowNumberOffsets ? 1f : 0f;
	        foreach (var numberId in ValidNumberIds)
	        {
		        offset += step;
		        var num = WorkspaceMapper.NumberMapper(numberId);
		        var pen = Pens.SegPens[Domain.CreationIndex % Pens.SegPens.Count];
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
        public override SKPath GetHighlightAt(float t, SKPoint targetPoint)
	    {
		    return Renderer.GetCirclePath(targetPoint);
	    }
	}
}
