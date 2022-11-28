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
	    public CommandAgent Agent { get; set; }

	    public Brain Brain => Agent.Brain;
	    public Workspace Workspace => Agent.Workspace;

	    public List<ITask> Tasks { get; } = new List<ITask>();
        protected int _taskIndex = 0;

        public virtual ICommandStack Stack { get; set; }
	    public virtual bool AppendElements()
	    {
		    throw new NotImplementedException();
	    }
	    public virtual bool RemoveElements()
	    {
		    throw new NotImplementedException();
	    }
	    public virtual bool IsMergableWith(ICommand command)
	    {
		    throw new NotImplementedException();
	    }
	    public virtual bool TryMergeWith(ICommand command) => false;

        public virtual DateTime StartTime { get; }
	    public virtual DateTime EndTime { get; }
	    public virtual TimeSpan Duration { get; }
	    public virtual int RepeatCount { get; }
	    public virtual int RepeatIndex { get; }
	    public virtual bool IsRepeatable()
	    {
		    throw new NotImplementedException();
	    }

	    public virtual bool IsActive { get; }
	    public virtual bool IsContinuous { get; }
	    public virtual bool IsRetainedCommand { get; } = true;
	    public virtual bool IsComplete() => true;
	    public virtual bool Evaluate() => true;

        public virtual void Execute()
        {
	        // remember selection state
	        // stamp times
	        // run tasks
	        // select new element
	        //foreach (var task in Tasks)
	        //{
	        // task.RunTask();
	        //}
        }
        public virtual void Update()
        {
        }
        public virtual void Unexecute()
        {
	        while (_taskIndex > 0)
	        {
		        _taskIndex--;
		        Tasks[_taskIndex].UnRunTask();
	        }
            Tasks.Clear();
        }

        public virtual void Completed() { }

        public void AddTask(ITask task)
        {
	        task.Agent = Agent;
	        Tasks.Add(task);
        }
        public void AddTasks(params ITask[] tasks)
        {
	        foreach (var task in tasks)
	        {
		        AddTask(task);
	        }
        }
        public void AddTaskAndRun(ITask task)
        {
	        AddTask(task);
	        RunToEnd();
        }
        public void AddTasksAndRun(params ITask[] tasks)
        {
	        foreach (var task in tasks)
	        {
		        AddTask(task);
	        }
	        RunToEnd();
        }
        protected void RunToEnd()
        {
	        while (_taskIndex < Tasks.Count)
	        {
		        Tasks[_taskIndex].RunTask();
		        _taskIndex++;
	        }
        }

        public virtual ICommand Duplicate()
        {
	        return null;
        }
    }
}
