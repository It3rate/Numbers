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
        SKSegment Guideline { get; }
    }
}
