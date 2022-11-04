﻿using SkiaSharp;

namespace Numbers.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CorePens
    {
        public List<SKPaint> Pens = new List<SKPaint>();

        public float DefaultWidth { get; }
        public bool IsHoverMap { get; set; }

        public SKPaint DrawPen { get; private set; }
        public SKPaint GrayPen { get; private set; }
        public SKPaint TickBoldPen { get; private set; }
        public SKPaint TickPen { get; private set; }
        public SKPaint SegPen { get; private set; }


        public SKPaint HoverPen { get; private set; }
        public SKPaint SelectedPen { get; private set; }
        public SKPaint UnitPen { get; private set; }
        public SKPaint UnitGhostPen { get; private set; }
        public SKPaint DarkPen { get; private set; }
        public SKPaint WorkingPen { get; private set; }
        public SKPaint HighlightPen { get; private set; }
        public SKPaint LockedPen { get; private set; }
        public SKPaint FocalPen { get; private set; }
        public SKPaint BondPen { get; private set; }
        public SKPaint BondFillPen { get; private set; }
        public SKPaint BondSelectPen { get; private set; }

        public SKPaint LineTextPen { get; private set; }
        public SKPaint TextBackgroundPen { get; private set; }
        public SKPaint SlugTextPen { get; private set; }

        public CorePens(float defaultWidth = 1f)
        {
            DefaultWidth = defaultWidth;
            GenPens();
        }

        public SKPaint this[int i] => GetPenForIndex(i);
        private SKPaint GetPenForIndex(int index)
        {
            SKPaint result;
            index = index < 0 ? 0 : index;
            if (index < Pens.Count)
            {
                result = Pens[index];
            }
            else
            {
                throw new OverflowException("Pen not found with index:" + index);
                //result = GetPenByOrder(index - Pens.Count);
            }

            return result;
        }
        public SKPaint GetPenByOrder(int index, float widthScale = 1, bool antiAlias = true)
        {
            //uint col = (uint)((index + 3) | 0xFF000000);
            uint col = (uint)((index + 3) * 0x110D05) | 0xFF000000;
            if (IndexOfColor.ContainsKey(col))
            {
                IndexOfColor[col] = index;
            }
            else
            {
                IndexOfColor.Add(col, index);
            }

            var color = new SKColor(col);
            return GetPen(color, DefaultWidth * widthScale, antiAlias);
        }
        public int IndexOfPen(SKPaint pen) => Pens.IndexOf(pen);

        private void GenPens()
        {
            DrawPen = GetPen(SKColors.LightBlue, DefaultWidth * 4);
            GrayPen = GetPen(SKColors.LightGray, DefaultWidth * .75f);
            TickBoldPen = GetPen(SKColors.LightCyan, DefaultWidth * 1f);
            TickPen = GetPen(SKColors.LightGray, DefaultWidth * 0.5f);
            SegPen = GetPen(new SKColor(20, 200, 255, 100), DefaultWidth * 4f);

            HoverPen = GetPen(new SKColor(240, 190, 190), DefaultWidth * 5);
            SelectedPen = GetPen(SKColors.Red, DefaultWidth * 1f);
            UnitPen = GetPen(new SKColor(10, 200, 100, 150), DefaultWidth * 5f);
            UnitGhostPen = GetPen(new SKColor(10, 200, 100, 50), DefaultWidth * 5f);
            DarkPen = GetPen(SKColors.Black, DefaultWidth);
            WorkingPen = GetPen(SKColors.LightCoral, DefaultWidth);
            HighlightPen = GetPen(SKColors.DarkRed, DefaultWidth * 5f);
            LockedPen = GetPen(new SKColor(180, 180, 190), DefaultWidth * 1);
            FocalPen = GetPen(new SKColor(100, 120, 210), DefaultWidth * 3);
            BondPen = GetPen(new SKColor(100, 20, 240), DefaultWidth * 2);
            //BondFillPen = GetPen(new SKColor(100, 20, 240, 40), DefaultWidth * 2);
            //BondFillPen.Style = SKPaintStyle.Fill;
            BondFillPen = new SKPaint();
            BondFillPen.IsAntialias = true;
            BondFillPen.Color = new SKColor(100, 20, 240, 40);
            BondFillPen.Style = SKPaintStyle.Fill;
            BondSelectPen = new SKPaint();
            BondSelectPen.Color = new SKColor(50, 10, 200, 50);
            BondSelectPen.Style = SKPaintStyle.Fill;

            LineTextPen = new SKPaint(new SKFont(SKTypeface.Default, 12f));
            LineTextPen.IsAntialias = true;
            LineTextPen.Color = new SKColor(0x40, 0x40, 0x60);
            LineTextPen.TextAlign = SKTextAlign.Center;
            TextBackgroundPen = GetPen(new SKColor(244, 244, 244, 220), 0);
            TextBackgroundPen.Style = SKPaintStyle.Fill;

            SlugTextPen = new SKPaint(new SKFont(SKTypeface.Default, 8f));
            SlugTextPen.IsAntialias = true;
            SlugTextPen.Color = new SKColor(0x80, 0x40, 0x40);
            SlugTextPen.TextAlign = SKTextAlign.Center;

            Pens.Clear();
            Pens.Add(GetPen(SKColors.Black, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkRed, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkOrange, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkGoldenrod, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkOliveGreen, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkGreen, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkCyan, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkBlue, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkOrchid, DefaultWidth));
            Pens.Add(GetPen(SKColors.DarkMagenta, DefaultWidth));
            Pens.Add(GetPen(SKColors.Red, DefaultWidth));
            Pens.Add(GetPen(SKColors.Orange, DefaultWidth));
            Pens.Add(GetPen(SKColors.Yellow, DefaultWidth));
            Pens.Add(GetPen(SKColors.Chartreuse, DefaultWidth));
            Pens.Add(GetPen(SKColors.Green, DefaultWidth));
            Pens.Add(GetPen(SKColors.Cyan, DefaultWidth));
            Pens.Add(GetPen(SKColors.Blue, DefaultWidth));
            Pens.Add(GetPen(SKColors.Orchid, DefaultWidth));
            Pens.Add(GetPen(SKColors.Magenta, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth)); // filler
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));
            Pens.Add(GetPen(SKColors.White, DefaultWidth));

            Pens.Add(HoverPen);
            Pens.Add(SelectedPen);
            Pens.Add(UnitPen);
            Pens.Add(UnitGhostPen);
            Pens.Add(DarkPen);
            Pens.Add(GrayPen);
            Pens.Add(WorkingPen);
            Pens.Add(DrawPen);
            Pens.Add(HighlightPen);
            Pens.Add(LockedPen);
            Pens.Add(FocalPen);
            Pens.Add(BondPen);
            Pens.Add(BondFillPen);
            Pens.Add(BondSelectPen);
            Pens.Add(LineTextPen);
            Pens.Add(TextBackgroundPen);
            Pens.Add(SlugTextPen);
        }

        public Dictionary<uint, int> IndexOfColor { get; } = new Dictionary<uint, int>();

        public SKPaint GetPen(SKColor color, float width, bool antiAlias = true)
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
