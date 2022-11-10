using System.Net;
using System.Windows.Forms;
using Numbers.Core;
using Numbers.Mind;
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

		private SKDomainMapper SelectionMapper => Workspace.DomainMapper(Transform.Selection[0].Domain.Id);
		private SKDomainMapper RepeatMapper => Workspace.DomainMapper(Transform.Repeat.Domain.Id);

        //public SKDomainMapper SelectionMapper => 
        public override SKPoint StartPoint => SKPoint.Empty;//NumberSeg.StartPoint;
        public override SKPoint MidPoint => SKPoint.Empty;//NumberSegment.Midpoint;
        public override SKPoint EndPoint => SKPoint.Empty;//NumberSegment.EndPoint;

        public SKTransformMapper(Workspace workspace, Transform transform) : base(workspace, transform)
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

			var a0Unit = selDr.DomainSegment.PointAlongLine(selRatio.Start);
			var a1Unit = selDr.DomainSegment.PointAlongLine(selRatio.End);
			var b0Unit = repDr.DomainSegment.PointAlongLine(repRatio.Start);
			var b1Unit = repDr.DomainSegment.PointAlongLine(repRatio.End);

			var a0Unot = selDr.DomainSegment.PointAlongLine(1f - selRatio.Start);
			var a1Unot = selDr.DomainSegment.PointAlongLine(1f - selRatio.End);
			var b0Unot = repDr.DomainSegment.PointAlongLine(1f - repRatio.Start);
			var b1Unot = repDr.DomainSegment.PointAlongLine(1f - repRatio.End);


            var aaPos = selNum.EndValue * repNum.EndValue >= 0;
			var abPos = selNum.StartValue * repNum.EndValue >= 0;
			var baPos = selNum.EndValue * repNum.StartValue >= 0;
			var bbPos = -selNum.StartValue * repNum.StartValue >= 0;

			DrawTriangle(bbPos, unitAA_Brush, true, a0Unit, b0Unit, org);
            DrawTriangle(aaPos, unitBB_Brush, true, a1Unit, b1Unit, org);
            DrawTriangle(baPos, unotBA_Brush, true, b0Unot, a1Unit, org);
            DrawTriangle(aaPos, unotAB_Brush, true, b1Unot, a0Unit, org);

            DrawTriangle(bbPos, unitAA_Pen, true, b0Unot, a0Unot, org);
            DrawTriangle(aaPos, unitBB_Pen, true, b1Unot, a1Unot, org);
            DrawTriangle(baPos, unotBA_Pen, true, a1Unot, b0Unit, org);
            DrawTriangle(aaPos, unotAB_Pen, true, a0Unot, b1Unit, org);

            DrawEquation(selNum, repNum, repDr.DomainSegment.StartPoint + new SKPoint(500, -200), Pens.TextBrush);
			DrawAreaValues(selNum, repNum);
		}

		public override SKPath HighlightAt(float t, SKPoint targetPoint)
		{
			return new SKPath(); // todo: add line in focused triangle
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

		public void DrawTextOnSegment(string txt, SKPoint startPt, SKPoint endPt, SKPaint paint)
		{
			var ordered = new List<SKPoint>() {startPt, endPt};
			if (startPt.X > endPt.X)
			{
				ordered.Reverse();
			}

			var offset = startPt.Y < endPt.Y ? new SKPoint(0, 18) : new SKPoint(0, -5);
            var seg = new SKSegment(ordered[0], ordered[1]);
            var seg2 = new SKSegment(seg.PointAlongLine(0.3f), seg.PointAlongLine(0.7f));
            var p = new SKPath();
            p.AddPoly(new SKPoint[] { seg2.StartPoint, seg2.EndPoint }, false);
            Canvas.DrawTextOnPath(txt, p, offset, paint);
        }
		private void DrawAreaValues(Number sel, Number rep, bool unitPerspective = true)
		{
            var selSeg = SelectionMapper.DomainSegment;
			var repSeg = RepeatMapper.DomainSegment;

			var aa = sel.EndValue * rep.EndValue;
			var ab = sel.StartValue * rep.EndValue;
			var ba = sel.EndValue * rep.StartValue;
			var bb = sel.StartValue * -rep.StartValue;

            var aaTxt = $"{aa:0.0}";
			var abTxt = $"{ab:0.0}";
			var baTxt = $"{ba:0.0}";
			var bbTxt = $"{bb:0.0}";

			var selRat = sel.Ratio;
			var repRat = rep.Ratio;
			DrawTextOnSegment(aaTxt, selSeg.PointAlongLine(selRat.End), repSeg.PointAlongLine(repRat.End), unitText);
			DrawTextOnSegment(bbTxt, selSeg.PointAlongLine(selRat.Start), repSeg.PointAlongLine(repRat.Start), unitText);
            DrawTextOnSegment(baTxt, selSeg.PointAlongLine(selRat.End), repSeg.PointAlongLine(1 - repRat.Start), unotText);
            DrawTextOnSegment(abTxt, selSeg.PointAlongLine(selRat.Start), repSeg.PointAlongLine(1 - repRat.End), unotText);

            var total = aa + ab + ba + bb;
			Canvas.DrawText($"{total:0.0}", selSeg.StartPoint.X, repSeg.EndPoint.Y, Pens.TextBrush);
        }

		private static SKColor unitAA_Color = SKColor.Parse("#70C93B0C");
		private static SKColor unitBB_Color = SKColor.Parse("#70F87A0E");
		private static SKColor unotAB_Color = SKColor.Parse("#900060A9");
		private static SKColor unotBA_Color = SKColor.Parse("#9000BCFD");
		private static readonly SKPaint unitAA_Brush = CorePens.GetBrush(unitAA_Color);
        private static readonly SKPaint unitBB_Brush = CorePens.GetBrush(unitBB_Color);
        private static readonly SKPaint unotAB_Brush = CorePens.GetBrush(unotAB_Color);
		private static readonly SKPaint unotBA_Brush = CorePens.GetBrush(unotBA_Color);


		private static readonly SKPaint unitBB_Pen = CorePens.GetPen(unitBB_Color, 2);
		private static readonly SKPaint unitAA_Pen = CorePens.GetPen(unitAA_Color, 2);
		private static readonly SKPaint unotAB_Pen = CorePens.GetPen(unotAB_Color,1.5f);
		private static readonly SKPaint unotBA_Pen = CorePens.GetPen(unotBA_Color, 1.5f);

		private static readonly SKPaint unitText = CorePens.GetText(unitAA_Color, 18);
		private static readonly SKPaint unotText = CorePens.GetText(unotAB_Color, 18);
	}
}