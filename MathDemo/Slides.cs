namespace MathDemo
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers;
    using Numbers.Agent;
    using Numbers.Drawing;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using NumbersCore.Primitives;
    using SkiaSharp;

    public class Slides : DemoBase
    {
        private Brain Brain { get; }
        Random rnd = new Random();
        public Slides(Brain brain)
        {
            Brain = brain;
            Pages.AddRange(new PageCreator[]
            {
                RandomVsOrder_A,
                RandomVsOrder_B,
                GradientLine_A,
                GradientLine_B,
                Page2,
                Page3,
                Page4,
                Page5,
                Page6,
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
        private SKWorkspaceMapper RandomVsOrder_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);

            string[] txt = new string[] {
             "All things are random, ordered or a combination.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));

            var w = 20;
            for (int i = 0; i < 40; i++)
            {
                var path = wm.CreatePathMapper();
                path.Pen = CorePens.GetPen(SKColor.FromHsl((rnd.Next(100) + 150), 100, 50), 8);
                var x = rnd.Next(500) + 200;
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

            var paint = CorePens.GetPen(SKColors.Red, 12);
            for (int i = 0; i < 1; i++)
            {
                var path = wm.CreatePathMapper();
                path.Pen = paint;
                var x = 900;
                var y = 450;
                path.SetOval(new SKPoint(x, y), new SKPoint(x + 60, y + 60));
            }
            return wm;
        }
        private SKWorkspaceMapper GradientLine_A()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
                "Order can be represented by a gradient line - this is a proxy. ",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper GradientLine_B()
        {
            var wm = GradientLine_A();
            wm.AppendText(
                "Temperature, brightness, path home, points on a star. These are traits."
               );
            var w = 80;
            var hue = 100;
            var x = 100;
            var y = 200;
            var pts = 3;
            var count = 10;
            for (int i = 0; i < count; i++)
            {
                var path = wm.CreatePathMapper();
                path.Pen = CorePens.GetPen(SKColor.FromHsl(hue, 80, 40), 7);
                path.SetStar(new SKPoint(x, y), new SKPoint(x + w, y + w), pts++);
                x += 90;
                hue += 150 / count;
            }
            return wm;
        }
        private SKWorkspaceMapper Page2()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Everything that can happen on these gradient lines is valid math. ",
"Any non random thing is predictable to the extent of its precision.",
"Gradients can be any shape, including circles, but if they have branches they are no longer 1D.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
            return wm;
        }
        private SKWorkspaceMapper Page3()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"You can select areas. Selections can have two directions, one in the increasing direction, one in the decreasing. ",
"Gradients also have two directions, in that there are two options for the increasing direction. This is the polarity.",
"You can make multiple selections, and they can have different polarity.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page4()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"With multiple selections, you can compare lengths. The first is the basis of the comparison.",
"You can say things like longer, to the left of, twice as long, overlapping.",
"Switching basis is the reciprocal, shorter, to the right of, half as long.",
"Basis denotes zero, one, and dominant (positive) direction.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page5()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"You can add, subtract, select by starting with 'smallest imaginable' and growing. ",
"This stretches the tiny selection, addition.",
"If you don't do it 'zero wards', it stretches the start point off zero.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page6()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Can't encode segment with + and two unit numbers, that adds them.",
"Need to start with unot, end with unit, think an iceberg's height.",
"A segment on the inverted polarity ends on 'i'.",
"Infinity is horseshit, ALL numbers are segments. Points are (-xi,x). Domains are bounded.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
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
"Multiply is stretch existing (non zero) section. Stretch from unit maps to our numbers, but isn't required.",
"Divide is stretch from endpoint to unit (reciprocal action).",
"Multiply by negative flips. ",
"Multiply by inverted is normal, but uses 'i' basis, and flips polarity (making it a unique operation).",
"i*i*i*i... is a very important cycle.",
               };
            wm.CreateTextMapper(txt, new SKSegment(50, 50, 100, 50));
            return wm;
        }
        private SKWorkspaceMapper Page9()
        {
            var wm = new SKWorkspaceMapper(_currentMouseAgent, 100, 350, 800, 400);
            string[] txt = new string[] {
"Repeat steps are powers, but they can be done on any operation. The result is like a 'sectionable' result that can be merged.",
"Repeated addition (pushing from zero as it is segment addition) can show steps, or merge. ",
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
                dm.OffsetNumbers = true;
                dm.ShowBasisMarkers = true;
                dm.ShowBasis = true;
                yt += ytStep;
            }

            return result;
        }
        #endregion
    }
}
