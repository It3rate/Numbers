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
	    public SKSegment NumberSegment { get; private set; }

	    private SKDomainMapper DomainMapper => Workspace.DomainMapper(Number.Domain.Id);
	    public bool IsUnitOrUnot => Number.IsUnitOrUnot;
	    public bool IsUnit => Number.IsUnit;
        public bool IsUnot => Number.IsUnot;

        public override SKPoint StartPoint
	    {
		    get => NumberSegment.StartPoint;
		    set
		    {
			    (Number.StartT, _) = DomainSegment.TFromPoint(value, false);
			    EnsureSegment();
            }
	    }
	    public override SKPoint MidPoint => NumberSegment.Midpoint;
	    public override SKPoint EndPoint
	    {
		    get => NumberSegment.EndPoint;
		    set
		    {
			    (Number.EndT, _) = DomainSegment.TFromPoint(value, false);
			    EnsureSegment();
		    }
	    }

	    public SKSegment DomainSegment => DomainMapper.DomainSegment;

	    public SKNumberMapper(Workspace workspace, Number number) : base(workspace, number)
	    {
		    Number = number;
		    EnsureSegment();
	    }

	    public void EnsureSegment()
	    {
		    var nr = Number.Ratio;
            NumberSegment = DomainSegment.SegmentAlongLine(nr.Start, nr.End);
	    }
        public void DrawNumber(float offsetScale, SKPaint paint)
        {
	        EnsureSegment();
	        if (Number.Id != Number.Domain.UnitId)
	        {
		        var dir = Number.Direction;
		        var offset = NumberSegment.RelativeOffset(paint.StrokeWidth / 2f * offsetScale * dir);
		        var seg = NumberSegment + offset;
		        Renderer.DrawDirectedLine(seg, Number.IsUnitPerspective, paint);
            }
        }
        public void DrawUnit()
        {
	        EnsureSegment();
            var dir = Number.Direction;
            Console.WriteLine(dir);
            var pen = dir > 0 ? Pens.UnitPen : Pens.UnotPen;
	        var offset = NumberSegment.OffsetAlongLine(0,  pen.StrokeWidth / 2f * dir) - NumberSegment.StartPoint;
	        var seg = NumberSegment - offset;
	        if (Pens.UnitStrokePen != null)
	        {
		        Renderer.DrawSegment(seg, Pens.UnitStrokePen);
            }
            Renderer.DrawSegment(seg, pen);
        }

        public float TFromPoint(SKPoint point)
        {
	        var ds = DomainMapper.DomainSegment;
	        var pt = ds.ProjectPointOnto(point);
            var (t, _) = ds.TFromPoint(pt, false);
	        t = (float)(Math.Round(t * ds.Length) / ds.Length);
	        return t;
        }
        public void SetValueByKind(SKPoint newPoint, UIKind kind)
        {
            if (kind.IsMajor())
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
	        var pt = NumberSegment.PointAlongLine(t);
	        return Renderer.GetCirclePath(pt);
        }
    }
}
