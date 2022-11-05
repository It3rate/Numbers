using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
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
	    public Dictionary<int, DomainRenderer> DomainRenderers = new Dictionary<int, DomainRenderer>();
	    public Dictionary<int, TransformRenderer> TransformRenderers = new Dictionary<int, TransformRenderer>();

        public CoreRenderer() : base()
        {
        }

	    public override void Draw()
	    {
		    base.Draw();
		    EnsureRenderers();
            foreach (var trait in Trait.TraitStore)
            {
                foreach (var transform in trait.TransformStore.Values)
                {
	                TransformRenderers[transform.Id].Draw();
                }
                foreach (var domain in trait.DomainStore.Values)
                {
					DomainRenderers[domain.Id].Draw();
                }
            }
        }

	    public DomainRenderer GetOrCreateDomainRenderer(Domain domain, SKPoint startPoint, SKPoint endPoint)
	    {
		    if (!DomainRenderers.TryGetValue(domain.Id, out var result))
		    {
			    result = new DomainRenderer(this, domain, startPoint, endPoint);
			    DomainRenderers[domain.Id] = result;
		    }
		    return result;
	    }
	    public TransformRenderer GetOrCreateTransformRenderer(Transform transform)
	    {
		    if (!TransformRenderers.TryGetValue(transform.Id, out var result))
		    {
			    result = new TransformRenderer(this, transform);
			    TransformRenderers[transform.Id] = result;
		    }
		    return result;
	    }

        public void DrawSegment(SKSegment seg, SKPaint paint)
	    {
		    Canvas.DrawLine(seg.StartPoint, seg.EndPoint, paint);
	    }
	    public void DrawLine(SKPoint p0, SKPoint p1, SKPaint paint)
	    {
		    Canvas.DrawLine(p0, p1, paint);
	    }
        public void DrawHLine(float start, float end, float y, SKPaint paint)
	    {
		    Canvas.DrawLine(start, y, end, y, paint);
        }
        public void DrawHTick(float x, float pos, SKPaint paint)
        {
	        Canvas.DrawLine(x, pos, x + 8, pos, paint);
        }
        public void DrawHTick(SKPoint pt, SKPaint paint)
        {
	        Canvas.DrawLine(pt.X, pt.Y, pt.X + 8, pt.Y, paint);
        }
        public void DrawVLine(float start, float end, float x, SKPaint paint)
        {
	        Canvas.DrawLine(x, start, x, end, paint);
        }
        public void DrawVTick(float pos, float y, SKPaint paint)
        {
	        Canvas.DrawLine(pos, y, pos, y - 8, paint);
        }
        public void DrawVTick(SKPoint pt, SKPaint paint)
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
	    public override void DrawDirectedLine(SKSegment seg, SKPaint paint)
	    {
            DrawPolyline(seg.Points, paint);
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


	    private void EnsureRenderers()
	    {
		    var cx = Width / 2f - 100;
		    var cy = Height / 2f;
            var armLen = 280;
            var lines = new []{ new SKSegment(cx - armLen, cy, cx + armLen, cy), new SKSegment(cx, cy + armLen, cx, cy - armLen) };

		    foreach (var trait in Trait.TraitStore)
		    {
			    int index = 0;
			    foreach (var domain in trait.DomainStore.Values)
			    {
				    var dr = GetOrCreateDomainRenderer(domain, lines[index].StartPoint, lines[index].EndPoint);
				    index++;
			    }
			    foreach (var transform in trait.TransformStore.Values)
			    {
				    var tr = GetOrCreateTransformRenderer(transform);
			    }
		    }
	    }
        public override void GeneratePens()
	    {
		    Pens = new CorePens(1);
	    }
    }
}
