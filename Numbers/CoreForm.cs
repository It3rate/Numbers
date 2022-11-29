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
using NumbersAPI.Motion;
using NumbersCore.Primitives;

namespace Numbers
{
    public partial class CoreForm : Form
    {
	    private readonly CoreRenderer _renderer;
	    private Runner _runner;
        private readonly Control _control;
	    private readonly Agent.MouseAgent _mouseAgent;

        public CoreForm()
        {
	        DoubleBuffered = true;
            InitializeComponent();

            _renderer = new CoreRenderer();
            _runner = new Runner(this);

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

            _mouseAgent = new Agent.MouseAgent(Brain.ActiveBrain, _renderer);
        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
	        if (_mouseAgent.MouseDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
	        if (_mouseAgent.MouseMove(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
	        if (_mouseAgent.MouseUp(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
	        if (_mouseAgent.MouseDoubleClick(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
	        if (_mouseAgent.MouseWheel(e))
	        {
		        Redraw();
	        }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
	        if (_mouseAgent.KeyDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
	        if (_mouseAgent == null || _mouseAgent.KeyUp(e))
	        {
		        Redraw();
	        }
        }
        private void Redraw()
        {
	        //Renderer.MouseAgent = MouseAgent;
	        _control.Invalidate();
        }

    }
}
