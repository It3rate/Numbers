using NumbersAPI.Commands;
using NumbersCore.Primitives;

namespace NumbersAPI.CommandEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class TaskBase : ITask
    {
        private static int _idCounter = 1;
        public int Id { get; }

        public ICommand Command { get; }
        public CommandAgent Agent { get; }

        public Brain Brain => Agent.Brain;
        public Workspace Workspace => Agent.Workspace;
        public abstract bool IsValid { get; }
        public virtual void Initialize() { }

        protected TaskBase(CommandAgent agent)
        {
	        Agent = agent;
	        Id = _idCounter++;
        }
        public virtual void RunTask() { }
        public virtual void UnRunTask() { }

    }
}
