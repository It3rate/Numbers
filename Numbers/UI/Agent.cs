using Numbers.Core;
using Numbers.Mind;
using Numbers.Renderer;

namespace Numbers.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;
    using OpenTK.Graphics.OpenGL;
    using SkiaSharp;
    using SkiaSharp.Views.Desktop;

    public class Agent : IAgent
    {
        public static Agent Current { get; private set; }

        //public UIData Data;
        //private CommandStack<EditCommand> _editCommands;
        //private Entity _activeEntity;
        //private EditCommand _activeCommand;

        //public readonly Dictionary<PadKind, Pad> Pads = new Dictionary<PadKind, Pad>();

        //public Pad WorkingPad => PadFor(PadKind.Working);
        //public Pad InputPad => PadFor(PadKind.Input);
        //public Pad PadFor(PadKind kind) => Pads[kind];

        private readonly RendererBase _renderer;
        //public RenderStatus RenderStatus { get; }

        public int ScrollLeft { get; set; }
        public int ScrollRight { get; set; }

        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }

        //private List<int> _ignoreList = new List<int>();
        //private ElementKind _selectableKind = ElementKind.Any;
        public event EventHandler OnModeChange;
        public event EventHandler OnDisplayModeChange;
        public event EventHandler OnSelectionChange;

        public Agent(RendererBase renderer)
        {
            Current = this;
            //AddPad(PadKind.None); // for empty elements
            //AddPad(PadKind.Working);
            //AddPad(PadKind.Input);
            //Data = new UIData(this);
            //_editCommands = new CommandStack<EditCommand>(this);

            _renderer = renderer;
            //Renderer.Data = Data;

            //var entityCmd = new CreateEntityCommand(InputPad);
            //_editCommands.Do(entityCmd);
            //_activeEntity = entityCmd.Entity;

            //MakeXY();

            UIMode = UIMode.Any;
            ClearMouse();
            test0();
        }

        private Brain _brain;
        private Workspace _workspace;
        private void test0()
        {
	        _brain = Brain.BrainA;
	        _workspace = new Workspace(_brain, _renderer);
            _renderer.Workspaces.Add(_workspace);

	        Trait t0 = new Trait();
	        var unitSize = 500;
	        var unit = t0.AddFocalByValues(0, unitSize);
	        var range = t0.AddFocalByValues(-1000, 1000);
	        var domain = t0.AddDomain(unit.Id, range.Id);
	        var domain2 = t0.AddDomain(unit.Id, range.Id);
	        var val2 = t0.AddFocalByValues(900, 200);
	        var val3 = t0.AddFocalByValues(-400, 600);
	        //var val2 = t0.AddFocalByValues(unitSize, unitSize);
	        //var val3 = t0.AddFocalByValues(unitSize, unitSize);


            var num2 = new Number(domain, val2.Id);
	        var num3 = new Number(domain2, val3.Id);
	        var sel = new Selection(num2);
	        var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

	        _workspace.AddFullDomains(domain, domain2);

	        //var val0 = t0.AddFocalByIndexValue(unit.StartId, 650);
	        //var val1 = t0.AddFocalByValueIndex(-300, unit.StartId);
	        //var num0 = new Number(domain, val0.Id);
	        //var num1 = new Number(domain, val1.Id);
	        //Console.WriteLine(num0);
	        //Console.WriteLine(num1);
	        //Console.WriteLine(num2);
	        //var num3 = num2.Clone();
	        //num3.Add(num1);
	        //Console.WriteLine(num3);
	        //num3.Multiply(num0);
	        //Console.WriteLine(num3);
	        //num3.Divide(num0);
	        //Console.WriteLine(num3);
        }

        #region Position and Keyboard

        private bool _creatingOnDown = false;
        private SKPoint _downRawMousePoint;
        private SKPoint _rawMousePoint;
        public SKPoint GetTransformedPoint(SKPoint point) => point;// Data.Matrix.Invert().MapPoint(point);

        private Highlight _highlight = new Highlight();
        private HighlightSet SelBegin => _workspace.SelBegin;
        private HighlightSet SelCurrent   => _workspace.SelCurrent;
        private HighlightSet SelHighlight => _workspace.SelHighlight;
        private HighlightSet SelSelection => _workspace.SelSelection;

        public bool MouseDown(MouseEventArgs e)
        {
            // Add to selection if ctrl down etc.
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            _downRawMousePoint = _rawMousePoint;
	        _workspace.GetSnapPoint(_highlight, SelCurrent, mousePoint);
            SelHighlight.Set(_highlight);

            //Data.GetHighlight(mousePoint, Data.Begin, _ignoreList, false, _selectableKind);
            //Data.Begin.Position = mousePoint; // gethighlight clears position so this must be second.
            //Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, false, _selectableKind);

            if (e.Button == MouseButtons.Middle)
            {
                StartPan();
            }
            else
            {
	            if (_highlight.IsSet)
	            {
		            SelBegin.Set(_highlight.Clone());
                }
                //Data.GetHighlight(mousePoint, Data.Selected, _ignoreList, false, _selectableKind);
                //Data.Selected.Position = mousePoint;
                //if (UIMode == UIMode.SetUnit && Data.Selected.FirstElement is Focal focal)
                //{
                //    InputPad.SetUnit(focal);
                //}
            }

            IsDown = true;
            _creatingOnDown = _isControlDown;
            return true;
        }
        public bool MouseMove(MouseEventArgs e)
        {
            var result = false;
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            _workspace.GetSnapPoint(_highlight, SelCurrent, mousePoint);
            SelHighlight.Set(_highlight);

            if (IsDown)
            {
                result = MouseDrag(mousePoint);
            }
            return true;
        }

        private float _minDragDistance = 4f;
        public bool MouseDrag(SKPoint mousePoint)
        {
            if (UIMode == UIMode.Pan)
            {
                //Data.SetPanAndZoom(_startMatrix, _downRawMousePoint, _rawMousePoint - _downRawMousePoint, 1f);
            }
            else if (SelHighlight.HasHighlight && !IsDragging)
            {
	            var dist = (mousePoint - SelBegin.Position).Length;
                if (dist > _minDragDistance)
                {
                    IsDragging = true;
					SelCurrent.Set(_highlight.Clone());
                }
            }

            if (IsDragging)
            {
	            var ah = SelCurrent.ActiveHighlight;
	            if (ah.Mapper is SKNumberMapper nm)
	            {
                    if (SelCurrent.ActiveHighlight.T < 0.5)
		            {
			            nm.SetStartValueByPoint(_highlight.SnapPoint);
                    }
		            else
		            {
                        nm.SetEndValueByPoint(_highlight.SnapPoint);
                    }
	            }
            }

            return true;
        }

        public bool MouseUp(MouseEventArgs e)
        {
            // If dragging or creating, check for last point merge
            // If rect select, add contents to selection (also done in move).
            // If not dragging or creating and dist < selMax, click select
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);

            //Data.Current.UpdatePositions(mousePoint);
            //Data.GetHighlight(mousePoint, Data.Highlight, _ignoreList, false, _selectableKind);

            if (UIMode == UIMode.Pan)
            {
                //_startMatrix = Data.Matrix;
                if (_lastKeyUp != null)
                {
                    KeyUp(_lastKeyUp);
                }
                else
                {
                    UIMode = PreviousMode;
                }
            }
            //else if (Data.HasHighlightPoint && _activeCommand is IDraggableCommand cmd && cmd.HasDraggablePoint)
            //{
            //    if (cmd.DraggablePoint.CanMergeWith(Data.HighlightPoint))
            //    {
            //        var fromKey = cmd.DraggablePoint.Key;
            //        var toKey = Data.Highlight.Point.Key;
            //        if (fromKey != ElementBase.EmptyKeyValue && toKey != ElementBase.EmptyKeyValue && fromKey != toKey)
            //        {
            //            cmd.AddTaskAndRun(new MergePointsTask(cmd.Pad.PadKind, fromKey, toKey));
            //        }
            //    }
            //    cmd.AddTaskAndRun(new SetSelectionTask(Data.Selected, ElementBase.EmptyKeyValue));
            //}
            //else if (!IsDragging && _activeCommand == null)  // clicked
            //{
            //    var selCmd = new SetSelectionCommand(Data.Selected, Data.Highlight.PointKey, Data.Highlight.ElementKeysCopy);
            //    selCmd.Execute();
            //}

            OnSelectionChange?.Invoke(this, new EventArgs());
            ClearMouse();

            return true;
        }
        public bool MouseDoubleClick(MouseEventArgs e)
        {
            if (CurrentKey == Keys.Space && e.Button == MouseButtons.Left)
            {
                //Data.ResetZoom();
            }
            return true;
        }

        public float ScaleTickSize { get; set; } = 0.2f;
        public bool MouseWheel(MouseEventArgs e)
        {
            var scale = 1f + (Math.Sign(e.Delta) * ScaleTickSize);
            var rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            //Data.SetPanAndZoom(Data.Matrix, mousePoint, new SKPoint(0, 0), scale);
            return true;
        }

        private Keys CurrentKey;
        private UIMode _uiMode = UIMode.Any;
        public UIMode UIMode
        {
            get => _uiMode;
            set
            {
                if (value != _uiMode)
                {
                    _uiMode = value;
                    OnModeChange?.Invoke(this, new EventArgs());
                    SetSelectable(UIMode);
                }
            }
        }

        private void SetSelectable(UIMode uiMode)
        {
            switch (uiMode)
            {
                case UIMode.None:
                case UIMode.Any:
                    //_selectableKind = ElementKind.Any;
                    break;
                //case UIMode.CreateEntity:
                //    _selectableKind = ElementKind.Any;
                //    break;
                //case UIMode.CreateTrait:
                //    _selectableKind = ElementKind.TraitPart;
                //    break;
                //case UIMode.CreateFocal:
                //    _selectableKind = _isControlDown ? ElementKind.TraitPart : ElementKind.FocalPart;
                //    break;
                //case UIMode.CreateBond:
                //    _selectableKind = _isControlDown ? ElementKind.FocalPart : ElementKind.BondPart;
                //    break;
                case UIMode.SetUnit:
                    //_selectableKind = ElementKind.Focal;
                    break;
                //case UIMode.Equal:
                //    _selectableKind = ElementKind.Focal;
                //    Data.Selected.Clear();
                //    break;
            }
        }

        private bool _isControlDown;
        private bool _isShiftDown;
        private bool _isAltDown;
        private UIMode PreviousMode = UIMode.Any;
        private SKMatrix _startMatrix;
        // When SingleBond is selected, focals can be highlighted (but not moved), bonds can be created or edited and have precedence in conflict.
        // ctrl defaults to 'create' causing select to be exclusive to focals or singleBond points.
        public bool KeyDown(KeyEventArgs e)
        {
            CurrentKey = e.KeyCode;
            _isControlDown = e.Control;
            _isShiftDown = e.Shift;
            _isAltDown = e.Alt;
            var curMode = UIMode;
            switch (CurrentKey)
            {
                case Keys.Escape:
                    UIMode = UIMode.Any;
                    break;
                //case Keys.E:
                //    UIMode = UIMode.CreateEntity;
                //    break;
                //case Keys.T:
                //    UIMode = UIMode.CreateTrait;
                //    break;
                //case Keys.F:
                //    UIMode = UIMode.CreateFocal;
                //    break;
                //case Keys.D:
                //    UIMode = UIMode.CreateDoubleBond;
                //    break;
                //case Keys.B:
                //    UIMode = UIMode.CreateBond;
                //    break;
                case Keys.Delete:
                    DeleteSelected();
                    break;
                case Keys.U:
                    UIMode = UIMode.SetUnit;
                    break;
                //case Keys.Oemplus:
                //    UIMode = UIMode.Equal;
                //    break;
                case Keys.I:
                    ToggleShowNumbers();
                    break;
                case Keys.Space:
                    StartPan();
                    break;
                case Keys.Z:
                    if (_isShiftDown && _isControlDown)
                    {
                        //_editCommands.Redo();
                    }
                    else if (_isControlDown)
                    {
                        //_editCommands.Undo();
                    }
                    break;
                case Keys.R:
                    if (_isControlDown)
                    {
                        //_editCommands.Redo();
                    }
                    else
                    {
                        //_editCommands.Repeat();
                    }
                    break;
            }
            SetSelectable(UIMode);
            if (curMode != UIMode)
            {
                PreviousMode = curMode;
            }

            return true;
        }

        private KeyEventArgs _lastKeyUp;
        public bool KeyUp(KeyEventArgs e)
        {
            bool result = true;
            if (UIMode == UIMode.Pan && IsDown && _lastKeyUp == null)
            {
                _lastKeyUp = e;
                result = false;
            }
            else
            {
                _lastKeyUp = null;
                CurrentKey = Keys.None;
                _isControlDown = e.Control;
                _isShiftDown = e.Shift;
                //_isAltDown = e.Alt;
                //_startMatrix = Data.Matrix;
                //if (UIMode.IsMomentary())
                //{
                //    UIMode = PreviousMode;
                //}
                SetSelectable(UIMode);
            }
            return result;
        }

        private void DeleteSelected()
        {
            //if (Data.Selected.HasElement)
            //{
            //    var element = Data.Selected.FirstElement;
            //    var remCommand = new RemoveElementCommand(InputPad, element);
            //    _editCommands.Do(remCommand);
            //}
        }
        private void StartPan()
        {
            if (UIMode != UIMode.Pan)
            {
                //_startMatrix = Data.Matrix;
            }
            UIMode = UIMode.Pan;
        }

        #endregion

        //public DisplayMode DisplayMode
        //{
        //    get => Data.DisplayMode;
        //    set
        //    {
        //        Data.DisplayMode = value;
        //        OnDisplayModeChange?.Invoke(this, new EventArgs());
        //    }
        //}

        public void ToggleShowNumbers()
        {
            //if (DisplayMode.HasFlag(DisplayMode.ShowLengths))
            //{
            //    DisplayMode &= ~(DisplayMode.ShowAllValues);
            //}
            //else
            //{
            //    DisplayMode |= DisplayMode.ShowAllValues;
            //}
        }
        public void ClearMouse()
        {
            IsDown = false;
            IsDragging = false;
            SetSelectable(UIMode);
            if (_workspace != null)
            {
	            SelBegin?.Reset();
	            SelCurrent?.Reset();
            }
        }
        public void Clear()
        {
        }

    }

    [Flags]
    public enum UIMode
    {
	    None = 0,
	    //CreateEntity = 0x1,
	    //CreateTrait = 0x2,
	    //CreateFocal = 0x4,
	    //CreateDoubleBond = 0x8,
	    //CreateBond = 0x10,
	    SetUnit = 0x20,
	    //Equal = 0x40,
	    Perpendicular = 0x80,
	    Pan = 0x100,

	    //Inspect = 0x10,
	    //Edit = 0x20,
	    //Interact = 0x40,
	    //Animate = 0x80,
	    //XXX = 0x100,
	    //XXX = 0x200,

	    Any = 0xFFFF,

	    //Create = CreateEntity | CreateTrait | CreateFocal | CreateBond,
    }
}
