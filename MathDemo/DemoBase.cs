namespace MathDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Numbers;
    using Numbers.Agent;
    using Numbers.Mappers;
    using Numbers.Renderer;
    using SkiaSharp;

    public abstract class DemoBase : IDemos
    {
        protected int _testIndex = 0;
        protected MouseAgent _currentMouseAgent;
        protected List<int> _tests{get;} = new List<int>();

        protected abstract SKWorkspaceMapper GetPage(int index);

        public SKWorkspaceMapper PreviousTest(MouseAgent mouseAgent)
        {
            int index = _testIndex >= 1 ? _testIndex - 1 : _tests.Count - 1;
            return LoadTest(index, mouseAgent);
        }
        public SKWorkspaceMapper Reload(MouseAgent mouseAgent)
        {
            return LoadTest(_testIndex, mouseAgent);
        }
        public SKWorkspaceMapper NextTest(MouseAgent mouseAgent)
        {
            int index = _testIndex >= _tests.Count - 1 ? 0 : _testIndex + 1;
            return LoadTest(index, mouseAgent);
        }

        public SKWorkspaceMapper LoadTest(int index, MouseAgent mouseAgent)
        {
            _testIndex = index;
            _currentMouseAgent = mouseAgent;
            _currentMouseAgent.IsPaused = true;
            _currentMouseAgent.ClearAll();
            _currentMouseAgent.CurrentPen = CorePens.GetPen(SKColor.FromHsl(50, 80, 60, 255), 10);

            SKWorkspaceMapper wm = GetPage(_testIndex);
            wm.EnsureRenderers();
            _currentMouseAgent.IsPaused = false;
            return wm;
        }
    }
}
