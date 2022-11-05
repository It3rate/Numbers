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

	        DrawTriangle(sel0.EndValue * reps.EndValue > 0, unitBB_Pen, true, a1, b1, org); // +
	        DrawTriangle(sel0.StartValue * reps.EndValue > 0, unitAB_Pen, true, a1, b0i, org); // +
            DrawTriangle(sel0.EndValue * reps.StartValue > 0, unitBA_Pen, true, b1, org, a0i); // +
            DrawTriangle(sel0.StartValue * reps.StartValue < 0, unitAA_Pen, true, a0i, b0i, org);  // -
            _renderer.DrawPath(new SKPoint[] { a1, b1, a0i, b0i }, unitOutline);

            DrawTriangle(sel0.EndValue * reps.EndValue > 0, unotBB_Pen, false, a1i, b1i, org); // +
            DrawTriangle(sel0.StartValue * reps.EndValue > 0, unotAB_Pen, false,   a1i, b0, org); // +
            DrawTriangle(sel0.EndValue * reps.StartValue > 0, unotBA_Pen, false,   b1i, org, a0); // +
            DrawTriangle(sel0.StartValue * reps.StartValue < 0, unotAA_Pen, false, a0, b0, org); // -
            _renderer.DrawPath(new SKPoint[] { a1i, b1i, a0, b0 }, unotOutline);

            DrawEquation(sel0, reps, repDr.DomainSeg.StartPoint + new SKPoint(500, -200), _pens.TextBrush);
            DrawAreaValues(sel0, reps);
        }

        private void DrawTriangle(bool isPositive, SKPaint color, bool isUnit, params SKPoint[] points)
        {
	        _renderer.DrawPath(points, color);
	        if (!isPositive)
	        {
		        _renderer.DrawPath(points, isUnit ? _pens.BackHatch : _pens.ForeHatch);
            }
        }

        private void DrawEquation(Number sel, Number rep, SKPoint location, SKPaint paint)
        {
	        var selTxt = $"({sel.StartValue:0.0}i → {sel.EndValue:0.0})";
            var repTxt = $"({rep.StartValue:0.0}i → {rep.EndValue:0.0})";
            var result = sel.Value * rep.Value;
            var resultTxt = $"({result.Imaginary:0.0}i → {result.Real:0.0})";
            var areaTxt = $"area:  {result.Imaginary + result.Real:0.0}";

            Canvas.DrawText(selTxt, location.X, location.Y, _pens.Seg0TextBrush);
            Canvas.DrawText(repTxt, location.X, location.Y + 30, _pens.Seg1TextBrush);
            Canvas.DrawLine(location.X, location.Y+38, location.X + 100, location.Y+38, _pens.GrayPen);
            Canvas.DrawText(resultTxt, location.X, location.Y + 60, _pens.TextBrush);
            Canvas.DrawText(areaTxt, location.X, location.Y + 95, unitText);
        }

        private void DrawAreaValues(Number sel, Number rep, bool unitPerspective = true)
        {
	        var selSeg = _renderer.DomainRenderers[sel.Domain.Id].DomainSeg;
	        var repSeg = _renderer.DomainRenderers[rep.Domain.Id].DomainSeg;

	        var aaTxt = $"{sel.EndValue * rep.EndValue:0.0}";
	        Canvas.DrawText(aaTxt, selSeg.EndPoint.X, repSeg.EndPoint.Y, unitText);

	        var abTxt = $"{sel.StartValue * rep.EndValue:0.0}";
	        Canvas.DrawText(abTxt, selSeg.StartPoint.X, repSeg.EndPoint.Y, unitText);

            var baTxt = $"{sel.EndValue * rep.StartValue:0.0}";
            Canvas.DrawText(baTxt, selSeg.EndPoint.X, repSeg.StartPoint.Y, unitText);

            var bbTxt = $"{sel.StartValue * -rep.StartValue:0.0}";
            Canvas.DrawText(bbTxt, selSeg.StartPoint.X, repSeg.StartPoint.Y, unitText);
        }






        private static readonly SKPaint unitBB_Pen = CorePens.GetBrush(SKColor.Parse("#80F87A0E"));
        private static readonly SKPaint unitAB_Pen = CorePens.GetBrush(SKColor.Parse("#80C8690B"));
        private static readonly SKPaint unitBA_Pen = CorePens.GetBrush(SKColor.Parse("#80FBB968"));
        private static readonly SKPaint unitAA_Pen = CorePens.GetBrush(SKColor.Parse("#80C93B0C"));
        private static readonly SKPaint unitOutline = CorePens.GetPen(SKColor.Parse("#A0200000"), 4);

        private static readonly SKPaint unotBB_Pen = CorePens.GetBrush(SKColor.Parse("#4000BCFD"));
        private static readonly SKPaint unotAB_Pen = CorePens.GetBrush(SKColor.Parse("#40177B9E"));
        private static readonly SKPaint unotBA_Pen = CorePens.GetBrush(SKColor.Parse("#4091E2FD"));
        private static readonly SKPaint unotAA_Pen = CorePens.GetBrush(SKColor.Parse("#400060A9"));
        private static readonly SKPaint unotOutline = CorePens.GetPen(SKColor.Parse("#A0000020"), 4);

        private static readonly SKPaint unitText = CorePens.GetText(unitBB_Pen.Color, 18);
        private static readonly SKPaint unotText = CorePens.GetText(unitBB_Pen.Color, 18);
    }
}
