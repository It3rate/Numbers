using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace NumbersAPI.CoreTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICreateNumberTask : ICreateTask
    {
	    Domain Domain { get; }
	    Number Number { get; }
    }

    public class CreateNumberByFocalIdTask : TaskBase, ICreateNumberTask
    {
	    public Domain Domain { get; }
	    public Number Number { get; private set; }
	    public int FocalId { get; }

	    public override bool IsValid => true;

	    public CreateNumberByFocalIdTask(Domain domain, int focalId) : base()
	    {
		    Domain = domain;
		    FocalId = focalId;
	    }
	    public override void RunTask()
	    {
		    if (Number == null)
		    {
			    Number = new Number(Domain, FocalId);
		    }
		    else
		    {
			    Domain.NumberIds.Add(Number.Id);
			    Agent.Brain.NumberStore.Add(Number.Id, Number);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Domain.NumberIds.Remove(Number.Id);
		    Agent.Brain.NumberStore.Remove(Number.Id);
	    }
    }
    public class CreateNumberByRangeTask : TaskBase, ICreateNumberTask
    {
	    public Domain Domain { get; }
	    public Number Number { get; private set; }

	    public Range Range { get; }

	    public override bool IsValid => true;

	    public CreateNumberByRangeTask(Domain domain, Range range) : base()
	    {
		    Domain = domain;
		    Range = range;
	    }
	    public override void RunTask()
	    {
		    if (Number == null)
		    {
			    Number = new Number(Domain, Range);
		    }
		    else
		    {
			    Domain.NumberIds.Add(Number.Id);
			    Agent.Brain.NumberStore.Add(Number.Id, Number);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Domain.NumberIds.Remove(Number.Id);
		    Agent.Brain.NumberStore.Remove(Number.Id);
	    }
    }
    public class CreateNumberByPositionsTask : TaskBase, ICreateNumberTask
    {
	    public Domain Domain { get; }
	    public Number Number { get; private set; }

	    public long StartPosition { get; set; }
	    public long EndPosition { get; set; }

        public override bool IsValid => true;

	    public CreateNumberByPositionsTask(Domain domain, long startPosition, long endPosition) : base()
	    {
		    Domain = domain;
		    StartPosition = startPosition;
		    EndPosition = endPosition;
	    }
	    public override void RunTask()
	    {
		    if (Number == null)
		    {
			    Number = new Number(Domain, StartPosition, EndPosition);
		    }
		    else
		    {
			    Domain.NumberIds.Add(Number.Id);
			    Agent.Brain.NumberStore.Add(Number.Id, Number);
		    }
	    }

	    public override void UnRunTask()
	    {
		    Domain.NumberIds.Remove(Number.Id);
		    Agent.Brain.NumberStore.Remove(Number.Id);
	    }
    }
}
