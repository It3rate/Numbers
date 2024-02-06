using System;
using System.Collections.Generic;
using System.Drawing;
using Numbers.Agent;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using SkiaSharp;

namespace Numbers.Mappers
{
	public class SKTransformMapper : SKMapper
    {
        CoreRenderer _renderer = CoreRenderer.Instance;
        public Transform Transform => (Transform)MathElement;
        private List<SKPoint[]> PolyShapes { get; } = new List<SKPoint[]>();

		private SKDomainMapper SelectionMapper => WorkspaceMapper.DomainMapper(Transform.Source[0].Domain);
		private SKDomainMapper RepeatMapper => WorkspaceMapper.DomainMapper(Transform.Change.Domain);

        private SKPoint SKOrigin => SelectionMapper.BasisSegment.StartPoint;

        public SKTransformMapper(MouseAgent agent, Transform transform) : base(agent, transform)
        {
        }

        private Range _selRange;
        private Range _repRange;
        private double ri_s;
        private double r_si;
        private double ri_si;
        private double r_s;

        private SKPoint[] r_s_shape;
        private SKPoint[] ri_si_shape;
        private SKPoint[] ri_s_shape;
        private SKPoint[] r_si_shape;

        public void Draw()
        {
            CalculateValue();
            if (DoRender)
            {
                DrawMultiply();
            }
        }
        private void CalculateValue()
        {
            var selNum = Transform.Source[0];
            var repNum = Transform.Change;
            Transform.Value.SetWith(selNum);
            Transform.Value.Multiply(repNum);
        }
        public void DrawMultiply()
        {
            PolyShapes.Clear();
            var selNum = Transform.Source[0];
            var repNum = Transform.Change;

            var selDr = SelectionMapper;
            var repDr = RepeatMapper;

            _selRange = selNum.RangeInMinMax;
            _repRange = repNum.RangeInMinMax;
            r_s = repNum.EndValue * selNum.EndValue;
            ri_si = -repNum.StartValue * selNum.StartValue;
            ri_s = repNum.StartValue * selNum.EndValue;
            r_si = repNum.EndValue * selNum.StartValue;

            var org = selDr.Guideline.PointAlongLine(0.5f);
            var sUnit = selDr.Guideline.PointAlongLine(_selRange.EndF);
            var rUnit = repDr.Guideline.PointAlongLine(_repRange.EndF);
            var sUnot = selDr.Guideline.PointAlongLine(_selRange.StartF);
            var rUnot = repDr.Guideline.PointAlongLine(_repRange.StartF);

            r_s_shape = new SKPoint[] { rUnit, new SKPoint(sUnit.X, rUnit.Y), sUnit, org };
            DrawPolyshape(r_s >= 0, unitAA_Brush, true, r_s_shape);
            ri_si_shape = new SKPoint[] { rUnot, new SKPoint(sUnot.X, rUnot.Y), sUnot, org };
            DrawPolyshape(ri_si >= 0, unitBB_Brush, true, ri_si_shape);
            ri_s_shape = new SKPoint[] { rUnot, new SKPoint(sUnit.X, rUnot.Y), sUnit, org };
            DrawPolyshape(ri_s >= 0, unotBA_Brush, false, ri_s_shape);
            r_si_shape = new SKPoint[] { rUnit, new SKPoint(sUnot.X, rUnit.Y), sUnot, org };
            DrawPolyshape(r_si >= 0, unotBA_Brush, false, r_si_shape);

            // draw product boxes
            var ev = Transform.Value.EndValue;
            var esv = Math.Sqrt(Math.Abs(ev));
            var ep0 = selDr.PointAt(esv);
            var ep1 = repDr.PointAt(esv);
            var ePen = ev<0 ? unitNegPen : unitPosPen;
            DrawPolyshape(true, ePen, false, org, ep0, new SKPoint(ep0.X, ep1.Y), ep1);
            var etxt = $"{ev:0.00}";
            Canvas.DrawText(etxt, ep0.X - 30, ep1.Y - 4, Pens.UnitMarkerText);

            var iv = Transform.Value.StartValue;
            var isv = Math.Sqrt(Math.Abs(iv));
            var ip0 = selDr.PointAt(-isv);
            var ip1 = repDr.PointAt(isv);
            var iPen = iv < 0 ? unotNegPen : unotPosPen;
            DrawPolyshape(true, iPen, false, org, ip0, new SKPoint(ip0.X, ip1.Y), ip1);
            var itxt = $"{iv:0.00}i";
            Canvas.DrawText(itxt, ip0.X, ip1.Y - 4, Pens.UnotMarkerText); 


            DrawEquation(selNum, repNum, new SKPoint(200, 40), Pens.TextBrush);
            DrawAreaValues(selNum, repNum);
        }
        public void DrawX()
        {
            PolyShapes.Clear();
            var selNum = Transform.Source[0];
            var repNum = Transform.Change;
            var selDr = SelectionMapper;
            var repDr = RepeatMapper;

            _selRange = selNum.RangeInMinMax;
            _repRange = repNum.RangeInMinMax;
            ri_s = repNum.StartValue * selNum.EndValue;
            r_si = repNum.EndValue * selNum.StartValue;
            ri_si = -repNum.StartValue * selNum.StartValue;
            r_s = repNum.EndValue * selNum.EndValue;

            var org = selDr.Guideline.PointAlongLine(0.5f);

            var s0Unit = selDr.Guideline.PointAlongLine(_selRange.StartF);
            var s1Unit = selDr.Guideline.PointAlongLine(_selRange.EndF);
            var r0Unit = repDr.Guideline.PointAlongLine(_repRange.StartF);
            var r1Unit = repDr.Guideline.PointAlongLine(_repRange.EndF);

            var s0Unot = selDr.Guideline.PointAlongLine(1f - _selRange.StartF);
            var s1Unot = selDr.Guideline.PointAlongLine(1f - _selRange.EndF);
            var r0Unot = repDr.Guideline.PointAlongLine(1f - _repRange.StartF);
            var r1Unot = repDr.Guideline.PointAlongLine(1f - _repRange.EndF);


            DrawPolyshape(ri_s >= 0, unotBA_Brush, false, r0Unot, s1Unit, org);
            DrawPolyshape(r_si >= 0, unotAB_Brush, false, r1Unot, s0Unit, org);
            DrawPolyshape(ri_si >= 0, unitAA_Brush, true, s0Unit, r0Unit, org);
            DrawPolyshape(r_s >= 0, unitBB_Brush, true, s1Unit, r1Unit, org);

            DrawPolyshape(ri_s >= 0, unotPosPen, false, r1Unit, s0Unot, org);
            DrawPolyshape(r_si >= 0, unotNegPen, false, r0Unit, s1Unot, org);
            DrawPolyshape(ri_si >= 0, unitPosPen, true, s0Unot, r0Unot, org);
            DrawPolyshape(r_s >= 0, unitNegPen, true, s1Unot, r1Unot, org);

            DrawEquation(selNum, repNum, new SKPoint(900, 500), Pens.TextBrush);
            DrawAreaValues(selNum, repNum);
            //DrawUnitBox(GetUnitBoxPoints(), unitRect_Pen);
            //DrawXFormedUnitBox(GetUnitBoxPoints(), unitXformRect_Pen);
        }

        public override SKPath GetHighlightAt(Highlight highlight)
		{
			return new SKPath(); // todo: add line in focused triangle
		}
        private void DrawPolyshape(bool isPositive, SKPaint color, bool isUnit, params SKPoint[] points)
		{
            PolyShapes.Add(points);
			Renderer.FillPolyline(color, points);
			if (!isPositive && !color.IsStroke)
			{
				Renderer.FillPolyline(isUnit ? Pens.BackHatch : Pens.ForeHatch, points);
			}
		}
		private void DrawEquation(Number sel, Number rep, SKPoint location, SKPaint paint)
		{
			var selTxt = $"  ({sel.StartValue:0.00}i → {sel.EndValue:0.00})";
			var repTxt = $"* ({rep.StartValue:0.00}i → {rep.EndValue:0.00})";
			var result = sel.Value * rep.Value;
			var resultTxt = $"= ({result.Start:0.00}i → {result.End:0.00})";
			//var areaTxt = $"area:  {result.Start + result.End:0.00}";

			Canvas.DrawText(selTxt, location.X, location.Y, Pens.Seg0TextBrush);
			Canvas.DrawText(repTxt, location.X, location.Y + 30, Pens.Seg1TextBrush);
			Canvas.DrawLine(location.X, location.Y + 38, location.X + 160, location.Y + 40, Pens.TextFractionPen);
			Canvas.DrawText(resultTxt, location.X, location.Y + 65, Pens.TextBrush);
			//Canvas.DrawText(areaTxt, location.X, location.Y + 95, unitText);
		}
        public void DrawTextOnSegment(string txt, SKPoint startPt, SKPoint endPt, SKPaint paint, bool addBkg = false)
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

            var pt = seg.PointAlongLine(0.5f);
            var bkg = addBkg ? TextBkg : null;
            _renderer.DrawText(pt, txt, paint, bkg);

            //Canvas.DrawTextOnPath(txt, p, offset, paint);
        }
		private void DrawAreaValues(Number selNum, Number repNum, bool unitPerspective = true)
		{
            var selSeg = SelectionMapper.Guideline;
			var repSeg = RepeatMapper.Guideline;

            var txtOffset = 35;
            var c = r_s_shape.Center().Subtract(txtOffset, 0);
			DrawTextOnSegment($"{r_s:0.0}", c, c.Add(txtOffset*2, 0), blackText, true);
            c = ri_si_shape.Center().Subtract(txtOffset, 0);
			DrawTextOnSegment($"{ri_si:0.0}", c, c.Add(txtOffset * 2, 0), blackText, true);
            c = ri_s_shape.Center().Subtract(txtOffset, 0);
            DrawTextOnSegment($"{ri_s:0.0}i", c, c.Add(txtOffset * 2, 0), blackText, true);
            c = r_si_shape.Center().Subtract(txtOffset, 0);
            DrawTextOnSegment($"{r_si:0.0}i", c, c.Add(txtOffset * 2, 0), blackText, true);


   //         var total = ri_s + r_si + ri_si + r_s;
			//Canvas.DrawText($"area: {total:0.0}", 30, 50, Pens.TextBrush);
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
            Renderer.DrawDirectedLine(rv, pen);
			Renderer.DrawDirectedLine(sh, pen);
			Renderer.DrawDirectedLine(rh, pen);
			Renderer.DrawDirectedLine(sv, pen);
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

			var rNum = Transform.Change.Value;
            var sNum = Transform.Source[0].Value;
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
			x = ((pt.X + unitLen) * prodU) / 2f;
			y = (pt.Y + unitLen) * ri;
            pts[3] = new SKPoint(x + org.X, y + org.Y);
        }

		private static SKColor unitAA_Color = SKColor.Parse("#700060A9");
		private static SKColor unitBB_Color = SKColor.Parse("#50C93B0C");
		private static SKColor unotAB_Color = SKColor.Parse("#7000BCFD");
		private static SKColor unotBA_Color = SKColor.Parse("#7000E0BC");//"#50F87A0E");

        private static readonly SKPaint unitAA_Brush = CorePens.GetBrush(unitAA_Color);
        private static readonly SKPaint unitBB_Brush = CorePens.GetBrush(unitBB_Color);
        private static readonly SKPaint unotAB_Brush = CorePens.GetBrush(unotAB_Color);
		private static readonly SKPaint unotBA_Brush = CorePens.GetBrush(unotBA_Color);


        private static SKColor unitPosColor = SKColor.Parse("#B0004089");
        private static SKColor unitNegColor = SKColor.Parse("#B0A03000");
        private static SKColor unotPosColor = SKColor.Parse("#B000C09C");
        private static SKColor unotNegColor = SKColor.Parse("#B0C05030");

		private static readonly SKPaint unitNegPen = CorePens.GetPen(unitNegColor, 3f);
		private static readonly SKPaint unitPosPen = CorePens.GetPen(unitPosColor, 3f);
		private static readonly SKPaint unotNegPen = CorePens.GetPen(unotNegColor, 3f);
		private static readonly SKPaint unotPosPen = CorePens.GetPen(unotPosColor, 3f);

        private static readonly SKPaint blackText = CorePens.GetText(SKColor.Parse("#FF444444"), 14);
        private static readonly SKPaint resultText = CorePens.GetText(SKColor.Parse("#B0004089"), 12);
        private static readonly SKPaint unitText = CorePens.GetText(SKColor.Parse("#A0F87A0E"), 18);
        private static readonly SKPaint unotText = CorePens.GetText(SKColor.Parse("#B000D0FF"), 18);

        private static readonly SKPaint TextBkg = CorePens.GetBrush(SKColor.Parse("#A0FFFFFF"));


        private static readonly SKPaint unitRect_Pen = CorePens.GetPen(SKColors.Wheat, 0.5f);
		private static readonly SKPaint unitXformRect_Pen = CorePens.GetPen(SKColors.White, 1f);
    }
}