using NumbersCore.Primitives;

namespace NumbersCore.CoreConcepts.Time
{
	public class MillisecondNumber : Number
    {
        private static readonly MillisecondTimeDomain _millisecondTimeDomain = new MillisecondTimeDomain();

        private MillisecondTimeDomain MillisecondTimeDomain => _millisecondTimeDomain;
        public override Domain Domain
        {
	        get => MillisecondTimeDomain;
	        set { }
        }

        protected MillisecondNumber(IFocal focal) : base(focal) { }

	    public static MillisecondNumber Create(long duration, bool addToStore = false) => Create(0, duration, addToStore);
	    
	    public static MillisecondNumber Create(long delay, long duration, bool addToStore = false)
        {
	        var focal = Primitives.Focal.CreateZeroFocal(duration);
	        var result = new MillisecondNumber(focal);
	        Knowledge.Instance.MillisecondTimeDomain.AddNumber(result, addToStore);
	        return result;
        }

        public static MillisecondNumber Zero(bool addToStore = false) => new MillisecondNumber(Primitives.Focal.CreateZeroFocal(0));
    }
}
