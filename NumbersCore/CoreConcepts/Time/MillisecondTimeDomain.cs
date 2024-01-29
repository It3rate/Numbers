using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Time
{
	public class MillisecondTimeDomain : Domain
    {
	    private MillisecondTimeDomain(Trait trait, Focal basis, Focal minMax) : base(trait, basis, minMax)
	    {
        }
	    public static MillisecondTimeDomain MinMax { get; } = new MillisecondTimeDomain(new TimeTrait(), Focal.CreateZeroFocal(1000), Focal.MinMaxFocal);
    }
}
