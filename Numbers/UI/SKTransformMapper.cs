using Numbers.Core;
using Numbers.Renderer;
using SkiaSharp;

namespace Numbers.UI
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public class SKTransformMapper : SKMapper
	{
		public Transform Transform { get; set; }
		private SKCanvas Canvas => Renderer.Canvas;
		private CorePens Pens => Renderer.Pens;
		private List<SKPoint[]> Triangles { get; } = new List<SKPoint[]>();

		private SKDomainMapper SelectionMapper => Renderer.DomainMapper(Transform.Selection[0].Domain.Id);
		private SKDomainMapper RepeatMapper => Renderer.DomainMapper(Transform.Repeat.Domain.Id);

        //public SKDomainMapper SelectionMapper => 
        public override SKPoint StartPoint => SKPoint.Empty;//NumberSeg.StartPoint;
        public override SKPoint MidPoint => SKPoint.Empty;//NumberSegment.Midpoint;
        public override SKPoint EndPoint => SKPoint.Empty;//NumberSegment.EndPoint;

        public SKTransformMapper(CoreRenderer renderer, Transform transform) : base(renderer, transform)
        {
	        Transform = transform;
        }

		public void Draw()
		{
            Triangles.Clear();
			var selNum = Transform.Selection[0];
			var repNum = Transform.Repeat;
			var selRatio = selNum.Ratio;
			var repRatio = repNum.Ratio;
			var selDr = SelectionMapper;
			var repDr = RepeatMapper;

			var org = selDr.DomainSegment.PointAlongLine(0.5f);

			var a0DirOk = selNum.StartValue >= 0;
			var a1DirOk = selNum.EndValue >= 0;
			var b0DirOk = repNum.StartValue >= 0;
			var b1DirOk = repNum.EndValue >= 0;

			var a0Unit = selDr.DomainSegment.PointAlongLine(a0DirOk ? selRatio.Start : 1f - selRatio.Start);
			var a1Unit = selDr.DomainSegment.PointAlongLine(a1DirOk ? selRatio.End : 1f - selRatio.End);
			var b0Unit = repDr.DomainSegment.PointAlongLine(b0DirOk ? repRatio.Start : 1f - repRatio.Start);
			var b1Unit = repDr.DomainSegment.PointAlongLine(b1DirOk ? repRatio.End : 1f - repRatio.End);

			var a0Unot = selDr.DomainSegment.PointAlongLine(!a0DirOk ? selRatio.Start : 1f - selRatio.Start);
			var a1Unot = selDr.DomainSegment.PointAlongLine(!a1DirOk ? selRatio.End : 1f - selRatio.End);
			var b0Unot = repDr.DomainSegment.PointAlongLine(!b0DirOk ? repRatio.Start : 1f - repRatio.Start);
			var b1Unot = repDr.DomainSegment.PointAlongLine(!b1DirOk ? repRatio.End : 1f - repRatio.End);


			var aaPos = selNum.EndValue * repNum.EndValue >= 0;
			var abPos = selNum.StartValue * repNum.EndValue >= 0;
			var baPos = selNum.EndValue * repNum.StartValue >= 0;
			var bbPos = selNum.StartValue * repNum.StartValue >= 0;

			DrawTriangle(aaPos, unotBB_Pen, false, a1Unot, b1Unot, org); // +
			DrawTriangle(abPos, unotAB_Pen, false, a0Unot, b1Unot, org); // +
			DrawTriangle(baPos, unotBA_Pen, false, a1Unot, b0Unot, org); // +
			DrawTriangle(!bbPos, unotAA_Pen, false, a0Unot, b0Unot, org); // -
			Renderer.DrawShape(new SKPoint[] {a1Unot, b1Unot, a0Unot, b0Unot}, unotOutline);

			DrawTriangle(aaPos, unitBB_Pen, true, a1Unit, b1Unit, org); // +
			DrawTriangle(abPos, unitAB_Pen, true, a0Unit, b1Unit, org); // +
			DrawTriangle(baPos, unitBA_Pen, true, a1Unit, b0Unit, org); // +
			DrawTriangle(!bbPos, unitAA_Pen, true, a0Unit, b0Unit, org); // -
			Renderer.DrawShape(new SKPoint[] {a1Unit, b1Unit, a0Unit, b0Unit}, unitOutline);


			DrawEquation(selNum, repNum, repDr.DomainSegment.StartPoint + new SKPoint(500, -200), Pens.TextBrush);
			DrawAreaValues(selNum, repNum);
		}

		private void DrawTriangle(bool isPositive, SKPaint color, bool isUnit, params SKPoint[] points)
		{
            Triangles.Add(points);
			Renderer.DrawShape(points, color);
			if (!isPositive)
			{
				Renderer.DrawShape(points, isUnit ? Pens.BackHatch : Pens.ForeHatch);
			}
		}

		private void DrawEquation(Number sel, Number rep, SKPoint location, SKPaint paint)
		{
			var selTxt = $"({sel.StartValue:0.0}i → {sel.EndValue:0.0})";
			var repTxt = $"({rep.StartValue:0.0}i → {rep.EndValue:0.0})";
			var result = sel.Value * rep.Value;
			var resultTxt = $"({result.Imaginary:0.0}i → {result.Real:0.0})";
			var areaTxt = $"area:  {result.Imaginary + result.Real:0.0}";

			Canvas.DrawText(selTxt, location.X, location.Y, Pens.Seg0TextBrush);
			Canvas.DrawText(repTxt, location.X, location.Y + 30, Pens.Seg1TextBrush);
			Canvas.DrawLine(location.X, location.Y + 38, location.X + 100, location.Y + 38, Pens.GrayPen);
			Canvas.DrawText(resultTxt, location.X, location.Y + 60, Pens.TextBrush);
			Canvas.DrawText(areaTxt, location.X, location.Y + 95, unitText);
		}

		private void DrawAreaValues(Number sel, Number rep, bool unitPerspective = true)
		{
            var selSeg = SelectionMapper.DomainSegment;
			var repSeg = RepeatMapper.DomainSegment;

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