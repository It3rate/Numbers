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

    public class SKNumberMapper : SKMapper
    {
        public Number Number { get; }
        public SKSegment NumberSegment { get; set; }
        public SKSegment RenderSegment { get; private set; }

        private SKDomainMapper DomainMapper => WorkspaceMapper.DomainMapper(Number.Domain.Id);
	    public bool IsUnitOrUnot => Number.IsUnitOrUnot;
	    public bool IsUnit => Number.IsUnit;
        public bool IsUnot => Number.IsUnot;

        public override SKPoint StartPoint
	    {
		    get => NumberSegment.StartPoint;
		    set
		    {
			    (Number.StartT, _) = UnitSegment.TFromPoint(value, false);
			    EnsureSegment();
            }
	    }
	    public override SKPoint MidPoint => NumberSegment.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => NumberSegment.EndPoint;
		    set
		    {
			    (Number.EndT, _) = UnitSegment.TFromPoint(value, false);
			    EnsureSegment();
		    }
	    }

	    public SKSegment UnitSegment => DomainMapper.UnitSegment;

	    public SKNumberMapper(Workspace workspace, Number number) : base(workspace, number)
	    {
		    Number = number;
		    //EnsureSegment();
	    }

	    public void EnsureSegment()
	    {
		    var val = Number.ValueInUnitPerspective;
            NumberSegment = UnitSegment.SegmentAlongLine((float)val.Imaginary, (float)val.Real);
	    }
        public void DrawNumber(float offsetScale, SKPaint paint)
        {
	        if (Number.Id != Number.Domain.UnitId)
	        {
				EnsureSegment();
		        var dir = Number.Direction;
		        var offset = NumberSegment.RelativeOffset(paint.StrokeWidth / 2f * offsetScale * dir);
		        RenderSegment = NumberSegment + offset;
		        Renderer.DrawDirectedLine(RenderSegment, Number.IsUnitPerspective, paint);
            }
        }
        public void DrawUnit()
        {
	        EnsureSegment();
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
	        Number.StartT = TFromPoint(newPoint);
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
	        Number.EndT = TFromPoint(newPoint);
        }

        public override SKPath HighlightAt(float t, SKPoint targetPoint)
        {
	        return Renderer.GetCirclePath(targetPoint);
        }
    }
}
