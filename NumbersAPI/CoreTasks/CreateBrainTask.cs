using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateBrainTask : TaskBase, ICreateTask
    {
	    public Brain Brain;

	    public override bool IsValid => true;

	    public CreateBrainTask(CommandAgent agent) : base(agent)
	    {
	    }
	    public override void RunTask()
	    {
		    if (Brain == null)
		    {
			    Brain = new Brain();
		    }
		    else
		    {
			    Brain.Brains.Add(Brain);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Brain.Brains.Remove(Brain);
	    }
    }
}
