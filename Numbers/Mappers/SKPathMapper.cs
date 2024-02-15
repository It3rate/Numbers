namespace Numbers.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Drawing;
    using NumbersCore.Primitives;
    using SkiaSharp;

    public class SKPathMapper : SKMapper
    {
        public SKPathMapper(MouseAgent agent, PolyNumberChain numberSet, SKSegment xBasis) : base(agent, numberSet, xBasis)
        {
        }

        public override SKPath GetHighlightAt(Highlight highlight)
        {
            throw new NotImplementedException();
        }
    }
}
