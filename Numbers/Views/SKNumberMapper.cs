﻿using System;
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

        private SKDomainMapper DomainMapper => WorkspaceMapper.DomainMapper(Number.Domain.Id);
	    public bool IsUnitOrUnot => Number.IsBasis;
	    public bool IsUnit => Number.IsUnit;
        public bool IsUnot => Number.IsUnot;
        public int UnitSign => DomainMapper.UnitSign;

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

	    public SKSegment UnitSegment => DomainMapper.UnitSegment;

	    public SKNumberMapper(Workspace workspace, Number number) : base(workspace, number)
	    {
		    Number = number;
		    //EnsureSegment();
	    }
        public int SegmentDirection => DomainMapper.DisplayLine.DirectionOnLine(NumberSegment);
        public void EnsureSegment()
        {
            var val = DomainMapper.UnitSign == 1 ? Number.ValueInUnitPerspective : Number.ValueInUnotPerspective;
            NumberSegment = UnitSegment.SegmentAlongLine((float)val.Start, (float)val.End);
	    }
        public void DrawNumber(float offsetScale, SKPaint paint)
        {
	        if (Number.Id != Number.Domain.BasisNumberId)
	        {
				EnsureSegment();
		        var dir = Number.Direction;
		        var offset = NumberSegment.RelativeOffset(paint.StrokeWidth / 2f * offsetScale * dir * UnitSign);
		        RenderSegment = NumberSegment + offset;
		        Renderer.DrawDirectedLine(RenderSegment, Number.IsUnitPerspective, paint);
            }
        }
        public void DrawUnit()
        {
            // BasisNumber is a special case where we don't want it's direction set by the unit direction of the line (itself).
            // So don't call EnsureSegment here.
	        if (SegmentDirection != UnitSign)
	        {
                // Invert unit if dragging past zero point.
		        Number.Focal.EndTickPosition = Number.AbsBasisTicks * SegmentDirection + Number.Focal.StartTickPosition;
            }
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
	        var us = DomainMapper.UnitSegment;
	        var pt = us.ProjectPointOnto(point, false);
            var (t, _) = us.TFromPoint(pt, false);
	        t = (float)(Math.Round(t * us.Length) / us.Length);
	        //Console.WriteLine(t);
	        return t;
        }
        public void SetValueByKind(SKPoint newPoint, UIKind kind)
        {
	        if (kind.IsUnit())
	        {
	            var ds = DomainMapper.DisplayLine;
	            var pt = ds.ProjectPointOnto(newPoint);
	            if (kind.IsMajor())
	            {
		            NumberSegment.EndPoint = pt;
	            }
	            else
	            {
		            NumberSegment.StartPoint = pt;
	            }
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
	        Number.StartValue = -TFromPoint(newPoint);
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
	        Number.EndValue = TFromPoint(newPoint);
        }

        public override SKPath HighlightAt(float t, SKPoint targetPoint)
        {
	        return Renderer.GetCirclePath(targetPoint);
        }
    }
}