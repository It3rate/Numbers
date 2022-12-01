using System;
using System.Collections.Generic;
using System.Linq;
using Numbers.Agent;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using SkiaSharp;

namespace Numbers.Mappers
{
	public class SKDomainMapper : SKMapper
	{
	    public Domain Domain => (Domain)MathElement;

	    private Dictionary<int, SKNumberMapper> NumberMappers = new Dictionary<int, SKNumberMapper>();
	    public SKNumberMapper NumberMapper(Number number) => GetOrCreateNumberMapper(number);
	    public SKNumberMapper NumberMapper(int numId) => GetOrCreateNumberMapper(Domain.GetNumber(numId));

        private SKNumberMapper BasisMapper => NumberMapper(Domain.BasisNumber);
	    public Number BasisNumber => Domain.BasisNumber;
	    public SKSegment BasisSegment => BasisMapper.Guideline;
	    public int BasisNumberSign => BasisNumber.Direction;

        public List<SKPoint> TickPoints = new List<SKPoint>();
	    public List<int> ValidNumberIds = new List<int>();
	    public Range DisplayLineRange => BasisSegment.RatiosAsBasis(Guideline);
	    public Range UnitRangeOnDomainLine
	    {
		    get => Guideline.RatiosAsBasis(BasisSegment);
		    set => BasisMapper.Reset(SegmentAlongGuideline(value));
	    }
	    public int UnitDirectionOnDomainLine => BasisSegment.DirectionOnLine(Guideline);

        public bool ShowGradientNumberLine;
	    public bool ShowTicks;
	    public bool ShowNumberOffsets;
	    public bool ShowKeyValues;
	    public bool ShowValueMarkers = true;
	    public bool ShowBasis;
	    public bool ShowBasisMarkers;
	    public bool ShowUnotArrow;
	    public bool ShowMaxMinValues;
	    public bool ShowDashedValuesOutOfRange;
	    public bool ShowFractions => WorkspaceMapper.ShowFractions;

        public SKDomainMapper(MouseAgent agent, Domain domain, SKSegment guideline, SKSegment unitSegment) : base(agent, domain, guideline)
	    {
		    var unit = Domain.BasisNumber;
		    var unitMapper = NumberMapper(unit);
		    unitMapper.Guideline.Reset(guideline.ProjectPointOnto(unitSegment.StartPoint), guideline.ProjectPointOnto(unitSegment.EndPoint));
	    }

        public SKNumberMapper AddNumberMapper(SKNumberMapper numberMapper)
        {
	        NumberMappers[numberMapper.Id] = numberMapper;
	        return numberMapper;
        }
        public bool RemoveNumberMapper(SKNumberMapper numberMapper) => NumberMappers.Remove(numberMapper.Id);
        
        public IEnumerable<SKNumberMapper> GetNumberMappers(bool reverse = false)
        {
	        var mappers = reverse ? NumberMappers.Values.Reverse() : NumberMappers.Values;
	        foreach (var dm in mappers)
	        {
		        yield return dm;
	        }
        }
        public SKNumberMapper GetOrCreateNumberMapper(int id)
        {
	        return GetOrCreateNumberMapper(Domain.NumberStore[id]);
        }
        public SKNumberMapper GetOrCreateNumberMapper(Number number)
        {
	        if (!NumberMappers.TryGetValue(number.Id, out var result))
	        {
		        result = new SKNumberMapper(Agent, number);
		        NumberMappers[number.Id] = result;
	        }
	        return (SKNumberMapper)result;
        }

        public override void Reset(SKPoint startPoint, SKPoint endPoint)
	    {
            base.Reset(startPoint, endPoint);
            AddValidNumbers();
	    }
	    public SKSegment SegmentAlongGuideline(Range ratio) => Guideline.SegmentAlongLine(ratio);

        private void AddValidNumbers()
	    {
		    ValidNumberIds.Clear();
		    foreach (var num in Domain.Numbers())
		    {
			    if (Workspace.IsElementActive(num.Id) && num.Id != Domain.BasisNumber.Id && num.Id != Domain.MinMaxNumber.Id)
			    {
				    ValidNumberIds.Add(num.Id);
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
            BasisMapper.Reset(SegmentAlongGuideline(unitRatio));
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
		    if (ShowBasis)
		    {
			    NumberMapper(Domain.BasisNumber).DrawUnit();
            }
	    }
        private void DrawNumbers()
        {
	        var offset = ShowNumberOffsets ? 1f : 0f;
	        var step = ShowNumberOffsets ? 1f : 0f;
	        foreach (var numberId in ValidNumberIds)
	        {
		        offset += step;
		        var nm = NumberMapper(numberId);
		        var pen = Pens.SegPens[Domain.CreationIndex % Pens.SegPens.Count];
		        nm.DrawNumber(offset, pen);

		        if (Domain.IsUnitPerspective)
		        {
			        var offsetScale = pen.StrokeWidth / Pens.UnitInlinePen.StrokeWidth;
			        nm.DrawNumber(offset * offsetScale, Pens.UnitInlinePen);
		        }
		        else
		        {
			        var offsetScale = pen.StrokeWidth / Pens.UnotInlinePen.StrokeWidth;
			        nm.DrawNumber(offset * offsetScale, Pens.UnotInlinePen);
		        }
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
			        var nm = Domain.GetNumber(id);
			        DrawMarker(nm, true);
			        DrawMarker(nm, false);
		        }
            }
        }
        private void DrawMarker(Number num, bool isStart)
        {
            var value = isStart ? num.Value.StartF : num.Value.EndF;
            var t = isStart ? num.ValueInRenderPerspective.StartF : num.ValueInRenderPerspective.EndF;
            var suffix = isStart ? "i" : "";
            var isUnitPersp = num.IsUnitPerspective;
		    var unitLabel = num.IsBasis && isStart ? "0" : isUnitPersp ? "1" : !isUnitPersp ? "i" : "";

		    var txtPoint = DrawMarkerPointer(t);

		    var numPaint = (isUnitPersp && isStart) || (!isUnitPersp && !isStart) ? Pens.UnotMarkerText : Pens.UnitMarkerText;
		    var txtBkgPen = Pens.TextBackgroundPen;
		    if (num.IsBasis)
		    {
			    var txt = isStart ? "0" : isUnitPersp ? "1" : "i";
			    var unitPaint = isUnitPersp ? Pens.UnitMarkerText : Pens.UnotMarkerText;
                Renderer.DrawText(txtPoint, txt, unitPaint, txtBkgPen);
            }
            else if (ShowFractions)
		    {
			    var parts = GetFractionText(num, isStart, suffix);
                Renderer.DrawFraction(parts, txtPoint, numPaint, txtBkgPen);
            }
		    else
		    {
				var txt = unitLabel != "" ? unitLabel : Math.Abs(value - (int) value) < 0.1f ? $"{value:0}{suffix}" : $"{value:0.0}{suffix}";
				Renderer.DrawText(txtPoint, txt, numPaint, txtBkgPen);
            }
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
		    var renderDir = UnitDirectionOnDomainLine;
		    var basisDir = BasisNumber.BasisFocal.Direction;
		    var gsp = renderDir * basisDir == 1 ? Guideline.StartPoint : Guideline.EndPoint;
		    var gep = renderDir * basisDir == 1 ? Guideline.EndPoint : Guideline.StartPoint;
            if (ShowGradientNumberLine)
		    {
			    var pnt = CorePens.GetGradientPen(gsp, gep, Pens.UnotLineColor, Pens.UnitLineColor, 10);
			    Renderer.DrawSegment(Guideline, pnt);
		    }
		    else if (!ShowBasis) // if not showing units at least color the line
		    {
			    var pnt = CorePens.GetGradientPen( gsp, gep, Pens.UnotLineColor, Pens.UnitLineColor, 3);
			    Renderer.DrawSegment(Guideline, pnt);
		    }

            Renderer.DrawSegment(Guideline, Renderer.Pens.NumberLinePen);
	    }
        private void DrawTicks()
	    {
		    if (BasisSegment.AbsLength < 4)
		    {
			    return;
		    }
            var offsetRange = -8f;
            var tickCount = Domain.BasisNumber.AbsBasisTicks;
            var tickToBasisRatio = Domain.TickToBasisRatio;
            var upDir = UnitDirectionOnDomainLine;
            // basis ticks
            var rangeInBasis = DisplayLineRange.ClampInner(); 
            TickPoints.Clear();
            for (long i = (long)rangeInBasis.Min; i <= (long)rangeInBasis.Max; i++)
            {
	            var isOnTick = Math.Abs(tickToBasisRatio) <= 1.0 ? true : (long)(i / tickToBasisRatio) * (long)tickToBasisRatio == i;
	            var offset = isOnTick ? offsetRange : offsetRange / 8f;
                TickPoints.Add(DrawTick((float)i, offset * upDir, Renderer.Pens.TickBoldPen));  
            }

            // minor ticks
            var rangeInTicks = Domain.ClampToInnerTick(DisplayLineRange);
		    var showMinorTicks = Math.Abs(BasisSegment.Length / tickToBasisRatio) >= 3; // don't show tiny ticks
            if (showMinorTicks)
            {
			    var offset = Domain.BasisIsReciprocal ? offsetRange * 2.5f : offsetRange;
	            for (var i = rangeInTicks.Min; i <= rangeInTicks.Max; i += Math.Abs(tickToBasisRatio))
	            {
		            if (i != 0) // don't draw tick on origin
		            {
			            DrawTick((float)i, offset * upDir, Renderer.Pens.TickPen);
                    }
	            }
            }
        }
        private SKPoint DrawTick(float t, float offset, SKPaint paint)
        {
	        var pts = BasisSegment.PerpendicularLine(t, offset);
	        Renderer.DrawLine(pts.Item1, pts.Item2, paint);
	        return pts.Item1;
        }

        private (string, string) GetFractionText(Number num, bool isStart, string suffix)
        {
	        var whole = "0";
	        var fraction = "";
            var val = isStart ? num.StartValue : num.EndValue;
            if (val != 0)
            {
	            var sign = isStart ? (num.StartValue >= 0 ? "" : "-") : (num.EndValue >= 0 ? "" : "-");
	            var wholeNum = isStart ? num.WholeStartValue : num.WholeEndValue;
	            whole = wholeNum == 0 ? "" : wholeNum.ToString();
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
            }

            return (whole, fraction);
        }

        public override SKPath GetHighlightAt(Highlight highlight)
	    {
		    return Renderer.GetCirclePath(highlight.SnapPoint);
	    }
	}
}
