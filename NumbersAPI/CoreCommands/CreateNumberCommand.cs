﻿using NumbersAPI.CommandEngine;
using NumbersAPI.Commands;
using NumbersAPI.CoreTasks;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace NumbersAPI.CoreCommands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ICreateNumberCommand : ICommand
    {
	    Domain Domain { get; }
	    Number Number { get; }
    }
    public class CreateNumberCommand : CommandBase, ICreateNumberCommand
    {
        public Domain Domain { get; }
        public Number Number { get; private set; }

        private CreateNumberByFocalIdTask NumberByFocalIdTask;
        private CreateNumberByRangeTask NumberByRangeTask;
        private CreateNumberByPositionsTask NumberByPositionsTask;

        public IFocal Focal { get; }

        public Range Range { get; }

        public long StartPosition { get; }
        public long EndPosition { get; }

        private readonly bool _isById = false;
        private readonly bool _isByRange = false;
        private readonly bool _isByPositions = false;


        public CreateNumberCommand(Domain domain, IFocal focal)
        {
	        Domain = domain;
	        Focal = focal;
	        _isById = true;
        }
        public CreateNumberCommand(Domain domain, Range range)
        {
	        Domain = domain;
	        Range = range;
	        _isByRange = true;
        }
        public CreateNumberCommand(Domain domain, long startPosition, long endPosition)
        {
	        Domain = domain;
            StartPosition = startPosition;
            EndPosition = endPosition;
	        _isByPositions = true;
        }

        public override void Execute()
        {
            base.Execute();
            if (_isById)
            {
	            NumberByFocalIdTask = new CreateNumberByFocalIdTask(Domain, Focal);
	            AddTaskAndRun(NumberByFocalIdTask);
	            Number = NumberByFocalIdTask.Number;
            }
            else if (_isByRange)
            {
	            NumberByRangeTask = new CreateNumberByRangeTask(Domain, Range);
	            AddTaskAndRun(NumberByRangeTask);
	            Number = NumberByRangeTask.Number;
            }
            else if (_isByPositions)
            {
	            NumberByPositionsTask = new CreateNumberByPositionsTask(Domain, StartPosition, EndPosition);
	            AddTaskAndRun(NumberByPositionsTask);
	            Number = NumberByPositionsTask.Number;
            }
        }

        public override void Unexecute()
        {
            base.Unexecute();
            NumberByFocalIdTask = null;
            NumberByRangeTask = null;
            NumberByPositionsTask = null;
            Number = null;
        }

        public override void Update()
        {
        }

        public override void Completed()
        {
        }
    }
}
