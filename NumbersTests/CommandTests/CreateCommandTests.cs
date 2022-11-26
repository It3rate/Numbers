using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersAPI.CommandEngine;
using NumbersAPI.Commands;
using NumbersAPI.CoreCommands;
using NumbersAPI.CoreTasks;
using NumbersCore.Primitives;

namespace NumbersTests.CommandTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;


	[TestClass]
	public class CreateCommandTests
	{
		private CommandStack<ICommand> _stack;
		private Brain _brain;
		private Workspace _workspace;
		private CommandAgent _agent;
        private Trait _trait;

		[TestInitialize]
		public void Init()
		{
			_brain = Brain.ActiveBrain;
			_workspace = new Workspace(_brain);
			_agent = new CommandAgent(_brain, _workspace);
            _stack = new CommandStack<ICommand>(_agent);
			_trait = new Trait(_brain);
		}

		[TestMethod]
		public void CoreDomainTests()
		{
            var cdc = new CreateDomainCommand(_trait, 0, 10, -1000, 1000);
            _stack.Do(cdc);
            Assert.AreEqual(1, _stack.UndoSize);
            Assert.AreEqual(3, cdc.Tasks.Count);
		}
	}
}
