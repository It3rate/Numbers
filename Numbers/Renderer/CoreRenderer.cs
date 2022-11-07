using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using SkiaSharp;
using Numbers.Core;
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
	    //public Dictionary<int, TransformRenderer> TransformRenderers = new Dictionary<int, TransformRenderer>();
	    public Dictionary<int, SKMapper> Mappers = new Dictionary<int, SKMapper>();
	    public SKDomainMapper DomainMapper(int id) => (SKDomainMapper)Mappers[id];
	    public SKTransformMapper TransformMapper(int id) => (SKTransformMapper)Mappers[id];
	    public SKNumberMapper NumberMapper(int id) => (SKNumberMapper)Mappers[id];

        public CoreRenderer() : base()
        {
        }

	    public override void Draw()
	    {
		    base.Draw();
		    EnsureRenderers();
            foreach (var trait in Trait.TraitStore.Values)
            {
                foreach (var transform in trait.TransformStore.Values)
                {
	                TransformMapper(transform.Id).Draw();
                }
                foreach (var domain in trait.DomainStore.Values)
                {
	                DomainMapper(domain.Id).Draw();
                }
            }
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
	    public override void DrawShape(SKPoint[] polyline, SKPaint paint)
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


	    public IEnumerable<SKMapper> MappersOfKind(MathElementKind kind)
	    {
		    var values = Mappers.Where(kvp => kvp.Value.MathElement.Kind == kind);
		    foreach (var kvp in values)
		    {
			    yield return kvp.Value;
		    }
	    }
	    public IEnumerable<SKMapper> MappersOfKindReversed(MathElementKind kind)
	    {
		    var values = Mappers.Where(kvp => kvp.Value.MathElement.Kind == kind).Reverse();
		    foreach (var kvp in values)
		    {
			    yield return kvp.Value;
		    }
	    }


        private void EnsureRenderers()
	    {
		    var cx = Width / 2f - 100;
		    var cy = Height / 2f;
            var armLen = 280;
            // all this etc will be a workspace element eventually
            var lines = new []{ new SKSegment(cx - armLen, cy, cx + armLen, cy), new SKSegment(cx, cy + armLen, cx, cy - armLen) };

		    foreach (var trait in Trait.TraitStore.Values)
		    {
			    int index = 0;
			    foreach (var domain in trait.DomainStore.Values)
			    {
				    GetOrCreateDomainMapper(domain, lines[index].StartPoint, lines[index].EndPoint);
				    foreach (var number in domain.Numbers())
				    {
					    GetOrCreateNumberMapper(number);
				    }
				    index++;
			    }
			    foreach (var transform in trait.TransformStore.Values)
			    {
				    GetOrCreateTransformMapper(transform);
			    }
		    }
        }

        public SKDomainMapper GetOrCreateDomainMapper(Domain domain, SKPoint startPoint, SKPoint endPoint)
        {
	        if (!Mappers.TryGetValue(domain.Id, out var result))
	        {
		        result = new SKDomainMapper(this, domain, startPoint, endPoint);// DomainRenderer(this, domain, startPoint, endPoint);
		        Mappers[domain.Id] = result;
	        }
	        return (SKDomainMapper)result;
        }
        public SKNumberMapper GetOrCreateNumberMapper(Number number)
        {
	        if (!Mappers.TryGetValue(number.Id, out var result))
	        {
		        result = new SKNumberMapper(this, number); // DomainRenderer(this, domain, startPoint, endPoint);
		        Mappers[number.Id] = result;
	        }

	        return (SKNumberMapper)result;
        }
        public SKTransformMapper GetOrCreateTransformMapper(Transform transform)
        {
	        if (!Mappers.TryGetValue(transform.Id, out var result))
	        {
		        result = new SKTransformMapper(this, transform);
		        Mappers[transform.Id] = result;
	        }
	        return (SKTransformMapper)result;
        }
        public override void GeneratePens()
	    {
		    Pens = new CorePens(1);
	    }
    }
}
