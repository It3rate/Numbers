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
	    public CreateTraitTask(string name = "")
	    {
		    Name = name;
	    }
	    public override void RunTask()
	    {
		    if (Trait == null)
		    {
			    Trait = new Trait(Agent.Brain, Name);
		    }
		    else
		    {
			    Agent.Brain.TraitStore.Add(Trait.Id, Trait);
            }
		    Agent.Workspace.AddTraits(false, Trait);
	    }

	    public override void UnRunTask()
	    {
		    Agent.Workspace.RemoveTraits(false, Trait);
		    Agent.Brain.TraitStore.Remove(Trait.Id);
	    }
    }
}
