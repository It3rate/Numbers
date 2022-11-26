using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateFocalTask : TaskBase, ICreateTask
    {
	    public FocalRef Focal;

	    public Trait Trait { get; }
        public long StartPosition { get; }
        public long EndPosition { get; }

	    public override bool IsValid => true;

	    public CreateFocalTask(CommandAgent agent, Trait trait, long startPosition, long endPosition) : base(agent)
	    {
		    Trait = trait;
		    StartPosition = startPosition;
            EndPosition = endPosition;
        }
	    public override void RunTask()
	    {
		    if (Focal == null)
		    {
			    Focal = FocalRef.CreateByValues(Trait, StartPosition, EndPosition);
		    }
		    else
		    {
			    Trait.FocalStore.Add(Focal.Id, Focal);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Trait.FocalStore.Remove(Focal.Id);
	    }
    }
}
