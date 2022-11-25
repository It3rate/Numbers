using Numbers.Utils;
using NumbersCore.Primitives;
using SkiaSharp;

namespace Numbers.Agent
{
	public class HighlightSet
    {
        public Highlight ActiveHighlight { get; set; }
	    public SKPoint Position => ActiveHighlight?.OrginalPoint ?? SKPoint.Empty;
	    public SKPoint SnapPosition => ActiveHighlight?.SnapPoint ?? SKPoint.Empty;
	    //public List<Highlight> Highlights { get; set; } // todo: make selections multiple sub-highlights
	    public bool HasHighlight => ActiveHighlight?.Mapper != null;

        // copied values from start of change transaction, probably need a separate class as abilities expand
	    public SKSegment OriginalSegment { get; set; }
	    public FocalPositions OriginalFocalPositions { get; set; }

        public void Reset()
	    {
		    ActiveHighlight = null;
	    }
	    public void Set(Highlight activeHighlight)
	    {
		    ActiveHighlight = activeHighlight;
	    }

	    public void Clear()
	    {
		    ActiveHighlight = null;
	    }
    }
}
