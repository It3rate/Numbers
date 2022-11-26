using NumbersAPI.CommandEngine;
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
	    public int BasisFocalId { get; }
	    public int MinMaxId { get; }

        public override bool IsValid => true;

	    public CreateDomainTask(CommandAgent agent, Trait trait, int basisFocalId, int minMaxId) : base(agent)
	    {
		    Trait = trait;
		    BasisFocalId = basisFocalId;
		    MinMaxId = minMaxId;
	    }
	    public override void RunTask()
	    {
		    if (Domain == null)
		    {
			    Domain = new Domain(Trait, BasisFocalId, MinMaxId);
		    }
		    else
		    {
                Trait.DomainStore.Add(Domain.Id, Domain);
		    }
		    Workspace.AddDomains(false, Domain);
	    }

	    public override void UnRunTask()
	    {
		    Workspace.RemoveDomains(false, Domain);
		    Trait.DomainStore.Remove(Domain.Id);
        }
    }
}
