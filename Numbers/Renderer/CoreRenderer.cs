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
	    Dictionary<int, DomainRenderer> _domainRenderers = new Dictionary<int, DomainRenderer>();
        public CoreRenderer() : base()
        {
        }

        DomainRenderer GetOrCreateDomainRenderer(Domain domain, SKPoint startPoint, SKPoint endPoint)
        {
	        DomainRenderer result;
	        if (_domainRenderers.ContainsKey(domain.Id))
	        {
		        result = _domainRenderers[domain.Id];
	        }
	        else
	        {
                result = new DomainRenderer(this, domain, startPoint, endPoint);
                _domainRenderers[domain.Id] = result;
	        }
	        return result;
        }
	    public override void Draw()
	    {
		    base.Draw();
            SKPoint center = new SKPoint(Width/2f, Height/2f);
            var armLen = 280;
            SKPoint[] startPoints = new SKPoint[] { center - new SKPoint(armLen, 0), center + new SKPoint(0, armLen) };
            SKPoint[] endPoints = new SKPoint[] { center + new SKPoint(armLen, 0), center - new SKPoint(0, armLen) };
            long traitY = (long)center.Y;
		    long margin = 50;
		    long nlLength = Width - margin * 2;
            foreach (var trait in Trait.TraitStore)
            {
	            int index = 0;
                foreach (var domain in trait.DomainStore.Values)
                {
					var _domainRenderer = GetOrCreateDomainRenderer(domain, startPoints[index], endPoints[index]);
					_domainRenderer.Draw();
					index++;
                }
                foreach (var transform in trait.TransformStore.Values)
                {
                    DrawTransform(transform);
                }
            }
        }

	    public void DrawTransform(Transform transform)
	    {
            var sel0 = transform.Selection[0];
            var reps = transform.Repeats;
            var selRatio = sel0.Ratio;
            var repRatio = reps.Ratio;
            var selDr = _domainRenderers[sel0.Domain.Id];
            var repDr = _domainRenderers[reps.Domain.Id];
            var p0 = selDr.DomainSeg.PointAlongLine(selRatio.End);
            var p1 = repDr.DomainSeg.PointAlongLine(repRatio.End);
            Canvas.DrawLine(p0, p1, Pens.BondPen);
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
