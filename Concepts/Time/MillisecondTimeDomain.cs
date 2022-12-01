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
	    public MillisecondTimeDomain(Knowledge knowledge) : base(knowledge.TimeTrait, Focal.CreateZeroFocal(1000), Focal.MaxFocal)
	    {
        }
    }
}
