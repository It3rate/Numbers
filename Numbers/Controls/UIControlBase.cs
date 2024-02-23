namespace Numbers.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers.Agent;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using SkiaSharp;

    public abstract class UIControlBase
    {
        public MouseAgent Agent { get; }
        public SKWorkspaceMapper WorkspaceMapper => Agent.WorkspaceMapper;
        public CoreRenderer Renderer => Agent.Renderer;
        public SKCanvas Canvas => Renderer.Canvas;

        public UIControlBase(MouseAgent agent) 
        {
            Agent = agent;
        }

        public abstract void Draw();
    }
}
