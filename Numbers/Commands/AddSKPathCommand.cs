namespace Numbers.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Drawing;
    using Numbers.Mappers;
    using NumbersAPI.CoreTasks;
    using NumbersCore.CoreConcepts.Time;
    using NumbersCore.Primitives;

    public class AddSKPathCommand : SKCommandBase
    {
        public SKPathMapper PathMapper;

        private MouseAgent _agent;

        public AddSKPathCommand(MouseAgent agent, SKSegment guideline = null) : base(guideline)
        {
            _agent = agent;
        }

        public override void Execute()
        {
            if (PathMapper == null)
            {
                PathMapper = new SKPathMapper(_agent, Guideline);
            }
            base.Execute(); // no tasks, but call anyway
            _agent.WorkspaceMapper.AddPathMapper(PathMapper);
        }

        public override void Unexecute()
        {
            base.Unexecute();
            _agent.WorkspaceMapper.RemovePathMapper(PathMapper);
        }

        public override void Update(MillisecondNumber currentTime, MillisecondNumber deltaTime)
        {
            base.Update(currentTime, deltaTime);
        }

        public override void Completed()
        {
        }
    }
}
