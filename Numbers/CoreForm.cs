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
using Numbers.Renderer;

namespace Numbers
{
    public partial class CoreForm : Form
    {
	    private readonly CoreRenderer _renderer;
	    private readonly Control _control;
	    private readonly Agent.DesktopAgent _desktopAgent;

        public CoreForm()
        {
	        DoubleBuffered = true;
            InitializeComponent();

            _renderer = new CoreRenderer();
            _control = _renderer.AddAsControl(corePanel, false);
            _control.MouseDown += OnMouseDown;
            _control.MouseMove += OnMouseMove;
            _control.MouseUp += OnMouseUp;
            _control.MouseDoubleClick += OnMouseDoubleClick;
            _control.MouseWheel += OnMouseWheel;
            KeyDown += OnKeyDown;
            //KeyPress += OnKeyPress;
            KeyUp += OnKeyUp;
            KeyPreview = true;

            _desktopAgent = new Agent.DesktopAgent(_renderer);
            _renderer.CurrentAgentMapper = new SKAgentMapper(_desktopAgent, _renderer);
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
	        if (_desktopAgent.MouseDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
	        if (_desktopAgent.MouseMove(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
	        if (_desktopAgent.MouseUp(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
	        if (_desktopAgent.MouseDoubleClick(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
	        if (_desktopAgent.MouseWheel(e))
	        {
		        Redraw();
	        }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
	        if (_desktopAgent.KeyDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
	        if (_desktopAgent == null || _desktopAgent.KeyUp(e))
	        {
		        Redraw();
	        }
        }
        private void Redraw()
        {
	        //Renderer.DesktopAgent = DesktopAgent;
	        _control.Invalidate();
        }

    }
}
