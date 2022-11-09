using SkiaSharp;

namespace Numbers.UI
{
    using Numbers.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Highlight is a sub-selection for UI purposes, can select parts of an equation to drag, like a orgPoint on a segment, or segment in a transform.
    /// Can probably be merged with selection eventually, should be similar mechanisms to select with segments.
    /// </summary>
    public class Highlight
    {
	    public SKPoint OrginalPoint { get; set; } = SKPoint.Empty;
	    public SKPoint SnapPoint { get; set; } = SKPoint.Empty;
        public SKMapper Mapper { get; set; }
	    public float T { get; set; }
	    //public Number Range { get; set; } // will set selection ranges with a length trait number eventually
	    public bool IsSet => Mapper != null;

	    public Highlight()
	    {
	    }
	    private Highlight(SKPoint orgPoint, SKMapper mapper, float t)
	    {
		    OrginalPoint = orgPoint;
		    Mapper = mapper;
		    T = t;
	    }

        public void Set(SKPoint orgPoint, SKPoint snapPoint, SKMapper mapper, float t)
        {
	        OrginalPoint = orgPoint;
	        SnapPoint = snapPoint;
            Mapper = mapper;
		    T = t;
	    }
	    public void Reset()
        {
	        OrginalPoint = SKPoint.Empty;
	        SnapPoint = SKPoint.Empty;
            Mapper = null;
            T = 0;
	    }

	    public Highlight Clone()
	    {
            return new Highlight(new SKPoint(OrginalPoint.X, OrginalPoint.Y), Mapper, T);
	    }

	    public SKPath HighlightPath()
	    {
		    return Mapper.HighlightAt(T, OrginalPoint);
	    }
    }
}
