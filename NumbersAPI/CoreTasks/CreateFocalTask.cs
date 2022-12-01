﻿using NumbersAPI.CommandEngine;
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
	    public IFocal Focal;

	    public Trait Trait { get; }
        public long StartPosition { get; }
        public long EndPosition { get; }

	    public override bool IsValid => true;

	    public CreateFocalTask(Trait trait, long startPosition, long endPosition)
	    {
		    Trait = trait;
		    StartPosition = startPosition;
            EndPosition = endPosition;
        }
	    public override void RunTask()
	    {
		    if (Focal == null)
		    {
			    Focal = NumbersCore.Primitives.Focal.CreateByValues(Trait, StartPosition, EndPosition, true);
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
