using Numbers.Core;
using Numbers.Renderer;
using SkiaSharp;

namespace Numbers.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SKNumberMapper : SKMapper
    {
        public Number Number { get; }
	    public SKSegment NumberSegment { get; }

	    private SKDomainMapper DomainMapper => Renderer.DomainMapper(Number.Domain.Id);

	    public override SKPoint StartPoint => NumberSegment.StartPoint;
	    public override SKPoint MidPoint => NumberSegment.Midpoint;
	    public override SKPoint EndPoint => NumberSegment.EndPoint;

        public SKSegment DomainSegment => DomainMapper.DomainSegment;

	    public SKNumberMapper(CoreRenderer renderer, Number number) : base(renderer, number)
	    {
		    Number = number;
	    }

        public void Draw(SKPoint offset, SKPaint paint)
	    {
		    var nr = Number.Ratio;
		    var numSeg = DomainSegment.SegmentAlongLine(nr.Start, nr.End);
		    numSeg += offset;
		    Renderer.DrawDirectedLine(numSeg, paint);
	    }
    }
}
