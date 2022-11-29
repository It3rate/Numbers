using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersAPI.CommandEngine;
using NumbersAPI.Commands;
using NumbersAPI.CoreCommands;
using NumbersAPI.CoreTasks;
using NumbersCore.Primitives;

namespace Numbers.Commands
{

    public class AddSKDomainCommand : SKCommandBase
    {
	    public SKDomainMapper DomainMapper => (SKDomainMapper)Mapper;

	    public CreateDomainCommand CreateDomainCommand { get; private set; }
	    public Domain Domain => ExistingDomain ?? CreateDomainCommand?.Domain;
        public Domain ExistingDomain { get; }
	    public Domain CreatedDomain => CreateDomainCommand?.Domain;

	    public SKSegment UnitSegment { get; }
	    public SKSegment unitSegment { get; }

        public AddSKDomainCommand(MouseAgent agent, Domain domain, SKSegment guideline, SKSegment unitSegment) : base(guideline)
        {
	        ExistingDomain = domain;
        }
	    public AddSKDomainCommand(Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd, SKSegment guideline) : base(guideline)
	    {
		    CreateDomainCommand = new CreateDomainCommand(trait, basisStart, basisEnd, minMaxStart, minMaxEnd);
	    }

	    public override void Execute()
	    {
		    base.Execute();
		    if (ExistingDomain == null)
		    {
			    CreateDomainCommand.Execute();
		    }
		    Mapper = new SKDomainMapper(Agent, Domain, Guideline, UnitSegment);
		    Agent.WorkspaceMapper.Mappers[Domain.Id] = Mapper;
        }

	    public override void Unexecute()
	    {
		    base.Unexecute();
		    Agent.WorkspaceMapper.Mappers.Remove(Domain.Id);
		    if (ExistingDomain == null)
		    {
			    CreateDomainCommand.Unexecute();
		    }
        }

	    public override void Update()
	    {
	    }

	    public override void Completed()
	    {
	    }
    }
}
