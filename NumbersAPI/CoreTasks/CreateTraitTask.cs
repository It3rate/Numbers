using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateTraitTask : TaskBase, ICreateTask
    {
	    public Trait Trait;
	    public string Name;
        public override bool IsValid => true;
	    public CreateTraitTask(CommandAgent agent, string name = "") : base(agent)
	    {
		    Name = name;
	    }
	    public override void RunTask()
	    {
		    if (Trait == null)
		    {
			    Trait = new Trait(Agent.Brain);
		    }
		    else
		    {
			    Brain.TraitStore.Add(Trait.Id, Trait);
            }
		    Workspace.AddTraits(false, Trait);
	    }

	    public override void UnRunTask()
	    {
		    Workspace.RemoveTraits(false, Trait);
		    Brain.TraitStore.Remove(Trait.Id);
	    }
    }
}
