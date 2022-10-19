﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Numbers.Renderer;

namespace Numbers
{
    public partial class CoreForm : Form
    {
	    private readonly RendererBase _renderer;
	    private readonly Control _control;
	    private readonly IAgent _agent;

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

        }
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
	        if (_agent == null ||  _agent.MouseDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
	        if (_agent == null ||  _agent.MouseMove(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
	        if (_agent == null ||  _agent.MouseUp(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
	        if (_agent == null ||  _agent.MouseDoubleClick(e))
	        {
		        Redraw();
	        }
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
	        if (_agent == null ||  _agent.MouseWheel(e))
	        {
		        Redraw();
	        }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
	        if (_agent == null ||  _agent.KeyDown(e))
	        {
		        Redraw();
	        }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
	        if (_agent == null || _agent.KeyUp(e))
	        {
		        Redraw();
	        }
        }
        private void Redraw()
        {
	        //_renderer.Agent = Agent;
	        _control.Invalidate();
        }

    }
}
