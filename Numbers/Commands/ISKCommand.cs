using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersAPI.Commands;
using NumbersCore.Utils;

namespace Numbers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISKCommand : ICommand
    {
	    List<int> PreviousSelection { get; }

	    new MouseAgent Agent { get; }
        SKMapper Mapper { get; }
        SKSegment Guideline { get; }

	    float T { get; } // this will be Number once default traits are in.
    }
}
