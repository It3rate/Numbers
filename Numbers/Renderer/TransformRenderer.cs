using System.Drawing;
using Numbers.Core;
using SkiaSharp;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TransformRenderer
    {
        private CoreRenderer _renderer;
        private SKCanvas Canvas => _renderer.Canvas;
        private CorePens _pens;
        public Transform _transform { get; private set; }

        private TransformRenderer(CoreRenderer renderer)
        {
            _renderer = renderer;
            _pens = _renderer.Pens;
        }
        public TransformRenderer(CoreRenderer renderer, Transform transform) : this(renderer)
        {
            Reset(transform);
        }

        public void Reset(Transform transform)
        {
            _transform = transform;
        }

        public void Draw()
        {
	        var sel0 = _transform.Selection[0];
	        var reps = _transform.Repeats;
	        var selRatio = sel0.Ratio;
	        var repRatio = reps.Ratio;
	        var selDr = _renderer.DomainRenderers[sel0.Domain.Id];
	        var repDr = _renderer.DomainRenderers[reps.Domain.Id];
	        var org = selDr.DomainSeg.PointAlongLine(0.5f);
	        var a0 = selDr.DomainSeg.PointAlongLine(selRatio.Start);
            var a1 = selDr.DomainSeg.PointAlongLine(selRatio.End);
	        var b0 = repDr.DomainSeg.PointAlongLine(repRatio.Start);
	        var b1 = repDr.DomainSeg.PointAlongLine(repRatio.End);
	        var a0i = selDr.DomainSeg.PointAlongLine(1f - selRatio.Start);
	        var a1i = selDr.DomainSeg.PointAlongLine(1f - selRatio.End);
	        var b0i = repDr.DomainSeg.PointAlongLine(1f - repRatio.Start);
	        var b1i = repDr.DomainSeg.PointAlongLine(1f - repRatio.End);

	        DrawTriangle(sel0.EndValue * reps.EndValue > 0, unitBB_Pen, a1, b1, org);
	        DrawTriangle(sel0.StartValue * reps.EndValue > 0, unitAB_Pen, a1, b0i, org);
	        DrawTriangle(sel0.EndValue * reps.StartValue > 0, unitBA_Pen, b1, org, a0i);
	        DrawTriangle(sel0.StartValue * reps.StartValue < 0, unitAA_Pen, a0i, b0i, org);

            //_renderer.DrawPath(new SKPoint[] { a1, b1, org }, (sel0.EndValue * reps.EndValue > 0) ? unitBB_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { a1, b0i, org }, (sel0.StartValue * reps.EndValue > 0) ? unitAB_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { b1, org, a0i }, (sel0.EndValue * reps.StartValue > 0) ? unitBA_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { a0i, b0i, org },(sel0.StartValue * reps.StartValue < 0) ? unitAA_Pen : _pens.BkgBrushAlpha); // -
            _renderer.DrawPath(new SKPoint[] { a1, b1, a0i, b0i }, unitOutline);

            DrawTriangle(sel0.EndValue * reps.EndValue > 0, unotBB_Pen, a1i, b1i, org);
            DrawTriangle(sel0.StartValue * reps.EndValue > 0, unotAB_Pen,   a1i, b0, org);
            DrawTriangle(sel0.EndValue * reps.StartValue > 0, unotBA_Pen,   b1i, org, a0);
            DrawTriangle(sel0.StartValue * reps.StartValue < 0, unotAA_Pen, a0, b0, org);
            //_renderer.DrawPath(new SKPoint[] { a1i, b1i, org }, (sel0.EndValue * reps.EndValue > 0) ? unotBB_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { a1i, b0, org }, (sel0.StartValue * reps.EndValue > 0) ? unotAB_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { b1i, org, a0 }, (sel0.EndValue * reps.StartValue > 0) ? unotBA_Pen : _pens.BkgBrushAlpha); // +
            //_renderer.DrawPath(new SKPoint[] { a0, b0, org }, (sel0.StartValue * reps.StartValue < 0) ? unotAA_Pen : _pens.BkgBrushAlpha); // -
            _renderer.DrawPath(new SKPoint[] { a1i, b1i, a0, b0 }, unotOutline);
        }

        private void DrawTriangle(bool isPositive, SKPaint color, params SKPoint[] points)
        {
	        _renderer.DrawPath(points, color);
	        if (!isPositive)
	        {
		        _renderer.DrawPath(points, _pens.BkgBrushAlpha);
            }
        }

        private static readonly SKPaint unitBB_Pen = GetBrush(SKColor.Parse("#80F87A0E"));
        private static readonly SKPaint unitAB_Pen = GetBrush(SKColor.Parse("#80C8690B"));
        private static readonly SKPaint unitBA_Pen = GetBrush(SKColor.Parse("#80FBB968"));
        private static readonly SKPaint unitAA_Pen = GetBrush(SKColor.Parse("#80C93B0C"));
        private static readonly SKPaint unitOutline = GetPen(SKColor.Parse("#A0200000"), 4);

        private static readonly SKPaint unotBB_Pen = GetBrush(SKColor.Parse("#4000BCFD"));
        private static readonly SKPaint unotAB_Pen = GetBrush(SKColor.Parse("#40177B9E"));
        private static readonly SKPaint unotBA_Pen = GetBrush(SKColor.Parse("#4091E2FD"));
        private static readonly SKPaint unotAA_Pen = GetBrush(SKColor.Parse("#400060A9"));
        private static readonly SKPaint unotOutline = GetPen(SKColor.Parse("#A0000020"), 4);

        public static SKPaint GetBrush(SKColor color)
        {
	        SKPaint pen = new SKPaint()
	        {
		        Style = SKPaintStyle.Fill,
		        Color = color
	        };
	        return pen;
        }
        public static SKPaint GetPen(SKColor color, float width, bool antiAlias = true)
        {
	        SKPaint pen = new SKPaint()
	        {
		        Style = SKPaintStyle.Stroke,
		        Color = color,
		        StrokeWidth = width,
		        IsAntialias = antiAlias,
		        StrokeCap = SKStrokeCap.Round,
	        };
	        return pen;
        }
    }
}
