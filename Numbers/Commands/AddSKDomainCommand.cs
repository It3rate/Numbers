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
using NumbersCore.CoreConcepts.Time;
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

        public AddSKDomainCommand(MouseAgent agent, Domain domain, SKSegment guideline, SKSegment unitSegment) : base(guideline)
        {
	        ExistingDomain = domain;
	        UnitSegment = unitSegment;
        }
	    public AddSKDomainCommand(Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd, SKSegment guideline, SKSegment unitSegment) : base(guideline)
	    {
		    CreateDomainCommand = new CreateDomainCommand(trait, basisStart, basisEnd, minMaxStart, minMaxEnd);
		    UnitSegment = unitSegment;
        }

	    public override void Execute()
	    {
		    base.Execute();
		    if (CreateDomainCommand != null)
		    {
			    Stack.Do(CreateDomainCommand);
		    }

		    Mapper = MouseAgent.WorkspaceMapper.GetOrCreateDomainMapper(Domain, Guideline, UnitSegment);
		    DomainMapper.ShowGradientNumberLine = true;
		    DomainMapper.ShowBasis = true;
		    DomainMapper.ShowBasisMarkers = true;
        }

	    public override void Unexecute()
	    {
		    base.Unexecute();
		    MouseAgent.WorkspaceMapper.RemoveDomainMapper(DomainMapper.Id);
		    if (CreateDomainCommand != null)
		    {
			    Stack.Undo();
		    }
        }

	    public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
	    {
		    base.Update(currentTime, deltaTime);
	    }

        public override void Completed()
	    {
	    }
    }
}
