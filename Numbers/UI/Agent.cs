using System.Linq;
using System.Numerics;
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

        private Brain Brain { get; }
        private Workspace Workspace { get; }
        protected SKWorkspaceMapper WorkspaceMapper
        {
	        get
	        {
		        Brain.WorkspaceMappers.TryGetValue(Workspace.Id, out var mapper);
		        return mapper;
	        }
        }

        private RendererBase Renderer { get; }

        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }
        public bool IsPaused { get; set; } = true;

        public event EventHandler OnModeChange;
        public event EventHandler OnDisplayModeChange;
        public event EventHandler OnSelectionChange;

        private ColorTheme _colorTheme = ColorTheme.Normal;
        public ColorTheme ColorTheme
        {
	        get => _colorTheme;
	        set
	        {
		        if (_colorTheme != value)
		        {
			        _colorTheme = value;
			        Renderer.GeneratePens(_colorTheme);
		        }
	        }
        }

        private int _testIndex = 2;
        private readonly int[] _tests = new int[]{0,1,2};
        public Agent(Brain brain, RendererBase renderer)
        {
            Brain = brain;
            Renderer = renderer;
            Current = this;
            Workspace = new Workspace(Brain);

            ClearMouse();
            NextTest();
        }

        private List<Domain> CreateDomainLines(Trait trait, params long[] focalPositions)
        {
	        var result = new List<Domain>();
            var wm = Brain.WorkspaceMappers[Workspace.Id];
	        var unit = trait.FocalStore.Values.First();
	        var padding = 1.4;
	        long maxPos = (long)(focalPositions.Max() * padding);
	        long minPos = (long)(focalPositions.Min() * padding);
            var range = trait.AddFocalByUnitPositions(minPos, maxPos);
            var yt = 0.1f;
            var ytStep = (float)(0.8 / Math.Floor(focalPositions.Length / 2.0));
            for (int i = 1; i < focalPositions.Length; i += 2)
	        {
		        var domain = trait.AddDomain(unit.Id, range.Id);
		        result.Add(domain);
                var focal = trait.AddFocalByUnitPositions(focalPositions[i-1], focalPositions[i]);
		        var num = new Number(domain, focal.Id);
		        Workspace.AddDomain(domain);
		        var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(yt, 100));
	            dm.ShowGradientNumberLine = true;
	            dm.ShowNumberOffsets = true;
	            dm.ShowUnitMarkers = true;
	            dm.ShowUnits = true;
		        yt += ytStep;
	        }

            return result;
        }

        private SKWorkspaceMapper test2()
        {
	        Trait t0 = new Trait();
	        var unitSize = 8;
	        var unit = t0.AddFocalByUnitPositions(0, unitSize);
	        var wm = new SKWorkspaceMapper(Workspace, Renderer, 20, 20, 800, 800);
            CreateDomainLines(t0, 20, 50, -30, 40, -45, 64, -4, -10);
            return wm;
        }

        private SKWorkspaceMapper test0()
        {
            Trait t0 = new Trait();
	        var unitSize = 8;
	        var unit = t0.AddFocalByUnitPositions(0, unitSize);
	        var range = t0.AddFocalByUnitPositions(-16, 16);
	        var hDomain = t0.AddDomain(unit.Id, range.Id);
	        var vDomain = t0.AddDomain(unit.Id, range.Id);
	        var hFocal = t0.AddFocalByUnitPositions(-2, 9);
	        var vFocal = t0.AddFocalByUnitPositions(4, 6);
	        //var val2 = t0.AddFocalByUnitPositions(unitSize, unitSize);
	        //var val3 = t0.AddFocalByUnitPositions(unitSize, unitSize);

            var hNum = new Number(hDomain, hFocal.Id);
	        var vNum = new Number(vDomain, vFocal.Id);
	        var hSel = new Selection(hNum);
	        var transform = t0.AddTransform(hSel, vNum, TransformKind.Blend);

            Workspace.AddFullDomains(hDomain, vDomain);

	        var wm = new SKWorkspaceMapper(Workspace, Renderer, 150, 10, 600, 600);

	        var dm = wm.GetOrCreateDomainMapper(hDomain, wm.GetHorizontalSegment(.5f, 50));
	        dm.ShowGradientNumberLine = false;
	        dm.ShowValueMarkers = true;
	        dm.ShowUnitMarkers = false;
	        dm.ShowUnits = false;

            var dm2 = wm.GetOrCreateDomainMapper(vDomain, wm.GetVerticalSegment(.5f, 50));
	        dm2.ShowGradientNumberLine = false;
	        dm2.ShowValueMarkers = true;
	        dm2.ShowUnitMarkers = false;
            dm2.ShowUnits = false;
            return wm;
        }
        private SKWorkspaceMapper test1()
        {
	        Trait t0 = new Trait();
	        var unitSize = 4;
	        var unit = t0.AddFocalByUnitPositions(3, 3+unitSize);
	        var range = t0.AddFocalByUnitPositions(-40, 40);
	        var domain = t0.AddDomain(unit.Id, range.Id);
	        //var domain2 = t0.AddDomain(unit.Id, range.Id);
	        var val2 = t0.AddFocalByUnitPositions(-15, 20);
	        //var val3 = t0.AddFocalByUnitPositions(-40, 60);
	        //var val2 = t0.AddFocalByUnitPositions(unitSize, unitSize);
	        //var val3 = t0.AddFocalByUnitPositions(unitSize, unitSize);

	        var num2 = new Number(domain, val2.Id);
	        //var num3 = new Number(domain2, val3.Id);
	        //var sel = new Selection(num2);
	        //var transform = t0.AddTransform(sel, num3, TransformKind.Blend);

	        Workspace.AddFullDomains(domain);//, domain2);
            var wm = new SKWorkspaceMapper(Workspace, Renderer, 20,20, 800, 800);
            var dm = wm.GetOrCreateDomainMapper(domain, wm.GetHorizontalSegment(.3f, 100));
            dm.ShowGradientNumberLine = true;
            dm.ShowNumberOffsets = true;
            dm.ShowUnitMarkers = true;
            dm.ShowUnits = true;
            wm.EnsureRenderers();
            var nm = wm.NumberMapper(num2.Id);
            //dm.EndPoint += new SKPoint(0, -50);
            return wm;
        }


        public void NextTest()
        {
	        IsPaused = true;
	        ClearAll();
	        SKWorkspaceMapper wm;
            switch (_tests[_testIndex])
	        {
		        case 0:
			        wm = test0();
			        break;
		        case 1:
			        wm = test1();
			        break;
		        case 2:
                default:
			        wm = test2();
			        break;
            }
	        _testIndex = _testIndex >= _tests.Length - 1 ? 0 : _testIndex + 1;
            wm.EnsureRenderers();
            IsPaused = false;
        }

        private void ClearAll()
        {
            Workspace.ClearAll();
	        WorkspaceMapper?.ClearAll();
        }


        #region Position and Keyboard

        private bool _creatingOnDown = false;
        private SKPoint _downRawMousePoint;
        private SKPoint _rawMousePoint;
        public SKPoint GetTransformedPoint(SKPoint point) => point;// Data.Matrix.Invert().MapPoint(point);

        private Highlight _highlight = new Highlight();
        private HighlightSet SelBegin => Workspace.SelBegin;
        private HighlightSet SelCurrent   => Workspace.SelCurrent;
        private HighlightSet SelHighlight => Workspace.SelHighlight;
        private HighlightSet SelSelection => Workspace.SelSelection;

        public bool MouseDown(MouseEventArgs e)
        {
            if(IsPaused)return false;

            // Add to selection if ctrl down etc.
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            _downRawMousePoint = _rawMousePoint;
            WorkspaceMapper.GetSnapPoint(_highlight, SelCurrent, mousePoint);
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
	        if (IsPaused) return false;

            var result = false;
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            WorkspaceMapper.GetSnapPoint(_highlight, SelCurrent, mousePoint);
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
	        if (IsPaused) return false;

            if (SelBegin.HasHighlight && !IsDragging)
            {
	            var dist = (mousePoint - SelBegin.Position).Length;
                if (dist > _minDragDistance)
                {
                    IsDragging = true;
					SelCurrent.Set(_highlight.Clone());
					if (SelCurrent.ActiveHighlight.Mapper is SKNumberMapper nm && nm.IsUnitOrUnot)
					{
						Workspace.SaveNumberValues();
                    }
                }
            }

            if (IsDragging)
            {
                var activeHighlight = SelCurrent.ActiveHighlight;
	            var activeKind = activeHighlight.Kind;
	            if (activeHighlight.Mapper is SKNumberMapper nm)
	            {
		            nm.SetValueByKind(_highlight.SnapPoint, activeHighlight.Kind);
		            Workspace.RestoreNumberValues();
	            }
                else if (activeHighlight.Mapper is SKDomainMapper dm)
	            {
		            if (activeKind.IsDomainPoint())
		            {
			            dm.SetValueByKind(_highlight.SnapPoint, activeHighlight.Kind);
		            }
		            else if (activeKind.IsBoldTick())
		            {
			            //dm.SetValueByHighlight(_highlight);
		            }
                }
            }

            return true;
        }

        public bool MouseUp(MouseEventArgs e)
        {
	        if (IsPaused) return false;

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
	        if (IsPaused) return false;

            if (CurrentKey == Keys.Space && e.Button == MouseButtons.Left)
            {
                //Data.ResetZoom();
            }
            return true;
        }

        public float ScaleTickSize { get; set; } = 0.2f;
        public bool MouseWheel(MouseEventArgs e)
        {
	        if (IsPaused) return false;

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
	        if (CurrentKey == Keys.Escape)
	        {
		        IsPaused = !IsPaused;
	        }
	        if (IsPaused) return false;

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
                case Keys.E:
                    Workspace.LockValuesOnDrag = true;
                    break;
                case Keys.T:
                    NextTest();
                    break;
                //case Keys.F:
                //    UIMode = UIMode.CreateFocal;
                //    break;
                case Keys.D:
                    ColorTheme = ColorTheme == ColorTheme.Normal ? ColorTheme.Dark : ColorTheme.Normal;
                    break;
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
                Workspace.LockValuesOnDrag = false;
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
            Workspace.ClearNumberValues();
            SetSelectable(UIMode);
            if (Workspace != null)
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
