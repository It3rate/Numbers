using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Numbers.Mappers;
using Numbers.Agent;
using Numbers.Renderer;
using NumbersAPI.Motion;
using NumbersCore.Primitives;

namespace MathDemo
{
    public partial class CoreForm : Form
	{
		private readonly Demos _demos;
		private readonly CoreRenderer _renderer;
		private readonly Control _control;
	    private readonly MouseAgent _mouseAgent;
	    private Runner _runner;
	    private Workspace _workspace;

        public CoreForm()
        {
	        DoubleBuffered = true;
            InitializeComponent();

            _renderer = CoreRenderer.Instance;
            _control = _renderer.AddAsControl(corePanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            _control.MouseDoubleClick += OnMouseDoubleClick;
            _control.MouseWheel += OnMouseWheel;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            KeyPreview = true;

            _workspace = new Workspace(Brain.ActiveBrain);
            _demos = new Demos(_workspace.Brain, _renderer);
            _mouseAgent = new MouseAgent(_workspace, _control, _renderer, _demos);
            _runner = _mouseAgent.Runner;
            _ = Execute(null, 50);
        }

        public async Task Execute(Action action, int timeoutInMilliseconds)
        {
	        await Task.Delay(timeoutInMilliseconds);
            NextTest();
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

        private void OnKeyDown(object sender, KeyEventArgs e) { if (_mouseAgent.KeyDown(e)) { NeedsUpdate(); } }
        private void OnKeyUp(object sender, KeyEventArgs e) { if (_mouseAgent == null || _mouseAgent.KeyUp(e)) { NeedsUpdate(); } }

        private void NeedsUpdate() 
        { 
	        _runner.NeedsUpdate();
        }

    }
}
