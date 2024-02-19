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
        public string Text { get => TextElement.Text; set => TextElement.Text = value; }

        public SKTextMapper(MouseAgent agent, string text, SKSegment guideline) : base(agent, new TextElement(text), guideline)
        {
        }

        public override SKPath GetHighlightAt(Highlight highlight)
        {
            throw new NotImplementedException();
        }
        public void Draw()
        {
            Renderer.DrawTextAt(Guideline.StartPoint, Text, Pens.TextBrush);
        }
    }
}
