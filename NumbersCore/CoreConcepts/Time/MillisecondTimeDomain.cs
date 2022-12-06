using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Time
{
	public class MillisecondTimeDomain : Domain
    {
	    public MillisecondTimeDomain() : base(new TimeTrait(), Focal.CreateZeroFocal(1000), Focal.MaxFocal)
	    {
        }
    }
}
