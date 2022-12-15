using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Time
{
	public class MillisecondTimeDomain : Domain
    {
	    private MillisecondTimeDomain(Trait trait, IFocal basis, IFocal minMax) : base(trait, basis, minMax)
	    {
        }
	    private static MillisecondTimeDomain _MinMax;
	    public static MillisecondTimeDomain MinMax =>
            _MinMax ?? new MillisecondTimeDomain(new TimeTrait(), Focal.CreateZeroFocal(1000), Focal.MinMaxFocal);
    }
}
