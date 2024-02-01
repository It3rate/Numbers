using System;
using System.Diagnostics;
using System.IO.Ports;
using Numbers.Agent;
using Numbers.Utils;
using NumbersCore.Primitives;
using SkiaSharp;

namespace Numbers.Mappers
{
	public class SKNumberMapper : SKMapper
    {
	    public Number Number => (Number) MathElement;
        public SKSegment RenderSegment { get; private set; }

        public SKDomainMapper DomainMapper => WorkspaceMapper.DomainMapper(Number.Domain);
        public SKSegment UnitSegment => DomainMapper.BasisSegment;
        public bool IsBasis => Number.IsBasis;
        public int BasisSign => Number.BasisFocal.Direction;
        public Alignment Polarity { get => Number.Polarity; set => Number.Polarity = value; }
        public int UnitDirectionOnDomainLine => Guideline.DirectionOnLine(DomainMapper.Guideline);

        public SKNumberMapper(MouseAgent agent, Number number) : base(agent, number)
	    {
	    }
        public Alignment InvertPolarity()
        {
            return Number.InvertPolarity();
        }
        public void ResetNumber(Number number) => MathElement = number;
        public void EnsureSegment()
        {
	        var val = Number.ValueInRenderPerspective;
	        Reset(UnitSegment.SegmentAlongLine(val.StartF, val.EndF));
	    }


        public void DrawNumber(float offsetScale, SKPaint paint)
        {
			EnsureSegment();
			var dir = UnitDirectionOnDomainLine;
	        var offset = Guideline.RelativeOffset(paint.StrokeWidth / 2f * offsetScale * dir);
	        RenderSegment = Guideline + offset;
	        Renderer.DrawDirectedLine(RenderSegment, Number.IsAligned, paint);

            //var ef = Number.ExpansiveForce;
            //Trace.WriteLine(ef);
        }

        public void DrawUnit(bool aboveLine)
        {
            // BasisNumber is a special case where we don't want it's direction set by the unit direction of the line (itself).
            // So don't call EnsureSegment here.
            var dir = Number.Focal.Direction;
            var pen = Number.IsAligned ? Pens.UnitPenLight : Pens.UnotPenLight;
	        var offset = Guideline.OffsetAlongLine(0,  pen.StrokeWidth / 2f * dir) - Guideline.StartPoint;
	        RenderSegment = aboveLine ? Guideline + offset : Guideline - offset;
	        if (Pens.UnitStrokePen != null)
	        {
		        Renderer.DrawSegment(RenderSegment, Pens.UnitStrokePen);
            }
            Renderer.DrawSegment(RenderSegment, pen);
        }

        public float TFromPoint(SKPoint point)
        {
	        var basisSegment = DomainMapper.BasisSegment;
	        var pt = basisSegment.ProjectPointOnto(point, false);
            var (t, _) = basisSegment.TFromPoint(pt, false);
	        t = (float)(Math.Round(t * basisSegment.Length) / basisSegment.Length);
	        return t;
        }

        public void AdjustBySegmentChange(HighlightSet beginState) => AdjustBySegmentChange(beginState.OriginalSegment, beginState.OriginalFocalPositions);
        public void AdjustBySegmentChange(SKSegment originalSegment, FocalPositions originalFocalPositions)
        {
	        var change = originalSegment.RatiosAsBasis(Guideline);
	        var ofp = originalFocalPositions;
	        Number.Focal.Reset(
		        (long)(ofp.StartTickPosition + change.Start * ofp.Length),
		        (long)(ofp.EndTickPosition + (change.End - 1.0) * ofp.Length));
        }

        public void SetValueByKind(SKPoint newPoint, UIKind kind)
        {
	        if (kind.IsBasis())
	        {
		        SetValueOfBasis(newPoint, kind);
	        }
	        else if (kind.IsMajor() == Number.IsAligned)
            {
	            SetEndValueByPoint(newPoint);
            }
	        else
            {
	            SetStartValueByPoint(newPoint);
            }
        }

        public void MoveSegmentByT(SKSegment orgSeg, float diffT)
        {
	        var orgStartT = -DomainMapper.BasisSegment.TFromPoint(orgSeg.StartPoint, false).Item1;
	        var orgEndT = DomainMapper.BasisSegment.TFromPoint(orgSeg.EndPoint, false).Item1;
	        Number.StartValue = orgStartT - diffT;
	        Number.EndValue = orgEndT + diffT;
        }
        public void MoveBasisSegmentByT(SKSegment orgSeg, float diffT)
        {
	        var dl = DomainMapper.Guideline;
	        var orgStartT = dl.TFromPoint(orgSeg.StartPoint, false).Item1;
	        var orgEndT = dl.TFromPoint(orgSeg.EndPoint, false).Item1;
	        Guideline.StartPoint = dl.PointAlongLine(orgStartT + diffT);
	        Guideline.EndPoint = dl.PointAlongLine(orgEndT + diffT);
        }

        public void SetStartValueByPoint(SKPoint newPoint)
        {
            var dir = -Number.PolarityDirection;
            Number.StartValue = TFromPoint(newPoint) * dir;
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
            var dir = Number.PolarityDirection;
            Number.EndValue = TFromPoint(newPoint) * dir;
        }
        public void SetValueOfBasis(SKPoint newPoint, UIKind kind)
        {
	        var pt = DomainMapper.Guideline.ProjectPointOnto(newPoint);
	        if (kind.IsMajor())
	        {
		        Guideline.EndPoint = pt;
	        }
	        else
	        {
		        Guideline.StartPoint = pt;
	        }
        }

        public override SKPath GetHighlightAt(Highlight highlight)
        {
	        SKPath result;
	        if (highlight.Kind.IsLine())
	        {
		        result = Renderer.GetSegmentPath(RenderSegment, 0f);
	        }
	        else
	        {
		        result = Renderer.GetCirclePath(highlight.SnapPoint);
            }
	        return result;
        }
        public override string ToString()
        {
            return "nm:" + Number.ToString();
        }
    }
}
