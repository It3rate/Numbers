using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Numbers.Agent;
using Numbers.Renderer;
using NumbersAPI.Motion;
using NumbersCore.Primitives;
using Numbers;
using System.Diagnostics;

namespace MathDemo
{
    public partial class CoreForm : Form
	{
		private readonly IDemos _demos;
		private readonly CoreRenderer _renderer;
		private readonly Control _control;
	    private readonly MouseAgent _mouseAgent;
	    private Runner _runner;
	    private Workspace _workspace;
        
        public CoreForm()
        {
            InitializeComponent();
	        DoubleBuffered = true;
            KeyPreview = true;

            _renderer = CoreRenderer.Instance;
            _control = _renderer.AddAsControl(corePanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            _control.MouseDoubleClick += OnMouseDoubleClick;
            _control.MouseWheel += OnMouseWheel;
            _control.PreviewKeyDown += OnPreviewKeyDown;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;

            _workspace = new Workspace(Brain.ActiveBrain);
            //_demos = new Demos(_workspace.Brain);
            _demos = new Slides(_workspace.Brain);
            _mouseAgent = new MouseAgent(_workspace, _control, _renderer, _demos);
            _runner = _mouseAgent.Runner;
            _runner.lbEquation = lbText;
            _ = Execute(null, 50);
        }

        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
	        await Task.Delay(timeoutInMilliseconds);
            ReloadTest();
        }
        public void PreviousTest()
        {
            _runner.HasUpdated = false;
            _demos.PreviousTest(_mouseAgent);
            _runner.HasUpdated = true;
        }
        public void ReloadTest()
        {
            _runner.HasUpdated = false;
            _demos.Reload(_mouseAgent);
            _runner.HasUpdated = true;
        }
        public void NextTest()
        {
            _runner.HasUpdated = false;
            _demos.NextTest(_mouseAgent);
            _runner.HasUpdated = true;
        }


        private void OnMouseDown(object sender, MouseEventArgs e) { if (_mouseAgent.MouseDown(e)) { NeedsUpdate(); } }
        private void OnMouseMove(object sender, MouseEventArgs e) { if (_mouseAgent.MouseMove(e)) { NeedsUpdate(); } }
        private void OnMouseUp(object sender, MouseEventArgs e) { if (_mouseAgent.MouseUp(e)) { NeedsUpdate(); } }
        private void OnMouseDoubleClick(object sender, MouseEventArgs e) { if (_mouseAgent.MouseDoubleClick(e)) { NeedsUpdate(); } }
        private void OnMouseWheel(object sender, MouseEventArgs e) { if (_mouseAgent.MouseWheel(e)) { NeedsUpdate(); } }


        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyData == Keys.Right || e.KeyData == Keys.Left || e.KeyData == Keys.Down || e.KeyData == Keys.Up) // todo: can't figure out why arrow keys aren't passed, passing manually for now but may lead to double invoke.
            {
                var ea = new KeyEventArgs(e.KeyData);
                if (_mouseAgent.KeyDown(ea)) { NeedsUpdate(); }
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e) { if (_mouseAgent.KeyDown(e)) { NeedsUpdate(); } }
        private void OnKeyUp(object sender, KeyEventArgs e) { if (_mouseAgent == null || _mouseAgent.KeyUp(e)) { NeedsUpdate(); } }

        private void NeedsUpdate() 
        { 
	        _runner.NeedsUpdate();
        }

    }
}
