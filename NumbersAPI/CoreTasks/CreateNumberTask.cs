using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateNumberTask : TaskBase, ICreateTask
    {
	    public Number Number;

        public Domain Domain { get; }
        public int FocalId { get; }

	    public override bool IsValid => true;

        public CreateNumberTask(CommandAgent agent, Domain domain, int focalId) : base(agent)
	    {
            Domain = domain;
            FocalId = focalId;
        }
	    public override void RunTask()
	    {
		    if (Number == null)
		    {
			    Number = new Number(Domain, FocalId);
		    }
		    else
		    {
			    Domain.NumberIds.Add(Number.Id);
			    Brain.NumberStore.Add(Number.Id, Number);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Domain.NumberIds.Remove(Number.Id);
		    Brain.NumberStore.Remove(Number.Id);
	    }
    }
}
