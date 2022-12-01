using NumbersCore.Primitives;

namespace Concepts.Time
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MillisecondTimeDomain : Domain
    {
	    public MillisecondTimeDomain(Mind mind) : base(mind.TimeTrait, mind.TimeTrait.CreateZeroFocal(1000), mind.TimeTrait.MaxFocal)
	    {
        }
    }
}
