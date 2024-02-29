﻿namespace MathDemo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices.Expando;
    using System.Security.Cryptography;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using MathDemo.Controls;
    using Numbers;
    using Numbers.Agent;
    using Numbers.Drawing;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using NumbersCore.CoreConcepts.Temperature;
    using NumbersCore.Primitives;
    using SkiaSharp;
    using static System.Net.Mime.MediaTypeNames;

    public class Slides : DemoBase
    {
        private Brain Brain { get; }
        private Random rnd = new Random();
        private SKPaint _oldTextPen;
        private SKPaint _newTextPen;
        public Slides(Brain brain)
        {
            Brain = brain;
            SKWorkspaceMapper.DefaultWorkspaceGhostText = CorePens.GetText(SKColor.Parse("#B0C0D0"), 18);
            SKWorkspaceMapper.DefaultWorkspaceText = CorePens.GetText(SKColor.Parse("#3030A0"), 18);
            _testIndex = 16;
            Pages.AddRange(new PageCreator[]
            {
                RandomVsOrder_A,
                RandomVsOrder_B,
                GradientLine_A,
                //GradientLine_B,
                Uncertainty_A,
                Categories_A,
                ValidMath_A,
                //ValidMath_B,
                Selection_A,
                Selection_B,
                Polarity_A,
                ComparisonsBasis_A,
                ComparisonsBasis_B,
                ComparisonsBasis_C,
                InvertedBasis_A,
                AddSubtract_A,
                AddSubtract_B,
                AddSubtract_C,
                UnitUnot_A,
                UnitUnot_B,
                //UnitUnot_C,
                DefineSegments_A,
                DefineSegments_B,
                DefineSegments_C,
                Page7,
                Page8,
                Page9,
                Page10,
                Page11,
                Page12,
                Page13,
                Page14,
                Page15,
            });
        }
        // todo: Add a TLDR page first.

        private SKWorkspaceMapper RandomVsOrder_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            wm.ShowNone();
            string[] txt = new string[] {
             "Derive math from first principles.",
             "All things are random, ordered or a combination.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            wm.CreateImageMapper("0_ants.png", new SKSegment(50, 230, 450, 230));

            var w = 20;
            for (int i = 0; i < 40; i++)
            {
                var path = wm.CreatePathMapper();
                path.Pen = CorePens.GetPen(SKColor.FromHsl((rnd.Next(100) + 150), 70, 50), 8);
                var x = rnd.Next(500) + 500;
                var y = rnd.Next(300) + 300;
                path.SetOval(new SKPoint(x, y), new SKPoint(x + w, y + w));
            }

            return wm;
        }
        private SKWorkspaceMapper RandomVsOrder_B()
        {
            var wm = RandomVsOrder_A();
            wm.AppendText(
            "Many things have order we can't distinguish: unseen patterns, or differences to small or large to measure.",
            "Random is simple, ordered is math. Distinguishing is precision."
            );
            var im = wm.LastImageMapper();
            im.Reset("1_antsSingleFile.png");

            var paint = CorePens.GetPen(SKColors.Red, 12);
            for (int i = 0; i < 5; i++)
            {
                var path = wm.CreatePathMapper();
                path.Pen = paint;
                var x = 700;
                var y = 180;
                path.SetOval(new SKPoint(x, y), new SKPoint(x + 60 + (float)rnd.NextDouble(), y + 60 + (float)rnd.NextDouble()));
            }
            return wm;
        }
        private SKWorkspaceMapper GradientLine_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            wm.ShowNone();
            string[] txt = new string[] {
                "Order can be represented by a gradient line - this is a proxy.",
                "Temperature, brightness, path home, points on a star. These are traits."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            wm.CreateImageMapper("3_path.jpg", new SKSegment(80, 150, 530, 150));


            wm.DefaultShowGradientNumberLine = false;
            wm.DefaultShowPolarity = false;
            wm.DefaultShowPolarity = false;
            wm.DefaultShowFractions = false;
            wm.DefaultShowMinorTicks = false;
            wm.DefaultShowTicks = false;

            // thermometer
            var left = 678;
            var right = 805;
            var top = 135;
            wm.CreateImageMapper("therm.png", new SKSegment(left, top, right, top));
            var mid = (right - left) / 2f + left;

            var guideline = new SKSegment(mid + 2, top + 116, mid + 2, top + 425);
            var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("therm", 1, 50), guideline);
            var num = hd.CreateNumber(new Focal(50, -10));
            num.InvertPolarity();

            // stars
            var w = 80;
            var hue = 100;
            var x = 970;
            var y = 50;
            var pts = 3;
            var count = 8;
            var paths = new List<SKPathMapper>();
            var val = num.Number.EndValue * 2;
            byte midH = 128;
            byte green = 30;
            for (int i = 0; i < count; i++)
            {
                var path = wm.CreatePathMapper();
                path.DefaultBrush = CorePens.GetBrush(new SKColor((byte)(midH + val), green, (byte)(midH - val)));
                path.Pen = CorePens.GetPen(SKColor.FromHsl(hue, 80, 20), 5);
                path.SetStar(new SKPoint(x, y), new SKPoint(x + w, y + w), pts++);
                y += 100;
                hue += 150 / count;
                paths.Add(path);
            }

            num.OnChanged += (sender, e) =>
            {
                val = Math.Min(100, Math.Max(-100,num.Number.EndValue * 2));
                var brush = CorePens.GetBrush(new SKColor((byte)(midH + val), green, (byte)(midH - val)));
                foreach (var path in paths)
                {
                    path.DefaultBrush = brush;
                }
            };

            return wm;
        }
        //private SKWorkspaceMapper GradientLine_B()
        //{
        //    var wm = GradientLine_A();
        //    wm.AppendText(
        //        "Temperature, brightness, path home, points on a star. These are traits."
        //       );

        //    wm.DefaultShowGradientNumberLine = false;
        //    wm.DefaultShowPolarity = false;
        //    wm.DefaultShowPolarity = false;
        //    wm.DefaultShowFractions = false;
        //    wm.DefaultShowMinorTicks = false;
        //    wm.DefaultShowTicks = false;

        //    // thermometer
        //    var left = 678;
        //    var right = 805;
        //    var top = 135;
        //    wm.CreateImageMapper("therm.png", new SKSegment(left, top, right, top));
        //    var mid = (right - left) / 2f + left;

        //    var guideline = new SKSegment(mid + 2, top + 116, mid + 2, top + 425);
        //    var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("therm", 1, 50), guideline);
        //    var num = hd.CreateNumber(new Focal(50, -10));
        //    num.InvertPolarity();

        //    // stars
        //    var w = 80;
        //    var hue = 100;
        //    var x = 1000;
        //    var y = 50;
        //    var pts = 3;
        //    var count = 8;
        //    for (int i = 0; i < count; i++)
        //    {
        //        var path = wm.CreatePathMapper();
        //        path.Pen = CorePens.GetPen(SKColor.FromHsl(hue, 80, 40), 7);
        //        path.SetStar(new SKPoint(x, y), new SKPoint(x + w, y + w), pts++);
        //        y += 100;
        //        hue += 150 / count;
        //    }

        //    return wm;
        //}
        private SKWorkspaceMapper Uncertainty_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 200, 650, 800, 300);
            wm.ShowNone();
            wm.DefaultShowTicks = true;
            string[] txt = new string[] {
                "We partition these traits into the highest resolution we can (or we care about), anything below this is unpredictable at this resolution.",
                "We can also choose a section we care about, accept values coming arbitrarily from anywhere in that section.",
                "All measures have uncertain, unimportant, or unpredictable aspects, but we can quantify and qualify these unknowns."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var control = new ShapeControl(_currentMouseAgent, 175, 320, 600, 400);
            wm.AddUIControl(control);

            var dm = wm.CreateLinkedNumber(control.Fill.Hue);
            wm.CreateLinkedNumber(control.Radius);
            wm.CreateLinkedNumber(control.RadiusOffset);
            wm.CreateLinkedNumber(control.Points);

            return wm;
        }
        private SKWorkspaceMapper Categories_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 200, 650, 800, 300);
            wm.ShowNone();
            wm.DefaultShowTicks = true;
            wm.DefaultShowFractions = true;
            string[] txt = new string[] {
                "When measurable properties cluster, we can create categories.",
                "These categories can be as strict or loose as needed, and we don't need to consider all properties at once.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var control = new FruitControl(_currentMouseAgent, 150, 150, 900, 300);
            wm.AddUIControl(control);

            wm.CreateLinkedNumber(control.Fill.Hue);
            wm.CreateLinkedNumber(control.Fill.Lightness);
            wm.CreateLinkedNumber(control.Size);
            wm.DefaultShowMinorTicks = true;
            wm.CreateLinkedNumber(control.AspectRatio);
            var (_, conv) = wm.CreateLinkedNumber(control.Convexity);
            //conv.OnSelected += (sender, e) =>
            //{
            //    Trace.WriteLine(control.GetValuesAsString());
            //};

            var label = wm.CreateTextMapper(new string[] { "Random" }, new SKSegment(60, 180, 100, 180));

            var fruitCount = FruitControl.FruitData.Count;
            var guideline = new SKSegment(70, 600, 70, 200);
            var kindDomain = wm.GetOrCreateDomainMapper(Domain.CreateDomain("kind", 1, 0, fruitCount, 0), guideline);
            var kindNum = kindDomain.CreateNumber(new Focal(0, 1));
            kindNum.OnChanged += (sender, e) =>
            {
                var name = control.UpdateToIndex((int)-kindNum.Number.StartValue);
                label.Lines[0] = name;
            };
            return wm;
        }


        private SKWorkspaceMapper ValidMath_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            wm.ShowNone();
            wm.DefaultShowTicks = true;
            wm.DefaultShowFractions = true;
            string[] txt = new string[] {
                "Gradients can be any shape, including circles. If they are anything more than a single path, they become multidimensional.",
                "*** Everything that can happen on these gradient lines is valid math. ***",
                "Many of these 'valid things' are intuitive, useful, and not covered by existing math. Some will follow here, but there are many more.",
                "You should explore this!",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            wm.CreateImageMapper("4_parkPath.jpg", new SKSegment(350, 250, 850, 250));
            
            var path = " 705.00,329.00, 706.00,340.00, 710.00,350.00, 723.00,365.00, 732.00,389.00, 731.00,396.00, 724.00,400.00, 715.00,401.00, 714.00,403.00, 709.00,403.00, 705.00,395.00, 685.00,374.00, 675.00,370.00, 644.00,383.00, 608.00,405.00, 593.00,407.00, 563.00,407.00, 560.00,400.00, 546.00,382.00, 532.00,335.00, 502.00,321.00, 500.00,318.00, 489.00,315.00, 467.00,314.00, 456.00,317.00, 412.00,343.00, 406.00,350.00, 397.00,364.00, 397.00,384.00, 400.00,395.00, 400.00,433.00, 398.00,436.00, 397.00,458.00, 400.00,466.00, 402.00,481.00, 402.00,514.00, 410.00,523.00, 425.00,530.00, 437.00,532.00, 478.00,532.00, 484.00,537.00, 484.00,559.00, 482.00,567.00, 470.00,584.00, 460.00,588.00, 454.00,594.00, 449.00,612.00, 449.00,619.00, 463.00,634.00, 471.00,636.00, 512.00,637.00, 516.00,636.00, 520.00,631.00, 527.00,629.00, 538.00,629.00, 557.00,625.00, 558.00,622.00, 570.00,613.00, 584.00,605.00, 606.00,597.00, 626.00,595.00, 638.00,603.00, 641.00,608.00, 647.00,612.00, 663.00,635.00, 665.00,641.00, 664.00,668.00, 666.00,670.00, 680.00,669.00, 683.00,625.00, 684.00,570.00, 682.00,554.00, 682.00,510.00, 679.00,466.00, 693.00,453.00, 704.00,447.00, 716.00,436.00, 735.00,437.00, 743.00,435.00, 747.00,416.00, 746.00,408.00, 744.00,406.00, 744.00,391.00, 749.00,379.00, 762.00,364.00, 770.00,350.00, 776.00,333.00, 776.00,331.00, 771.00,328.00, 742.00,327.00, 733.00,320.00";
            var pm = wm.CreatePathMapper();
            pm.SetStoredPoints(path);
            pm.Pen = CorePens.GetPen(SKColors.DarkBlue, 10);

            var guideline = new SKSegment(100, 200, 1100, 200);
            var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("validMath", 128, 0, 1, 0), guideline);
            var num = hd.CreateNumber(new Focal(0, 5));
            pm.SetPartialPath(-num.Number.StartTValue(), num.Number.EndTValue(), true);

            num.OnChanged += (sender, e) =>
            {
                pm.SetPartialPath(-num.Number.StartTValue(), num.Number.EndTValue(), true);
            };
            return wm;
        }
        //private SKWorkspaceMapper ValidMath_B()
        //{
        //    var wm = ValidMath_A();
        //    wm.AppendText(
        //       );
        //    //var guideline = new SKSegment(hd.MidPoint, hd.MidPoint + new SKPoint(0, 300));
        //    var guideline = new SKSegment(600, 200, 600, 800);
        //    var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("validMath", 10, 10), guideline);
        //    hd.ShowTicks = true;
        //    hd.ShowPolarity = false;

        //    return wm;
        //}
        private SKWorkspaceMapper Selection_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            wm.ShowAll();
            wm.DefaultShowPolarity = false;
            wm.DefaultShowFractions = false;
            wm.DefaultShowMinorTicks = false;
            string[] txt = new string[] {
                "There are two possible initial actions: create a selection, or choose an existing one.",
            "Create a selection by starting with a new position and expanding it.",
            "While stretching this tiny selection, new material is appended to the selection. Both positive and negative work.",
            "On a gradient, ALL motion must be linear and continuous, so creating a selection always results in a single segment.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var guideline = new SKSegment(200, 400, 1000, 400);
            var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("Selection", 1, 10), guideline);
            return wm;
        }
        private SKWorkspaceMapper Selection_B()
        {
            var wm = Selection_A();
            wm.AppendText(
            "If the final selection is less than the resolution, it still has position and length, just the length is to small to quantify.",
            "'Zero' length is not nil, zero position is also not nil. You can not destroy a selection by adjusting it."

            //"You can create a segment by selecting a position and expanding in the positive or negative direction.",
            //"You can also select an existing segment.",
            //"All selections have a start and end point, and therefore a direction.",
            //"Selection with '0' length, means the length is below the smallest resolution, and you can't tell the direction."
               );
            var last = wm.LastDomainMapper();
            return wm;
        }
        private SKWorkspaceMapper Polarity_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            wm.ShowAll();
            string[] txt = new string[] {
            "These gradient lines also have two directions, positive red or positive blue. This is the polarity.",
            "You can make multiple segments on a gradient line, and they can have different polarities."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var guideline = new SKSegment(200, 400, 1000, 400);
            var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("Polarity", 1, 10), guideline);
            return wm;
        }
        private SKWorkspaceMapper ComparisonsBasis_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 900, 400);
            wm.ShowAll();
            wm.DefaultShowPolarity = false;
            string[] txt = new string[] {
             //"Traits can be combined into new categories. Area, color, shortcuts, and speed are examples of combined traits.",
            "With multiple selections, you can compare lengths. The first is the basis of the comparison.",
            "You can say things like longer, to the left of, twice as long, overlapping."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var hd0 = wm.GetOrCreateDomainMapper(Domain.CreateDomain("Selection", 4, 3));
            var nm0 = hd0.CreateNumberFromFloats(0, 2f);
            return wm;
        }
        private SKWorkspaceMapper ComparisonsBasis_B()
        {
            var wm = ComparisonsBasis_A();
            wm.AppendText(
                "Switching basis is the reciprocal, shorter, to the right of, half as long.",
                "The basis denotes zero, one, and dominant (positive) direction."
            );

            return wm;
        }
        private SKWorkspaceMapper ComparisonsBasis_C()
        {
            var wm = ComparisonsBasis_B();
            wm.AppendText(
                "Using a different basis for the same property is a form of conversion."
            );

            wm.ShowAll();
            wm.DefaultShowMinorTicks = false;
            wm.DefaultShowPolarity = false;
            wm.DefaultShowFractions = false;
            wm.OffsetNextLineBy(100);
            var dmC = wm.GetOrCreateDomainMapper(TemperatureDomain.CelsiusDomain, null, null, "Celsius");
            var dmF = wm.GetOrCreateDomainMapper(TemperatureDomain.FahrenheitDomain, null, null, "Fahrenheit");
            var nm0 = dmF.CreateNumberFromFloats(35f, 8f);
            dmC.LinkNumber(nm0.Number);


            return wm;
        }
        private SKWorkspaceMapper InvertedBasis_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 900, 400);
            wm.ShowAll();
            wm.DefaultShowPolarity = true;
            string[] txt = new string[] {
                "What about inverted numbers? Every basis has an inverted basis.",
                "It always has the same length, it always defines zero and one, and it always points in the *opposite* direction."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var hd0 = wm.GetOrCreateDomainMapper(Domain.CreateDomain("Selection", 4, 3));
            var nm0 = hd0.CreateNumberFromFloats(0, 2f);


            return wm;
        }
        private SKWorkspaceMapper AddSubtract_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 400, 800, 400);
            wm.ShowAll();
            wm.DefaultShowPolarity = false;
            wm.DefaultShowMinorTicks = false;
            wm.DefaultShowFractions = false;
            string[] txt = new string[] {
                "Addition and subtraction are always two segment operations.",
                "Modifying existing selections from  endpoints is just a visual shorthand for adding two segments.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var hd0 = wm.GetOrCreateDomainMapper(Domain.CreateDomain("AddSubtract", 1, 16));
            return wm;
        }
        private SKWorkspaceMapper AddSubtract_B()
        {
            var wm = AddSubtract_A();
            wm.AppendText(
            "Segments can not be removed this way (that is deletion).", 
            "They can only be made to have a length too small to measure, which is zero, but not nil.",
            "Zero length segments can still be re-expanded. There are no operations that can be applied to nil (no selection)."
            );

            var leftDm = wm.LastDomainMapper();
            leftDm.Label = "  A";
            var rightDm = wm.GetOrCreateDomainMapper(Domain.CreateDomain("AddSubtract", 1, 16));
            rightDm.Label = "+ B";
            var resultDm = wm.GetOrCreateDomainMapper(Domain.CreateDomain("AddSubtract", 1, 16));
            resultDm.Label = "= C";
            resultDm.ShowNumbersOffset = true;

            rightDm.BasisNumber.Focal = leftDm.BasisNumber.Focal;
            resultDm.BasisNumber.Focal = leftDm.BasisNumber.Focal;
            var leftNum = leftDm.CreateNumberFromFloats(0, 2);
            var rightNum = rightDm.CreateNumberFromFloats(0, 3);
            Transform transform = Brain.AddTransform(leftNum.Number, rightNum.Number, TransformKind.Add);
            var tm = wm.GetOrCreateTransformMapper(transform);
            tm.Do2DRender = false;
            var resultNum = resultDm.Domain.AddNumber(transform.Result);
            return wm;
        }
        private SKWorkspaceMapper AddSubtract_C()
        {
            var wm = AddSubtract_B();
            wm.DefaultShowPolarity = true;
            wm.AppendText(
            "What does moving from the zero position imply? Do you think our number 5 represents a point or a segment here?"
            );
            wm.LastDomainMapper().ShowPolarity = true;
            return wm;
        }
        private SKWorkspaceMapper UnitUnot_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 50, 1050, 800);
            wm.ShowNone();
            wm.DefaultDrawPen = CorePens.GetPen(SKColor.Parse("#6090D0"), 8);
            string[] txt = new string[] {
                "If we say 3+8, we just get 11. How can we say 'from 3 to 8'?",
                "Complex numbers face this same dilemma.",
                "Look close at use cases. From and to. Above and below zero. Deep and high. Vertical axis direction. CCW..."
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            wm.CreateImageMapper("clock.png", new SKSegment(120, 280, 450, 280));

            var guideline = new SKSegment(120 + 170, 280 + 170, 460, 280 + 170);
            var hd = wm.GetOrCreateDomainMapper(Domain.CreateDomain("clock", 60, 0, 12, 0), guideline);
            var nm = hd.CreateNumberFromFloats(0, 11f);

            return wm;
        }
        private SKWorkspaceMapper UnitUnot_B()
        {
            var wm = UnitUnot_A();
            wm.ShowAll();
            wm.DefaultShowNumbersOffset = true;
            wm.AppendText(
                "The start point uses the inverted basis. The end point uses the polarity's basis.",
                "Remember these are two zero based values joined together.",
                "It is handy to write the end point in the polarity's units (3i+2, or -3-2i)."
            );
            wm.DefaultShowPolarity = true;
            wm.CreateImageMapper("iceberg.png", new SKSegment(550, 150, 1050, 150));

            var guideline = new SKSegment(1100, 850, 1100, 50);
            var hd0 = wm.GetOrCreateDomainMapper(Domain.CreateDomain("UnitUnot", 4, -11, 5, 0), guideline);
            return wm;
        }
        private SKWorkspaceMapper DefineSegments_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
                "To review: ALL numbers are directed segments, and they can exist on either polarity.",
                "Points are expressed as (-5i+5 or 5-5i). Domains are also bounded by resolution. There is no infinity.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper DefineSegments_B()
        {
            var wm = DefineSegments_A();
            wm.AppendText(
                "A point is a segment where you can't tell which direction it is pointing.",
                "A segment on the inverted polarity ends with the 'i' value."
            );
            return wm;
        }
        private SKWorkspaceMapper DefineSegments_C()
        {
            var wm = DefineSegments_B();
            wm.AppendText(
                "The inverted perspective represents the same information.",
                "The context you are in is part of this information.",
                "From the 'other' perspective, everything behaves normally.",
                "Once you develop an intuition for this dual perspective, you will see it everywhere."
            );
            return wm;
        }
        private SKWorkspaceMapper Page7()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
            "It is handy to break the units into 'ticks'. Math works without this, but there are difficulties. Our brains use ticks.",
            "If the unit is your smallest possible measurement, it is inaccurate by definition, and scaling to 1M will have error.",
            "Easier to have larger measure and divide it accurately, which is what ticks are.",
            "Ticks allow conversion between precision by narrowing numbers, otherwise you must renumber everything.",
            "Negative ticks are rounding. Zero sized ticks are crazy, and that is what we use in real numbers.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page8()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
            "Add means stretching from a (zero length) position. You can stretch from anywhere, but only the selected segment will be affected.",
            "Multiply means stretch an existing (non zero) section. Stretching from each perspective's unit maps to our numbers, but isn't required.",
            "Divide is stretch from endpoint to unit (reciprocal action).",
            "Multiply by negative flips.",
            "Multiply by an inverted value works the same, but uses 'i' basis, and flips polarity (making it a different operation than a unit value).",
            "i*i*i*i... is a very important cycle.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page9()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
            "Repeat steps are powers, but they can be done on any operation with at least one selection.",
            "Repeated creation (pushing from zero) merges results, repeated addition appends them.",
            "The result is like a 'sectionable' result that can be merged.",
            "The 'matter' is different than with multiplication, can be indicated with step coloring.",
            "Multiplication is a second step, do div by zero isn't a valid question. It is like x^4 with the third x missing.",
            "You can get lengths, areas, grids, fence posts, stacked boxes etc with various number combinations.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page10()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Merge operations (bool truth tables) are probably the most obvious, and should be first. There are 16.",
"These get complex in math, but simple in visualization and language (NAND is visually behind, and linguistically 'except')",
"These are like conditions that allow alternate paths along branches. Also can be 'physics' on segments.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page11()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Numbers and merge results can be compared, and the results can be described with quantitative bools, like prepositions:",
"near, over, to the left of, at least etc. These can all be constructed with segment definitions and bool operations.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page12()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Random sampling is a valid operation on a line, it is qualifiable, but not predictable outside those bounds.",
"The sampling need not be random point on line.",
"You could think aim for center of segments, from further based on length (normal dist).",
"This is currently complex math, but is very simple with segments, it is encoded as just a segment.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page13()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Join with branches, this happens as alternate paths on the same domain (same unit and trait).",
"Can be an optional or required paths, triggered or blocked by bool results.",
"Ornamental patterns and glyphs are examples of these visually. TYLOXP etc",
"Predictions and plans are by definition alternate paths. Words like 'either', 'detour', 'try' suggest them.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page14()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Join domains (area), use the same traits (spatial, optical, tactile), but combine different aspects of them (xy, rgb, curved/smooth)",
"Can be a 2D graph, join endpoints, and opposite tips. Drag out 4 lines to combine, triangles are alternate view.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page15()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Joint traits (mph), math is the same as with branches and other joins.",
"Common strategy is to normalize one trait (miles per gallon, liters per 100km)",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }

        #region Utils
        private SKDomainMapper CreateLowResDomain(int rangeSize, float offset, params Focal[] focals)
        {
            var newDomain = Domain.CreateDomain("lowres", 4, rangeSize);
            foreach (var focal in focals)
            {
                newDomain.CreateNumber(focal);
            }
            var wm = _currentMouseAgent.WorkspaceMapper;
            var result = wm.AddDomain(newDomain, offset, true);
            result.ShowGradientNumberLine = true;
            result.ShowBasis = true;
            result.ShowBasisMarkers = true;
            result.ShowMinorTicks = true;
            result.ShowPolarity = true;
            return result;
        }
        private SKDomainMapper CreateSimilarDomain(Domain domain, float offset, int rangeSize, params Focal[] focals)
        {
            var newDomain = Domain.CreateDomain(domain.Trait.Name, (int)domain.BasisFocal.LengthInTicks, rangeSize);
            foreach (var focal in focals)
            {
                newDomain.CreateNumber(focal);
            }
            var result = _currentMouseAgent.WorkspaceMapper.AddDomain(newDomain, offset, true, -200);
            result.ShowBasis = true;
            result.ShowBasisMarkers = true;
            result.ShowMinorTicks = false;
            return result;
        }
        /// <summary>
        /// Create lines with focal pairs.
        /// </summary>
        private List<Domain> CreateDomainLines(MouseAgent mouseAgent, Trait trait, Focal basisFocal, params long[] focalPositions)
        {
            var result = new List<Domain>();
            var wm = mouseAgent.WorkspaceMapper;
            var padding = 1.4;
            long maxPos = (long)Math.Max((focalPositions.Max() * padding), basisFocal.AbsLengthInTicks * padding);
            long minPos = (long)Math.Min((focalPositions.Min() * padding), -basisFocal.AbsLengthInTicks * padding);
            var range = new Focal(minPos, maxPos);
            var rangeLen = (double)range.LengthInTicks;
            var yt = 0.1f;
            var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
            //var domain = trait.AddDomain(basisFocal, range);
            ////domain.BasisIsReciprocal = true;
            //result.Add(domain);
            for (int i = 1; i < focalPositions.Length; i += 2)
            {
                var focal = new Focal(focalPositions[i - 1], focalPositions[i]);

                var domain = trait.AddDomain(basisFocal.Clone(), focal, "demoDomain"); // clone to avoid duplicate focals.
                                                                                       //domain.BasisIsReciprocal = true;
                result.Add(domain);

                var num = domain.CreateNumber(focal);
                mouseAgent.Workspace.AddDomains(true, domain);
                var displaySeg = wm.GetHorizontalSegment(yt, 100);
                var y = displaySeg.StartPoint.Y;

                var sz = domain.BasisIsReciprocal ? 0.01f : 0.05f;
                var unitSeg = displaySeg.SegmentAlongLine(0.5f - sz, 0.5f + sz);
                //var unitSeg = new SKSegment((float)unitStart, y, (float)unitStart + 20f, y);
                var dm = wm.GetOrCreateDomainMapper(domain, displaySeg, unitSeg);
                dm.ShowGradientNumberLine = true;
                dm.ShowNumbersOffset = true;
                dm.ShowBasisMarkers = true;
                dm.ShowBasis = true;
                yt += ytStep;
            }

            return result;
        }
        #endregion
    }
}
