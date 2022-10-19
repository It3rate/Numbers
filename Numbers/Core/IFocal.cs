namespace Numbers.Core
{
    public interface IFocal
	{
		long StartTickValue { get; set; }
		long EndTickValue { get; set; }
		long LengthInTicks { get; }
		Pointing Direction { get; }
		RatioSeg RatioIn(Domain domain);
    }

    public class RatioSeg
    {
	    public float Start { get; set; }
	    public float End { get; set; }

	    public RatioSeg(float start, float end)
	    {
		    Start = start;
		    End = end;
	    }
    }
}