using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Time
{
	public class MillisecondTimeDomain : Domain
    {
	    public MillisecondTimeDomain(Knowledge knowledge) : base(knowledge.TimeTrait, Focal.CreateZeroFocal(1000), Focal.MaxFocal)
	    {
        }
    }
}
