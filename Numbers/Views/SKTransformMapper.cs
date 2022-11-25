using System.Collections.Generic;
using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public class SKTransformMapper : SKMapper
	{
		public Transform Transform { get; set; }
		private SKCanvas Canvas => Renderer.Canvas;
		private CorePens Pens => Renderer.Pens;
		private List<SKPoint[]> Triangles { get; } = new List<SKPoint[]>();

		private SKDomainMapper SelectionMapper => WorkspaceMapper.DomainMapper(Transform.Selection[0].Domain.Id);
		private SKDomainMapper RepeatMapper => WorkspaceMapper.DomainMapper(Transform.Repeat.Domain.Id);

        //public SKDomainMapper SelectionMapper => 
        public override SKPoint StartPoint
        {
	        get { return SKPoint.Empty; }
	        set {  }
        }
        public override SKPoint MidPoint => SKPoint.Empty;
        public override SKPoint EndPoint
        {
	        get { return SKPoint.Empty; }
	        set {}
        }
        private SKPoint SKOrigin => SelectionMapper.BasisSegment.StartPoint;

        public SKTransformMapper(Workspace workspace, Transform transform) : base(workspace, transform)
        {
	        Transform = transform;
        }

        private Range _selRange;
        private Range _repRange;
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

			_selRange = selNum.RangeInMinMax;
			_repRange = repNum.RangeInMinMax;
			r0_s1 = repNum.StartValue * selNum.EndValue;
			r1_s0 = repNum.EndValue * selNum.StartValue;
			r0_s0 = -repNum.StartValue * selNum.StartValue;
			r1_s1 = repNum.EndValue * selNum.EndValue;

			var org = selDr.DisplayLine.PointAlongLine(0.5f);

			var s0Unit = selDr.DisplayLine.PointAlongLine(_selRange.StartF);
			var s1Unit = selDr.DisplayLine.PointAlongLine(_selRange.EndF);
			var r0Unit = repDr.DisplayLine.PointAlongLine(_repRange.StartF);
			var r1Unit = repDr.DisplayLine.PointAlongLine(_repRange.EndF);

			var s0Unot = selDr.DisplayLine.PointAlongLine(1f - _selRange.StartF);
			var s1Unot = selDr.DisplayLine.PointAlongLine(1f - _selRange.EndF);
			var r0Unot = repDr.DisplayLine.PointAlongLine(1f - _repRange.StartF);
			var r1Unot = repDr.DisplayLine.PointAlongLine(1f - _repRange.EndF);


            DrawTriangle(r0_s1 >= 0, unotBA_Brush, false, r0Unot, s1Unit, org);
            DrawTriangle(r1_s0 >= 0, unotAB_Brush, false, r1Unot, s0Unit, org);
            DrawTriangle(r0_s0 >= 0, unitAA_Brush, true, s0Unit, r0Unit, org);
            DrawTriangle(r1_s1 >= 0, unitBB_Brush, true, s1Unit, r1Unit, org);

            DrawTriangle(r0_s1 >= 0, unotBA_Pen, false, r1Unit, s0Unot, org);
            DrawTriangle(r1_s0 >= 0, unotAB_Pen, false, r0Unit, s1Unot, org);
            DrawTriangle(r0_s0 >= 0, unitAA_Pen, true, s0Unot, r0Unot, org);
            DrawTriangle(r1_s1 >= 0, unitBB_Pen, true, s1Unot, r1Unot, org);

            DrawEquation(selNum, repNum, new SKPoint(900, 500), Pens.TextBrush);
			DrawAreaValues(selNum, repNum);
			//DrawUnitBox(GetUnitBoxPoints(), unitRect_Pen);
			//DrawXFormedUnitBox(GetUnitBoxPoints(), unitXformRect_Pen);
		}

		public override SKPath GetHighlightAt(Highlight highlight)
		{
			return new SKPath(); // todo: add line in focused triangle
		}

        private void DrawTriangle(bool isPositive, SKPaint color, bool isUnit, params SKPoint[] points)
		{
            Triangles.Add(points);
			Renderer.FillPolyline(color, points);
			if (!isPositive && !color.IsStroke)
			{
				Renderer.FillPolyline(isUnit ? Pens.BackHatch : Pens.ForeHatch, points);
			}
		}

		private void DrawEquation(Number sel, Number rep, SKPoint location, SKPaint paint)
		{
			var selTxt = $"({sel.StartValue:0.00}i → {sel.EndValue:0.00})";
			var repTxt = $"({rep.StartValue:0.00}i → {rep.EndValue:0.00})";
			var result = sel.Value * rep.Value;
			var resultTxt = $"({result.Start:0.00}i → {result.End:0.00})";
			var areaTxt = $"area:  {result.Start + result.End:0.00}";

			Canvas.DrawText(selTxt, location.X, location.Y, Pens.Seg0TextBrush);
			Canvas.DrawText(repTxt, location.X, location.Y + 30, Pens.Seg1TextBrush);
			Canvas.DrawLine(location.X, location.Y + 38, location.X + 100, location.Y + 38, Pens.GrayPen);
			Canvas.DrawText(resultTxt, location.X, location.Y + 60, Pens.TextBrush);
			Canvas.DrawText(areaTxt, location.X, location.Y + 95, unitText);
		}

		public void DrawTextOnSegment(string txt, SKPoint startPt, SKPoint endPt, SKPaint paint)
		{
			bool isUnot = txt.EndsWith("i");
			var ordered = new List<SKPoint>() {startPt, endPt};
			if (startPt.X > endPt.X)
			{
				ordered.Reverse();
			}

			var offset = startPt.Y < endPt.Y ? new SKPoint(0, 18) : new SKPoint(0, -5);
            var seg = new SKSegment(ordered[0], ordered[1]);
            var seg2 = isUnot ? 
	            new SKSegment(seg.PointAlongLine(0.5f), seg.PointAlongLine(0.9f)) : 
	            new SKSegment(seg.PointAlongLine(0.2f), seg.PointAlongLine(0.6f));
            var p = new SKPath();
            p.AddPoly(new SKPoint[] { seg2.StartPoint, seg2.EndPoint }, false);
            Canvas.DrawTextOnPath(txt, p, offset, paint);
        }
		private void DrawAreaValues(Number selNum, Number repNum, bool unitPerspective = true)
		{
            var selSeg = SelectionMapper.DisplayLine;
			var repSeg = RepeatMapper.DisplayLine;

			var r0_s1Txt = $"{r0_s1:0.0}i";
			var r1_s0Txt = $"{r1_s0:0.0}i";
			var r0_s0Txt = $"{r0_s0:0.0}";
            var r1_s1Txt = $"{r1_s1:0.0}";

            DrawTextOnSegment(r0_s1Txt, selSeg.PointAlongLine(_selRange.EndF), repSeg.PointAlongLine(1 - _repRange.StartF), unotText);
            DrawTextOnSegment(r1_s0Txt, selSeg.PointAlongLine(_selRange.StartF), repSeg.PointAlongLine(1 - _repRange.EndF), unotText);
			DrawTextOnSegment(r0_s0Txt, selSeg.PointAlongLine(_selRange.StartF), repSeg.PointAlongLine(_repRange.StartF), unitText);
			DrawTextOnSegment(r1_s1Txt, selSeg.PointAlongLine(_selRange.EndF), repSeg.PointAlongLine(_repRange.EndF), unitText);

            var total = r0_s1 + r1_s0 + r0_s0 + r1_s1;
			Canvas.DrawText($"area: {total:0.0}", 30, 50, Pens.TextBrush);
        }

		private SKPoint[] GetUnitBoxPoints() // cw from org
		{
			var result = new SKPoint[4];
			var rs = RepeatMapper.BasisSegment;
			var ss = SelectionMapper.BasisSegment;
			result[0] = ss.StartPoint; // Org
            result[1] = rs.EndPoint; // TL
            result[2] = new SKPoint(ss.EndPoint.X, rs.EndPoint.Y); // TR
            result[3] = ss.EndPoint; // BR
            return result;
		}
		private void DrawUnitBox(SKPoint[] cwPts, SKPaint pen)
		{
			var left = new SKSegment(cwPts[0], cwPts[1]);
			var top = new SKSegment(cwPts[1], cwPts[2]);
			var right = new SKSegment(cwPts[3], cwPts[2]);
			var bottom = new SKSegment(cwPts[0], cwPts[3]);
			var inset = 5;
			var rv = left.InsetSegment(inset);
			var sh = top.InsetSegment(inset);
			var rh = right .InsetSegment(inset);
			var sv = bottom.InsetSegment(inset);
            Renderer.DrawDirectedLine(rv, true, pen);
			Renderer.DrawDirectedLine(sh, true, pen);
			Renderer.DrawDirectedLine(rh, true, pen);
			Renderer.DrawDirectedLine(sv, true, pen);
		}

		private void DrawXFormedUnitBox(SKPoint[] cwPts, SKPaint pen)
		{
			TransformPoints(cwPts);
			DrawUnitBox(cwPts, pen);
		}

		private void TransformPoints(SKPoint[] pts)
		{
			var org = SKOrigin;
			var unitLen = RepeatMapper.BasisSegment.Length;

			var rNum = Transform.Repeat.Value;
            var sNum = Transform.Selection[0].Value;
            var prod = rNum * sNum;

            var ri = (float)rNum.Start;
			var ru = (float)rNum.End;
			var si = (float)sNum.Start;
			var su = (float)sNum.End;
			var prodI = (float)prod.Start;
			var prodU = (float)prod.End;
            SKPoint pt = SKPoint.Empty;
			float x, y = 0;

			pt = pts[0]; // x of s0 y of r0
			pt -= org;
			x = -unitLen * si;
			y = unitLen * ri;
			pts[0] = new SKPoint(x + org.X, y + org.Y);

			pt = pts[1]; // x of s01 y of r01
			pt -= org;
			//x = -(unitLen * su - unitLen);
			//y = pt.Y * ri;
			x = ((pt.X - unitLen) * prodU) / 2f;
			y = pt.Y * ri;
            pts[1] = new SKPoint(x + org.X, y + org.Y);

			pt = pts[2]; // x of s1 y of r1
			pt -= org;
			x = pt.X * su;
			y = pt.Y * ru;
			pts[2] = new SKPoint(x + org.X, y + org.Y);

			pt = pts[3]; // x of s0 y of r10
			pt -= org;
			//x = pt.X * si - unitLen;
			//y = (pt.Y + unitLen) * ri;
			x = ((pt.X + unitLen) * prodU) / 2f;
			y = (pt.Y + unitLen) * ri;
            pts[3] = new SKPoint(x + org.X, y + org.Y);



            //         var 
            //for (int i = 0; i < pts.Length; i++)
            //{
            //	var pt = pts[i];
            //	pt -= org;
            //	var x = pt.X * ru + pt.X * ri;
            //	var y = pt.Y * su + pt.Y * si;
            //             pts[i] = new SKPoint(x + org.X, y+ org.Y);
            //}
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


		private static readonly SKPaint unitRect_Pen = CorePens.GetPen(SKColors.Wheat, 0.5f);
		private static readonly SKPaint unitXformRect_Pen = CorePens.GetPen(SKColors.White, 1f);
    }
}