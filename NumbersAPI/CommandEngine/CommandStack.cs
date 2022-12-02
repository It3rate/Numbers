using System.Collections.Generic;
using NumbersAPI.Commands;
using NumbersCore.Primitives;

namespace NumbersAPI.CommandEngine
{
	public interface ICommandStack
    {
	    CommandAgent Agent { get; }
	    Brain Brain { get; }
        Workspace Workspace { get; }
        bool CanUndo { get; }
		int UndoSize { get; }
        bool CanRedo { get; }
		int RedoSize { get; }
		bool CanRepeat { get; }

		ICommand Do(params ICommand[] commands);
		ICommand PreviousCommand();
		bool Undo();
		void UndoAll();
		void UndoToIndex(int index);
		bool Redo();
		void RedoAll();
		void RedoToIndex(int index);
		void Clear();
	}

	public class CommandStack : ICommandStack
    {
	    public CommandAgent Agent { get; }
	    public Brain Brain => Agent.Brain;
	    public Workspace Workspace => Agent.Workspace;

        private int _stackIndex = 0;
		private readonly List<ICommand> _stack = new List<ICommand>(4096);
		private readonly List<ICommand> _toAdd = new List<ICommand>();
		private readonly List<ICommand> _toRemove = new List<ICommand>();

		public bool CanUndo => _stackIndex > 0;
		public int UndoSize => _stackIndex;
        public bool CanRedo => RedoSize > 0;
		public int RedoSize => _stack.Count - _stackIndex;

		public CommandStack(CommandAgent agent)
		{
			Agent = agent;
		}

		public ICommand Do(params ICommand[] commands)
		{
			RemoveRedoCommands();
			ICommand result = default(ICommand);
			foreach (var command in commands)
			{
				command.Stack = this;
				command.Agent = Agent;
				if (command.IsRetainedCommand)
				{
					// try merging with previous command
					AddAndExecuteCommand(command);
					result = command;
				}
			}

			return result;
		}

		public ICommand PreviousCommand() => CanUndo ? _stack[_stackIndex - 1] : default;

		public bool Undo()
		{
			var result = false;
			if (CanUndo)
			{
				_stackIndex--;
				_stack[_stackIndex].Unexecute();
				result = true;
			}

			return result;
		}

		public void UndoAll()
		{
			while (CanUndo)
			{
				Undo();
			}
		}

		public void UndoToIndex(int index)
		{
			while (_stackIndex >= index)
			{
				Undo();
			}
		}

		public bool Redo()
		{
			var result = false;
			if (CanRedo)
			{
				_stack[_stackIndex].Execute();
				_stackIndex++;
				result = true;
			}

			return result;
		}

		public void RedoAll()
		{
			while (CanRedo)
			{
				Redo();
			}
		}

		public void RedoToIndex(int index)
		{
			while (_stackIndex < index)
			{
				Redo();
			}
		}

		public bool Repeat()
		{
			return false;
		}

		public void Clear()
		{
			_stack.Clear();
			_toAdd.Clear();
			_toRemove.Clear();
			_stackIndex = 0;
		}

		public bool CanRepeat => true;

		public List<ICommand> GetRepeatable()
		{
			return null;
		}

		private void AddAndExecuteCommand(ICommand command)
		{
			command.Agent = Agent;
			RemoveRedoCommands();
			_stackIndex++;
			_stack.Add(command);
			command.Execute();
		}

		private void RemoveRedoCommands()
		{
			if (CanRedo)
			{
				_stack.RemoveRange(_stackIndex, RedoSize);
			}
		}

		// saving
		// temporal command clock, elements and updates.
	}
}