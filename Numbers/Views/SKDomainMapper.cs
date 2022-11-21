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

	    private SKNumberMapper BasisMapper => WorkspaceMapper.NumberMapper(Domain.BasisNumberId);
	    public Number BasisNumber => Domain.BasisNumber;
	    public SKSegment BasisSegment => BasisMapper.NumberSegment;
	    public int BasisNumberSign => BasisNumber.Direction;

        public List<SKPoint> TickPoints = new List<SKPoint>();
	    public List<int> ValidNumberIds = new List<int>();
	    public SKSegment DisplayLine { get; private set; }
	    public Range DisplayLineRange => BasisSegment.RatiosAsBasis(DisplayLine);
	    public Range UnitRangeOnDomainLine
	    {
		    get => DisplayLine.RatiosAsBasis(BasisSegment);
		    set { BasisMapper.NumberSegment = DisplayLine.SegmentAlongLine(value); }
	    }
	    public int UnitDirectionOnDomainLine => DisplayLine.CosineSimilarity(BasisSegment) >= 0 ? 1 : -1;

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
	    public bool ShowFractions => WorkspaceMapper.ShowFractions;

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
		    foreach (var id in Domain.NumberIds)
		    {
			    if (Workspace.IsElementActive(id) && id != Domain.BasisNumberId && id != Domain.MinMaxNumberId)
			    {
				    ValidNumberIds.Add(id);
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
            BasisMapper.NumberSegment = DisplayLine.SegmentAlongLine(unitRatio);
        }

        public void Draw()
	    {
            if (Domain != null)
            {
	            AddValidNumbers();

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
	        if (ShowBasisMarkers)
	        {
		        var num = Domain.BasisNumber;
		        DrawMarker(num, true);
		        DrawMarker(num, false);
	        }

	        if (ShowValueMarkers)
	        {
		        foreach (var id in ValidNumberIds)
		        {
			        var num = MyBrain.NumberStore[id];
			        DrawMarker(num, true);
			        DrawMarker(num, false);
		        }
            }
        }

        private void DrawMarker(Number num, bool isStart)
        {
            var value = isStart ? num.Value.StartF : num.Value.EndF;
            var t = isStart ? num.ValueInFullUnitPerspective.StartF : num.ValueInFullUnitPerspective.EndF;
            var suffix = isStart ? "i" : "";
		    var unitLabel = num.IsBasis && isStart ? "0" : num.IsUnit ? "1" : num.IsUnot ? "i" : "";

		    var txtPoint = DrawMarkerPointer(t);

		    var txtPaint = isStart ? Pens.UnotMarkerText : Pens.UnitMarkerText;
		    var txtBkgPen = Pens.TextBackgroundPen;
            if (ShowFractions)
		    {
			    var parts = GetFractionText(num, isStart, suffix);
                DrawFraction(parts, txtPoint, txtPaint, txtBkgPen);
            }
		    else
		    {
				var txt = unitLabel != "" ? unitLabel : Math.Abs(value - (int) value) < 0.1f ? $"{value:0}{suffix}" : $"{value:0.0}{suffix}";
				Renderer.DrawText(txtPoint, txt, txtPaint, txtBkgPen);
            }
        }

        private void DrawFraction((string, string)parts, SKPoint txtPoint, SKPaint txtPaint, SKPaint txtBkgPen)
        {
	        var whole = parts.Item1;
	        var fraction = parts.Item2;
	        var fracPen = Pens.TextFractionPen;
	        if (fraction != "")
	        {
		        fracPen.Color = txtPaint.Color;
		        if (whole == "")
		        {
					fracPen.TextAlign = SKTextAlign.Center;
			        Renderer.DrawText(txtPoint, fraction, fracPen, txtBkgPen);
			        fracPen.TextAlign = SKTextAlign.Left;
                }
		        else
		        {
			        var txtAlign = txtPaint.TextAlign;
			        txtPaint.TextAlign = SKTextAlign.Right;
			        var wRect = Renderer.GetTextBackgroundSize(0, 0, whole, txtPaint);
			        var fRect = Renderer.GetTextBackgroundSize(0, 0, fraction, Pens.TextFractionPen);
			        Renderer.DrawText(txtPoint, whole, txtPaint, null);
			        var fPoint = new SKPoint(txtPoint.X - 2, txtPoint.Y);
			        Renderer.DrawText(fPoint, fraction, fracPen, null);
			        wRect.Union(fRect);
			        Renderer.DrawTextBackground(wRect, txtBkgPen);
			        txtPaint.TextAlign = txtAlign;
		        }
	        }
	        else
	        {
		        Renderer.DrawText(txtPoint, whole, txtPaint, txtBkgPen);
	        }
        }

        private (string, string) GetFractionText(Number num, bool isStart, string suffix)
        {
	        var sign = isStart ? (num.StartValue >= 0 ? "" : "-") : (num.EndValue >= 0 ? "" : "-");
	        var wholeNum = isStart ? num.WholeStartValue : num.WholeEndValue;
	        var whole = wholeNum == 0 ? "" : wholeNum.ToString();
	        string fraction = "";
		    var numerator = isStart ? num.RemainderStartValue : num.RemainderEndValue;
	        if (num.AbsBasisTicks != 0 && numerator != 0)
	        {
		        fraction = " " + numerator.ToString() + "|" + num.AbsBasisTicks.ToString() + suffix;
	        }
	        else
	        {
		        whole += suffix;
	        }

	        if (whole == "")
	        {
		        fraction = sign + fraction;
	        }
	        //var wlen = whole.Length;
	        //whole = whole.PadRight(fraction.Length + 1);
	        //fraction = fraction.PadLeft(wlen + 1);
            return (whole, fraction);
        }

        private SKPoint DrawMarkerPointer(float t)
	    {
		    var sign = UnitDirectionOnDomainLine;
            var w = 5.0f * sign;
		    var unitSeg = BasisSegment;
		    var markerHW = (float) (1.0 / BasisSegment.Length) * w;
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
		    var tickLen = BasisSegment.Length / tickCount;
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
	        var pts = BasisSegment.PerpendicularLine(t, offset);
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
