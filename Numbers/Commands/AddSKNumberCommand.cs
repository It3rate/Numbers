using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersAPI.CoreCommands;
using NumbersCore.CoreConcepts.Time;
using NumbersCore.Primitives;
using NumbersCore.Utils;

namespace Numbers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddSKNumberCommand : SKCommandBase
    {
	    public SKNumberMapper NumberMapper => (SKNumberMapper)Mapper;
	    public SKDomainMapper DomainMapper { get; }

        public override bool IsContinuous => false;//true;

	    public CreateNumberCommand CreateNumberCommand { get; private set; }
	    public Number Number => ExistingNumber ?? CreatedNumber;
	    public Number CreatedNumber => NumberMapper?.Number;
	    public Number ExistingNumber { get; }

        public AddSKNumberCommand(SKDomainMapper domainMapper, Range range) : base(domainMapper.SegmentAlongGuideline(range))
        {
	        DomainMapper = domainMapper;
            CreateNumberCommand = new CreateNumberCommand(domainMapper.Domain, range);
            //DefaultDuration = 2100;
            //DefaultDelay = -900;
        }
	    public AddSKNumberCommand(SKDomainMapper domainMapper, Number existingNumber) : base(domainMapper.SegmentAlongGuideline(existingNumber.Value))
	    {
		    DomainMapper = domainMapper;
		    ExistingNumber = existingNumber;
	    }

        public override void Execute()
	    {
		    base.Execute();
		    if (CreateNumberCommand != null)
		    {
			    Stack.Do(CreateNumberCommand);
            }

		    if (Mapper == null)
		    {
			    //_targetNumber = CreateNumberCommand.Number.Clone();
			    //CreateNumberCommand.Number.Value = new Range(0.0, 1.0);
                // HaltCondition = new Evaluation(CreateNumberCommand.Number, _targetNumber, FilterOperator.B_Implies_A);
                Mapper = new SKNumberMapper(MouseAgent, CreateNumberCommand.Number);

            }
		    DomainMapper.AddNumberMapper(NumberMapper);
            MouseAgent.Workspace.AddElements(Number);
	    }

        private Number _targetNumber;

	    public override void Unexecute()
	    {
		    base.Unexecute();
		    DomainMapper.RemoveNumberMapper(NumberMapper);
		    MouseAgent.Workspace.RemoveElements(Number);
            if (CreateNumberCommand != null)
		    {
			    Stack.Undo();
		    }
	    }

        private double _t = 0.001;
	    public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
	    {
		   base.Update(currentTime, deltaTime);
		   _t = Math.Sin(LiveTimeSpan.RatioFromStart(currentTime.EndValue));
		  // CreateNumberCommand.Number.InterpolateFromOne(_targetNumber.Value, _t);
        }

	    public override void Completed()
        {
	        if (IsComplete())
	        {
		        _t = 1.0;
				CreateNumberCommand.Number.Value = _targetNumber.Value;
            }
        }
    }
}
