﻿using Numbers.Agent;
using Numbers.Mappers;
using Numbers.Utils;
using NumbersAPI.CommandEngine;

namespace Numbers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SKCommandBase : CommandBase
    {
	    public List<int> PreviousSelection { get; }

	    public SKMapper Mapper { get; protected set; }
	    public SKSegment Guideline { get; }
	    public new MouseAgent Agent;
	    //public override CommandAgent Agent // agent in Desktop UI must be a mouse agent
	    //{
		   // get => MouseAgent;
		   // set => MouseAgent = (MouseAgent)value;
	    //}

	    public float DefaultDuration => 250;

	    public SKCommandBase(SKSegment guideline)
	    {
            Guideline = guideline;
	    }
	    public override void Execute()
	    {
		    base.Execute();
	    }

	    public override void Unexecute()
	    {
		    base.Unexecute();
	    }

	    public override void Update()
	    {
		    base.Update();
        }

	    public override void Completed()
	    {
		    base.Completed();
        }
    }
}
