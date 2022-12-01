﻿using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersAPI.CoreCommands;
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

        public CreateNumberCommand CreateNumberCommand { get; private set; }
	    public Number Number => ExistingNumber ?? CreatedNumber;
	    public Number CreatedNumber => NumberMapper?.Number;
	    public Number ExistingNumber { get; }

        public AddSKNumberCommand(SKDomainMapper domainMapper, Range range) : base(domainMapper.SegmentAlongGuideline(range))
        {
	        DomainMapper = domainMapper;
            CreateNumberCommand = new CreateNumberCommand(domainMapper.Domain, range);
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
			    Mapper = new SKNumberMapper(MouseAgent, CreateNumberCommand.Number);
            }
		    DomainMapper.AddNumberMapper(NumberMapper);
            MouseAgent.Workspace.AddElements(Number);
	    }

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

	    public override void Update()
	    {
	    }

	    public override void Completed()
	    {
	    }
    }
}
