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

        private RatioSeg selRatio;
        private RatioSeg repRatio;
        private double r0_s1;
        private double r1_s0;
        private double r0_s0;
        private double r1_s1;

        public void Draw()
		{
            Triangles.Clear();
			var selNum = Transform.Selection[0];
			var repNum = Transform.Repeat;
			var selDr = SelectionMapper;
			var repDr = RepeatMapper;

			selRatio = selNum.Ratio;
			repRatio = repNum.Ratio;
			r0_s1 = repNum.StartValue * selNum.EndValue;
			r1_s0 = repNum.EndValue * selNum.StartValue;
			r0_s0 = -repNum.StartValue * selNum.StartValue;
			r1_s1 = repNum.EndValue * selNum.EndValue;

			var org = selDr.DomainSegment.PointAlongLine(0.5f);

			var s0Unit = selDr.DomainSegment.PointAlongLine(selRatio.Start);
			var s1Unit = selDr.DomainSegment.PointAlongLine(selRatio.End);
			var r0Unit = repDr.DomainSegment.PointAlongLine(repRatio.Start);
			var r1Unit = repDr.DomainSegment.PointAlongLine(repRatio.End);

			var s0Unot = selDr.DomainSegment.PointAlongLine(1f - selRatio.Start);
			var s1Unot = selDr.DomainSegment.PointAlongLine(1f - selRatio.End);
			var r0Unot = repDr.DomainSegment.PointAlongLine(1f - repRatio.Start);
			var r1Unot = repDr.DomainSegment.PointAlongLine(1f - repRatio.End);


            DrawTriangle(r0_s1 >= 0, unotBA_Brush, false, r0Unot, s1Unit, org);
            DrawTriangle(r1_s0 >= 0, unotAB_Brush, false, r1Unot, s0Unit, org);
            DrawTriangle(r0_s0 >= 0, unitAA_Brush, true, s0Unit, r0Unit, org);
            DrawTriangle(r1_s1 >= 0, unitBB_Brush, true, s1Unit, r1Unit, org);

            DrawTriangle(r0_s1 >= 0, unotBA_Pen, false, r1Unit, s0Unot, org);
            DrawTriangle(r1_s0 >= 0, unotAB_Pen, false, r0Unit, s1Unot, org);
            DrawTriangle(r0_s0 >= 0, unitAA_Pen, true, s0Unot, r0Unot, org);
            DrawTriangle(r1_s1 >= 0, unitBB_Pen, true, s1Unot, r1Unot, org);

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
			if (!isPositive && !color.IsStroke)
			{
				Renderer.DrawShape(points, isUnit ? Pens.BackHatch : Pens.ForeHatch);
			}
		}

		private void DrawEquation(Number sel, Number rep, SKPoint location, SKPaint paint)
		{
			var selTxt = $"({sel.StartValue:0.00}i → {sel.EndValue:0.00})";
			var repTxt = $"({rep.StartValue:0.00}i → {rep.EndValue:0.00})";
			var result = sel.Value * rep.Value;
			var resultTxt = $"({result.Imaginary:0.00}i → {result.Real:0.00})";
			var areaTxt = $"area:  {result.Imaginary + result.Real:0.00}";

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
		private void DrawAreaValues(Number selNum, Number repNum, bool unitPerspective = true)
		{
            var selSeg = SelectionMapper.DomainSegment;
			var repSeg = RepeatMapper.DomainSegment;

			var r0_s1Txt = $"{r0_s1:0.0}";
			var r1_s0Txt = $"{r1_s0:0.0}";
			var r0_s0Txt = $"{r0_s0:0.0}";
            var r1_s1Txt = $"{r1_s1:0.0}";

            DrawTextOnSegment(r0_s1Txt, selSeg.PointAlongLine(selRatio.End), repSeg.PointAlongLine(1 - repRatio.Start), unotText);
            DrawTextOnSegment(r1_s0Txt, selSeg.PointAlongLine(selRatio.Start), repSeg.PointAlongLine(1 - repRatio.End), unotText);
			DrawTextOnSegment(r0_s0Txt, selSeg.PointAlongLine(selRatio.Start), repSeg.PointAlongLine(repRatio.Start), unitText);
			DrawTextOnSegment(r1_s1Txt, selSeg.PointAlongLine(selRatio.End), repSeg.PointAlongLine(repRatio.End), unitText);

            var total = r0_s1 + r1_s0 + r0_s0 + r1_s1;
			Canvas.DrawText($"{total:0.0}", selSeg.StartPoint.X, repSeg.EndPoint.Y, Pens.TextBrush);
        }

		private static SKColor unitAA_Color = SKColor.Parse("#50C93B0C");
		private static SKColor unitBB_Color = SKColor.Parse("#50F87A0E");
		private static SKColor unotAB_Color = SKColor.Parse("#700060A9");
		private static SKColor unotBA_Color = SKColor.Parse("#7000BCFD");
		private static readonly SKPaint unitAA_Brush = CorePens.GetBrush(unitAA_Color);
        private static readonly SKPaint unitBB_Brush = CorePens.GetBrush(unitBB_Color);
        private static readonly SKPaint unotAB_Brush = CorePens.GetBrush(unotAB_Color);
		private static readonly SKPaint unotBA_Brush = CorePens.GetBrush(unotBA_Color);


		private static readonly SKPaint unitBB_Pen = CorePens.GetPen(unitBB_Color, 2);
		private static readonly SKPaint unitAA_Pen = CorePens.GetPen(unitAA_Color, 2);
		private static readonly SKPaint unotAB_Pen = CorePens.GetPen(unotAB_Color,1.5f);
		private static readonly SKPaint unotBA_Pen = CorePens.GetPen(unotBA_Color, 1.5f);

		private static readonly SKPaint unitText = CorePens.GetText(SKColor.Parse("#A0F87A0E"), 18);
		private static readonly SKPaint unotText = CorePens.GetText(SKColor.Parse("#B000D0FF"), 18);
	}
}