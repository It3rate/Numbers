namespace Numbers.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Drawing;
    using Numbers.Primitives;
    using NumbersCore.CoreConcepts.Spatial;
    using SkiaSharp;

    public class SKTextMapper : SKMapper
    {
        public TextElement TextElement => (TextElement)MathElement;
        public string[] Lines { get => TextElement.Lines; set => TextElement.Lines = value; }

        public SKTextMapper(MouseAgent agent, string[] lines, SKSegment guideline) : base(agent, new TextElement(lines), guideline)
        {
        }

        public override SKPath GetHighlightAt(Highlight highlight)
        {
            throw new NotImplementedException();
        }
        public void Draw()
        {
            var sp = Guideline.StartPoint;
            foreach (var line in Lines)
            {
                Renderer.DrawTextAt(sp, line, Pens.TextBrush);
                sp.Y += 30;
            }
        }
    }
}
