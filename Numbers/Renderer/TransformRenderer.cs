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
	        var selx = _transform.Selection[0];
	        var repy = _transform.Repeats;
	        var selRatio = selx.Ratio;
	        var repRatio = repy.Ratio;
	        var selDr = _renderer.DomainRenderers[selx.Domain.Id];
	        var repDr = _renderer.DomainRenderers[repy.Domain.Id];

	        var org = selDr.DomainSeg.PointAlongLine(0.5f);

	        var a0DirOk = selx.StartValue >= 0;
	        var a1DirOk = selx.EndValue >= 0;
	        var b0DirOk = repy.StartValue >= 0;
	        var b1DirOk = repy.EndValue >= 0;

            var a0Unit = selDr.DomainSeg.PointAlongLine(a0DirOk ? selRatio.Start : 1f - selRatio.Start);
	        var a1Unit = selDr.DomainSeg.PointAlongLine(a1DirOk ? selRatio.End : 1f - selRatio.End);
	        var b0Unit = repDr.DomainSeg.PointAlongLine(b0DirOk ? repRatio.Start : 1f - repRatio.Start);
	        var b1Unit = repDr.DomainSeg.PointAlongLine(b1DirOk ? repRatio.End : 1f - repRatio.End);

	        var a0Unot = selDr.DomainSeg.PointAlongLine(!a0DirOk ? selRatio.Start : 1f - selRatio.Start);
	        var a1Unot = selDr.DomainSeg.PointAlongLine(!a1DirOk ? selRatio.End : 1f - selRatio.End);
	        var b0Unot = repDr.DomainSeg.PointAlongLine(!b0DirOk ? repRatio.Start : 1f - repRatio.Start);
	        var b1Unot = repDr.DomainSeg.PointAlongLine(!b1DirOk ? repRatio.End : 1f - repRatio.End);


	        var aaPos = selx.EndValue * repy.EndValue >= 0;
	        var abPos = selx.StartValue * repy.EndValue >= 0;
	        var baPos = selx.EndValue * repy.StartValue >= 0;
	        var bbPos = selx.StartValue * repy.StartValue >= 0;

            DrawTriangle(aaPos, unotBB_Pen, false, a1Unot, b1Unot, org); // +
            DrawTriangle(abPos, unotAB_Pen, false, a0Unot, b1Unot, org); // +
            DrawTriangle(baPos, unotBA_Pen, false, a1Unot, b0Unot, org); // +
            DrawTriangle(!bbPos, unotAA_Pen, false, a0Unot, b0Unot, org);  // -
            _renderer.DrawPath(new SKPoint[] { a1Unot, b1Unot, a0Unot, b0Unot }, unotOutline);

			DrawTriangle(aaPos, unitBB_Pen, true, a1Unit, b1Unit, org); // +
			DrawTriangle(abPos, unitAB_Pen, true, a0Unit, b1Unit, org); // +
			DrawTriangle(baPos, unitBA_Pen, true, a1Unit, b0Unit, org); // +
			DrawTriangle(!bbPos, unitAA_Pen, true, a0Unit, b0Unit, org);  // -
			_renderer.DrawPath(new SKPoint[] { a1Unit, b1Unit, a0Unit, b0Unit }, unitOutline);


            DrawEquation(selx, repy, repDr.DomainSeg.StartPoint + new SKPoint(500, -200), _pens.TextBrush);
            DrawAreaValues(selx, repy);
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






        private static readonly SKPaint unitBB_Pen = CorePens.GetBrush(SKColor.Parse("#70F87A0E"));
        private static readonly SKPaint unitAB_Pen = CorePens.GetBrush(SKColor.Parse("#70C8690B"));
        private static readonly SKPaint unitBA_Pen = CorePens.GetBrush(SKColor.Parse("#70FBB968"));
        private static readonly SKPaint unitAA_Pen = CorePens.GetBrush(SKColor.Parse("#70C93B0C"));
        private static readonly SKPaint unitOutline = CorePens.GetPen(SKColor.Parse("#A0200000"), 4);

        private static readonly SKPaint unotBB_Pen = CorePens.GetBrush(SKColor.Parse("#9000BCFD"));
        private static readonly SKPaint unotAB_Pen = CorePens.GetBrush(SKColor.Parse("#90177B9E"));
        private static readonly SKPaint unotBA_Pen = CorePens.GetBrush(SKColor.Parse("#9091E2FD"));
        private static readonly SKPaint unotAA_Pen = CorePens.GetBrush(SKColor.Parse("#900060A9"));
        private static readonly SKPaint unotOutline = CorePens.GetPen(SKColor.Parse("#A0000020"), 4);

        private static readonly SKPaint unitText = CorePens.GetText(unitBB_Pen.Color, 18);
        private static readonly SKPaint unotText = CorePens.GetText(unitBB_Pen.Color, 18);
    }
}
