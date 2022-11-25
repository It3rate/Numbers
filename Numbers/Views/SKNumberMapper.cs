using System;
using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public class SKNumberMapper : SKMapper
    {
        public Number Number { get; }
        public SKSegment NumberSegment { get; set; }
        public SKSegment RenderSegment { get; private set; }

        public SKDomainMapper DomainMapper => WorkspaceMapper.DomainMapper(Number.Domain.Id);
        public SKSegment UnitSegment => DomainMapper.BasisSegment;
        public bool IsBasis => Number.IsBasis;
        public int BasisSign => Number.BasisFocal.Direction;

        public int UnitDirectionOnDomainLine => NumberSegment.DirectionOnLine(DomainMapper.DisplayLine);

        public override SKPoint StartPoint
	    {
		    get => NumberSegment.StartPoint;
		    set
		    {
			    Number.StartValue = -UnitSegment.TFromPoint(value, false).Item1;
			    EnsureSegment();
            }
	    }
	    public override SKPoint MidPoint => NumberSegment.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => NumberSegment.EndPoint;
		    set
		    {
			    Number.EndValue = UnitSegment.TFromPoint(value, false).Item1;
			    EnsureSegment();
		    }
	    }


	    public SKNumberMapper(Workspace workspace, Number number) : base(workspace, number)
	    {
		    Number = number;
	    }
        public void EnsureSegment()
        {
	        var val = Number.ValueInRenderPerspective;
	        //var invertDirection = Number.BasisFocal.IsUnotPerspective;
	        NumberSegment = UnitSegment.SegmentAlongLine(val.StartF, val.EndF);//, invertDirection);
	    }
        public void DrawNumber(float offsetScale, SKPaint paint)
        {
			EnsureSegment();
			var dir = UnitDirectionOnDomainLine;
	        var offset = NumberSegment.RelativeOffset(paint.StrokeWidth / 2f * offsetScale * dir);
	        RenderSegment = NumberSegment + offset;
	        Renderer.DrawDirectedLine(RenderSegment, Number.IsUnitPerspective, paint);
        }

        public void DrawUnit()
        {
            // BasisNumber is a special case where we don't want it's direction set by the unit direction of the line (itself).
            // So don't call EnsureSegment here.
            var dir = Number.Direction;
            var pen = dir > 0 ? Pens.UnitPen : Pens.UnotPen;
	        var offset = NumberSegment.OffsetAlongLine(0,  pen.StrokeWidth / 2f * dir) - NumberSegment.StartPoint;
	        RenderSegment = NumberSegment - offset;
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
	        var change = originalSegment.RatiosAsBasis(NumberSegment);
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
	        else if (kind.IsMajor())
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
	        var dl = DomainMapper.DisplayLine;
	        var orgStartT = dl.TFromPoint(orgSeg.StartPoint, false).Item1;
	        var orgEndT = dl.TFromPoint(orgSeg.EndPoint, false).Item1;
	        NumberSegment.StartPoint = dl.PointAlongLine(orgStartT + diffT);
	        NumberSegment.EndPoint = dl.PointAlongLine(orgEndT + diffT);
        }

        public void SetStartValueByPoint(SKPoint newPoint)
        {
	        Number.StartValue = -TFromPoint(newPoint);
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
	        Number.EndValue = TFromPoint(newPoint);
        }
        public void SetValueOfBasis(SKPoint newPoint, UIKind kind)
        {
	        var pt = DomainMapper.DisplayLine.ProjectPointOnto(newPoint);
	        if (kind.IsMajor())
	        {
		        NumberSegment.EndPoint = pt;
	        }
	        else
	        {
		        NumberSegment.StartPoint = pt;
	        }
        }

        public override SKPath GetHighlightAt(float t, SKPoint targetPoint)
        {
	        return Renderer.GetCirclePath(targetPoint);
        }
    }
}
