using NumbersAPI.CommandEngine;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateTraitTask : TaskBase, ICreateTask
    {
	    public override bool IsValid => true;
	    public CreateTraitTask(CommandAgent agent) : base(agent)
	    {
	    }
    }
}
