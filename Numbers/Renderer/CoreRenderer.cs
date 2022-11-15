using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using SkiaSharp;
using Numbers.Core;
using Numbers.Mind;
using Numbers.UI;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CoreRenderer : RendererBase
    {
        public CoreRenderer()
        {
        }

	    public override void Draw()
	    {
		    base.Draw();
		    CurrentWorkspace.EnsureRenderers();
            CurrentWorkspace.Draw();
        }

	    public override SKPath GetCirclePath(SKPoint center, float radius = 10)
	    {
		    var path = new SKPath();
		    path.AddCircle(center.X, center.Y, radius);
		    return path;
	    }

        public override void DrawSegment(SKSegment seg, SKPaint paint)
        {
		    Canvas.DrawLine(seg.StartPoint, seg.EndPoint, paint);
	    }
	    public override void DrawLine(SKPoint p0, SKPoint p1, SKPaint paint)
	    {
		    Canvas.DrawLine(p0, p1, paint);
	    }

	    public override void DrawRoundBox(SKPoint point, SKPaint paint, float radius = 8f)
	    {
		    float round = radius / 3f;
		    var box = new SKRect(point.X - radius, point.Y - radius, point.X + radius, point.Y + radius);
		    Canvas.DrawRoundRect(box, round, round, paint);
	    }
	    public override void DrawPolyline(SKPaint paint, params SKPoint[] polyline)
	    {
		    Canvas.DrawPoints(SKPointMode.Polygon, polyline, paint);
	    }
	    public override void FillPolyline(SKPaint paint, params SKPoint[] polyline)
	    {
		    var path = new SKPath
		    {
			    FillType = SKPathFillType.EvenOdd
		    };
		    path.MoveTo(polyline[0]);
		    path.AddPoly(polyline, true);
		    Canvas.DrawPath(path, paint);
	    }
	    public override void DrawDirectedLine(SKSegment seg, bool isUnitPerspective, SKPaint paint)
	    {
            DrawPolyline(paint, seg.Points);
            Canvas.DrawCircle(seg.StartPoint, 2, paint);
            var triPts = seg.EndArrow(8);
            Canvas.DrawPoints(SKPointMode.Polygon, triPts, paint);
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

        public override void GeneratePens(ColorTheme colorTheme = ColorTheme.Normal)
	    {
		    Pens = new CorePens(1, colorTheme);
	    }
    }
}
