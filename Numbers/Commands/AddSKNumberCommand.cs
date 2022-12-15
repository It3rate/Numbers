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

	    public override bool IsContinuous => true;

	    public CreateNumberCommand CreateNumberCommand { get; private set; }
	    public Number Number => ExistingNumber ?? CreatedNumber;
	    public Number CreatedNumber => NumberMapper?.Number;
	    public Number ExistingNumber { get; }

        public AddSKNumberCommand(SKDomainMapper domainMapper, Range range) : base(domainMapper.SegmentAlongGuideline(range))
        {
	        DomainMapper = domainMapper;
            CreateNumberCommand = new CreateNumberCommand(domainMapper.Domain, range);
            DefaultDuration = 2100;
            DefaultDelay = -900;
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
			    //_targetRange = CreateNumberCommand.Number.Value;
			    //CreateNumberCommand.Number.Value = new Range(0.0, 1.0);
                Mapper = new SKNumberMapper(MouseAgent, CreateNumberCommand.Number);
            }
		    DomainMapper.AddNumberMapper(NumberMapper);
            MouseAgent.Workspace.AddElements(Number);
	    }

        //private Range _targetRange;

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

     //   private double _t = 0.001;
	    //public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
	    //{
		   // base.Update(currentTime, deltaTime);
		   // _t = Math.Sin(LiveTimeSpan.RatioAt(currentTime.EndValue));
		   // CreateNumberCommand.Number.InterpolateFromOne(_targetRange, _t);
     //   }

	    //public override bool IsComplete() => Math.Abs(_t) >= 4.0;

	    public override void Completed()
        {
	        //_t = 1.0;
	        //CreateNumberCommand.Number.Value = _targetRange;
        }
    }
}
