using System.Collections.Generic;
using NumbersAPI.Commands;
using NumbersCore.Primitives;

namespace NumbersAPI.CommandEngine
{
	public interface ICommandStack<TCommand> where TCommand : ICommand
    {
		Workspace Workspace { get; }
		bool CanUndo { get; }
		bool CanRedo { get; }
		int RedoSize { get; }
		bool CanRepeat { get; }

		TCommand Do(params TCommand[] commands);
		TCommand PreviousCommand();
		bool Undo();
		void UndoAll();
		void UndoToIndex(int index);
		bool Redo();
		void RedoAll();
		void RedoToIndex(int index);
		void Clear();
	}

	public class CommandStack<TCommand> : ICommandStack<TCommand> where TCommand : ICommand
    {
		public Workspace Workspace { get; }

		private int _stackIndex = 0;
		private readonly List<TCommand> _stack = new List<TCommand>(4096);
		private readonly List<TCommand> _toAdd = new List<TCommand>();
		private readonly List<TCommand> _toRemove = new List<TCommand>();

		public bool CanUndo => _stackIndex > -0;
		public bool CanRedo => RedoSize > 0;
		public int RedoSize => _stack.Count - _stackIndex;

		public CommandStack(Workspace workspace)
		{
			Workspace = workspace;
		}

		public TCommand Do(params TCommand[] commands)
		{
			RemoveRedoCommands();
			TCommand result = default(TCommand);
			foreach (var command in commands)
			{
				command.Stack = (ICommandStack<ICommand>) this;
				if (command.IsRetainedCommand)
				{
					// try merging with previous command
					AddAndExecuteCommand(command);
					result = command;
				}
			}

			return result;
		}

		public TCommand PreviousCommand() => CanUndo ? (TCommand) _stack[_stackIndex - 1] : default;

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

		public List<TCommand> GetRepeatable()
		{
			return null;
		}

		private void AddAndExecuteCommand(TCommand command)
		{
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