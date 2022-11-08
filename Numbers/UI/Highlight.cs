using SkiaSharp;

namespace Numbers.UI
{
    using Numbers.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Highlight
    {
	    public SKPoint OrginalPoint { get; set; } = SKPoint.Empty;
	    public SKMapper Mapper { get; set; }
	    public float T { get; set; }
	    //public Number Range { get; set; } // will set selection ranges with a length trait number eventually
	    public bool IsSet => Mapper != null;

	    public Highlight()
	    {
	    }

	    public void Set(SKPoint point, SKMapper mapper, float t)
	    {
		    OrginalPoint = point;
		    Mapper = mapper;
		    T = t;
	    }
	    public void Reset()
	    {
            OrginalPoint = SKPoint.Empty;
            Mapper = null;
            T = 0;
	    }

	    public SKPath HighlightPath()
	    {
		    return Mapper.HighlightAt(T, OrginalPoint);
	    }
    }
}
