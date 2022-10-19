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
            foreach (var trait in Trait.Traits)
            {
                DrawHLine(margin, Width - margin, traitY, Pens.GrayPen);
                foreach (var domain in trait.Domains.Values)
                {
	                var domainScale = nlLength / (float)domain.MaxRange.LengthInTicks;
	                var unitRatio = domain.UnitRatio;
                    var zeroPt = nlLength * unitRatio.Start + margin;
                    var unitPt = nlLength * unitRatio.End + margin;
                    DrawTick(zeroPt, traitY, Pens.TickBoldPen);
                    DrawTick(unitPt, traitY, Pens.TickPen);

                    DrawHLine(zeroPt, unitPt, traitY, Pens.HighlightPen);
                    foreach (var number in domain.Numbers.Values)
                    {
						traitY += 6;
	                    var nr = number.Ratio;
	                    var start = nlLength * nr.Start + margin;
	                    var end = nlLength * nr.End + margin;
                        DrawHLine(start, end, traitY, Pens.SegPen);
                    }
                }
            }
        }

	    public void DrawHLine(float start, float end, float y, SKPaint paint)
	    {
		    Canvas.DrawLine(start, y, end, y, paint);
	    }
	    public void DrawTick(float pos, float y, SKPaint paint)
	    {
		    Canvas.DrawLine(pos, y, pos, y - 8, paint);
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
