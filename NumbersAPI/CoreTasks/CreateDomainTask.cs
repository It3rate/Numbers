﻿using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateDomainTask : TaskBase, ICreateTask
    {
	    public Domain Domain;

	    public Trait Trait { get; }
	    public Focal BasisFocal { get; }
	    public Focal MinMax { get; }

        public override bool IsValid => true;

	    public CreateDomainTask(Trait trait, Focal basisFocal, Focal minMax)
	    {
		    Trait = trait;
		    BasisFocal = basisFocal;
		    MinMax = minMax;
	    }
	    public override void RunTask()
	    {
		    if (Domain == null)
		    {
			    Domain = new Domain(Trait, BasisFocal, MinMax);
		    }
		    else
		    {
                Trait.DomainStore.Add(Domain.Id, Domain);
		    }
		    Agent.Workspace.AddDomains(false, Domain);
	    }

	    public override void UnRunTask()
	    {
		    Agent.Workspace.RemoveDomains(false, Domain);
		    Trait.DomainStore.Remove(Domain.Id);
        }
    }
}
