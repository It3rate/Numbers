using NumbersAPI.Commands;
using NumbersCore.Primitives;

namespace NumbersAPI.CommandEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class CommandBase : ICommand
    {
	    public int Id { get; }
	    public CommandAgent Agent { get; }

	    public Brain MyBrain => Agent.Brain;
	    public Workspace MyWorkspace => Agent.Workspace;

        public ICommandStack<ICommand> Stack { get; set; }
	    public bool AppendElements()
	    {
		    throw new NotImplementedException();
	    }
	    public bool RemoveElements()
	    {
		    throw new NotImplementedException();
	    }
	    public bool IsMergableWith(ICommand command)
	    {
		    throw new NotImplementedException();
	    }
	    public bool TryMergeWith(ICommand command)
	    {
		    throw new NotImplementedException();
	    }

	    public DateTime StartTime { get; }
	    public DateTime EndTime { get; }
	    public TimeSpan Duration { get; }
	    public int RepeatCount { get; }
	    public int RepeatIndex { get; }
	    public bool IsRepeatable()
	    {
		    throw new NotImplementedException();
	    }

	    public bool IsActive { get; }
	    public bool IsContinuous { get; }
	    public bool IsRetainedCommand { get; }
	    public bool IsComplete()
	    {
		    throw new NotImplementedException();
	    }

	    public bool Evaluate()
	    {
		    throw new NotImplementedException();
	    }
        public void Execute()
	    {
		    throw new NotImplementedException();
	    }
        public void Update()
	    {
		    throw new NotImplementedException();
	    }
        public void Unexecute()
	    {
		    throw new NotImplementedException();
	    }
        public void Completed()
	    {
		    throw new NotImplementedException();
	    }

	    public List<ITask> Tasks { get; }
	    public void AddTask(ITask task)
	    {
		    throw new NotImplementedException();
	    }
        public void AddTasks(params ITask[] tasks)
	    {
		    throw new NotImplementedException();
	    }
        public void AddTaskAndRun(ITask task)
	    {
		    throw new NotImplementedException();
	    }
        public void AddTasksAndRun(params ITask[] tasks)
	    {
		    throw new NotImplementedException();
	    }

	    public ICommand Duplicate()
	    {
		    throw new NotImplementedException();
	    }
    }
}
