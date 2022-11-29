using NumbersAPI.CommandEngine;
using NumbersAPI.Commands;
using NumbersAPI.CoreTasks;
using NumbersCore.Primitives;

namespace NumbersAPI.CoreCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CreateDomainCommand : CommandBase
    {
	    public Domain Domain => DomainTask?.Domain;

        // combine tasks that are in core for common commands that require multiple tasks
	    private CreateFocalTask BasisTask;
	    private CreateFocalTask MinMaxTask;
	    private CreateDomainTask DomainTask;

        public Trait Trait { get; }

        public long BasisStart { get; }
        public long BasisEnd { get; }
        public long MinMaxStart { get; }
        public long MinMaxEnd { get; }

	    public int BasisFocalId { get; private set; } = -1;
        public int MinMaxFocalId { get; private set; } = -1;

        public CreateDomainCommand(Trait trait, long basisStart, long basisEnd, long minMaxStart, long minMaxEnd)
        {
	        Trait = trait;
	        BasisStart = basisStart;
	        BasisEnd = basisEnd;
	        MinMaxStart = minMaxStart;
	        MinMaxEnd = minMaxEnd;
        }
	    public CreateDomainCommand(Trait trait, int basisFocalId, int minMaxId)
	    {
		    Trait = trait;
		    BasisFocalId = basisFocalId;
		    MinMaxFocalId = minMaxId;
	    }

	    public override void Execute()
	    {
		    base.Execute();
		    if (BasisFocalId == -1)
		    {
			    BasisTask = new CreateFocalTask(Trait, BasisStart, BasisEnd);
			    MinMaxTask = new CreateFocalTask(Trait, MinMaxStart, MinMaxEnd);
			    AddTaskAndRun(BasisTask);
			    AddTaskAndRun(MinMaxTask);
	            BasisFocalId = BasisTask.Focal.Id;
			    MinMaxFocalId = MinMaxTask.Focal.Id;
		    }

		    DomainTask = new CreateDomainTask(Trait, BasisFocalId, MinMaxFocalId);
		    AddTaskAndRun(DomainTask);
	    }

	    public override void Unexecute()
	    {
		    base.Unexecute();
		    if (BasisTask != null) // revert to original state in case of redo.
		    {
			    BasisTask = null;
			    MinMaxTask = null;
			    BasisFocalId = -1;
                MinMaxFocalId = -1;
                // todo: undo should probably roll back all index counters as well.
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

