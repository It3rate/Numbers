using NumbersAPI.CommandEngine;
using NumbersCore.Primitives;

namespace NumbersAPI.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    // Commands happen over time, can be repeatable, appendable, and can be evaluated to check for termination.
    // Commands are more or less formulas.
    public interface ICommand
    {
	    int Id { get; }
	    // selection range - segment, first, all, not, direction
        // motion range
        // motion type (ideally as a range) prepend, postpend, scale, or, and, not...
        // repeat range
        // duration range
        // termination range & bool
        CommandAgent Agent { get; }
	    Workspace MyWorkspace { get; }
	    //WorkspaceKind WorkspaceKind { get; }
	    ICommandStack<ICommand> Stack { get; set; }

	    bool AppendElements();
	    bool RemoveElements();
        bool IsMergableWith(ICommand command);
	    bool TryMergeWith(ICommand command);

	    DateTime StartTime { get; }
	    DateTime EndTime { get; }
	    TimeSpan Duration { get; }

        int RepeatCount { get; }
        int RepeatIndex { get; }
	    bool IsRepeatable();

	    bool IsActive { get; }
	    bool IsContinuous { get; }
	    bool IsRetainedCommand { get; }
	    bool IsComplete();

	    bool Evaluate();

        void Execute();
	    void Update();//SKPoint point);
	    void Unexecute();
	    void Completed();

	    List<ITask> Tasks { get; }
	    void AddTask(ITask task);
	    void AddTasks(params ITask[] tasks);
	    void AddTaskAndRun(ITask task);
	    void AddTasksAndRun(params ITask[] tasks);
	    //void RunToEnd();

	    // event EventHandler OnExecute;
	    // event EventHandler OnUpdate;
	    // event EventHandler OnUnexecute;
	    // event EventHandler OnCompleted;

	    ICommand Duplicate();
    }
}
