using System.Drawing;
using SkiaSharp;
using Numbers.Core;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CoreRenderer : RendererBase
    {
	    public CoreRenderer() : base()
	    {
	    }

	    public override void Draw()
	    {
		    base.Draw();
		    Canvas.DrawCircle(200, 600, 50, Pens.DrawPen);
		    long traitY = 100;
		    long margin = 50;
		    long nlLength = Width - margin * 2;
		    var dr = new DomainRenderer(this);
            foreach (var trait in Trait.Traits)
            {
	            var startPoint = new SKPoint(margin, traitY);
	            var endPoint = new SKPoint(Width - margin, traitY);
                foreach (var domain in trait.Domains.Values)
                {
					dr.Reset(domain, startPoint, endPoint);
					dr.Draw();
                }
            }
        }

	    public void DrawSegment(SKSegment seg, SKPaint paint)
	    {
		    Canvas.DrawLine(seg.StartPoint, seg.EndPoint, paint);
	    }
        public void DrawHLine(float start, float end, float y, SKPaint paint)
	    {
		    Canvas.DrawLine(start, y, end, y, paint);
        }
        public void DrawTick(float pos, float y, SKPaint paint)
        {
	        Canvas.DrawLine(pos, y, pos, y - 8, paint);
        }
        public void DrawTick(SKPoint pt, SKPaint paint)
        {
	        Canvas.DrawLine(pt.X, pt.Y, pt.X, pt.Y - 8, paint);
        }


        public override void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
	    {
		    float round = radius / 3f;
		    var box = new SKRect(point.X - radius, point.Y - radius, point.X + radius, point.Y + radius);
		    Canvas.DrawRoundRect(box, round, round, paint);
	    }
	    public override void DrawPolyline(SKPoint[] polyline, SKPaint paint)
	    {
		    Canvas.DrawPoints(SKPointMode.Polygon, polyline, paint);
	    }
	    public override void DrawPath(SKPoint[] polyline, SKPaint paint)
	    {
		    var path = new SKPath
		    {
			    FillType = SKPathFillType.EvenOdd
		    };
		    path.MoveTo(polyline[0]);
		    path.AddPoly(polyline, true);
		    Canvas.DrawPath(path, paint);
	    }
	    public override void DrawDirectedLine(Number seg, SKPaint paint)
	    {
		    //DrawPolyline(seg.Points, paint);
		    //Canvas.DrawCircle(seg.StartPoint, 2, paint);
		    //var triPts = seg.EndArrow(8);
		    //Canvas.DrawPoints(SKPointMode.Polygon, triPts, paint);
	    }
	    public override void DrawText(SKPoint center, string text, SKPaint paint)
	    {
		    var rect = GetTextBackgroundSize(center.X, center.Y, text, paint);
		    Canvas.DrawRoundRect(rect, 5, 5, Pens.TextBackgroundPen);
		    Canvas.DrawText(text, center.X, center.Y, paint);
	    }
	    public override void DrawBitmap(SKBitmap bitmap)
	    {
		    Canvas.DrawBitmap(bitmap, new SKRect(0, 0, Width, Height));
	    }

	    public override void GeneratePens()
	    {
		    Pens = new CorePens(1);
	    }
    }
}
