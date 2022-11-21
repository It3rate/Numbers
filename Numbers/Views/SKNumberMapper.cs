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
	    public bool IsUnit => Number.IsUnit;
        public bool IsUnot => Number.IsUnot;
        public int BasisSign => Number.BasisFocal.Direction;

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
		    //EnsureSegment();
	    }
        public void EnsureSegment()
        {
	        var val = Number.ValueInFullUnitPerspective;
	        NumberSegment = UnitSegment.SegmentAlongLine(val.StartF, val.EndF);
	    }
        public void DrawNumber(float offsetScale, SKPaint paint)
        {
			EnsureSegment();
			var dir = Number.Direction;
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
	        var us = DomainMapper.BasisSegment;
	        var pt = us.ProjectPointOnto(point, false);
            var (t, _) = us.TFromPoint(pt, false);
	        t = (float)(Math.Round(t * us.Length) / us.Length);
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
        public void SetStartValueByPoint(SKPoint newPoint)
        {
	        Number.StartValue = -TFromPoint(newPoint) * BasisSign;
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
	        Number.EndValue = TFromPoint(newPoint) * BasisSign;
        }
        public void SetValueOfBasis(SKPoint newPoint, UIKind kind)
        {
	        var ds = DomainMapper.DisplayLine;
	        var pt = ds.ProjectPointOnto(newPoint);
	        var nsc = NumberSegment.Clone();
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
