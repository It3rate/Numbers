﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Numbers.Mappers;
using Numbers.Renderer;
using NumbersAPI.CommandEngine;
using NumbersAPI.Motion;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Numbers.Agent
{
	public class MouseAgent : CommandAgent, IMouseAgent
	{
		public static Dictionary<int, SKWorkspaceMapper> WorkspaceMappers = new Dictionary<int, SKWorkspaceMapper>();
		public SKWorkspaceMapper WorkspaceMapper
		{
			get
			{
				WorkspaceMappers.TryGetValue(Workspace.Id, out var result);
				return result;
			}
		}

		public CoreRenderer Renderer { get; }
        public Runner Runner;

        public Format.Program Program { get; }
        public bool IsPaused { get; set; } = true;

        public bool IsDown { get; private set; }
        public bool IsDragging { get; private set; }

        public event EventHandler OnModeChange;
        //public event EventHandler OnDisplayModeChange;
        public event EventHandler OnSelectionChange;

        private readonly Highlight _highlight = new Highlight();
        public HighlightSet SelBegin { get; } = new HighlightSet();
        public HighlightSet SelCurrent { get; } = new HighlightSet();
        public HighlightSet SelHighlight { get; } = new HighlightSet();
        public HighlightSet SelSelection { get; } = new HighlightSet();

        public bool LockBasisOnDrag { get; set; }
        public bool LockTicksOnDrag { get; set; }
        public bool LockUnitRatio { get; set; }
        public bool DoSyncMatchingBasis { get; set; } = true;

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

        private SKPoint _rawMousePoint;
        private float _minDragDistance = 4f;
        public float ScaleTickSize { get; set; } = 0.2f;
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

        private Dictionary<int, Range> SavedNumbers { get; } = new Dictionary<int, Range>();

        public MouseAgent(Workspace workspace, Runner runner, CoreRenderer renderer) : base(workspace)
        {
            Renderer = renderer;
            Renderer.Agent = this;
            Runner = runner;
            Runner.Agent = this;
            Program = new Format.Program(Brain, Renderer);

            ClearMouse();
        }

        public void NextTest()
        {
	        Program.NextTest(this);
        }

        public void Draw()
        {
	        var sel = SelHighlight;
	        if (sel.HasHighlight)
	        {
		        var pen = sel.ActiveHighlight.Kind.IsLine() ? Renderer.Pens.HighlightPen : Renderer.Pens.HoverPen;
		        Renderer.Canvas.DrawPath(sel.ActiveHighlight.HighlightPath(), pen);
	        }
        }

        public SKPoint GetTransformedPoint(SKPoint point) => point;// Data.Matrix.Invert().MapPoint(point);
        public bool MouseDown(MouseEventArgs e)
        {
            if(IsPaused){return false;}

            // Add to selection if ctrl down etc.
            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
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
            return true;
        }

        public bool MouseMove(MouseEventArgs e)
        {
	        if (IsPaused) {return false;}

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
        public bool MouseDrag(SKPoint mousePoint)
        {
	        if (IsPaused) {return false;}

            if (SelBegin.HasHighlight && !IsDragging)
            {
	            var dist = (mousePoint - SelBegin.Position).Length;
                if (dist > _minDragDistance)
                {
                    IsDragging = true;
					SelCurrent.Set(_highlight.Clone());
					if (SelCurrent.ActiveHighlight.Mapper is SKNumberMapper nm)
					{
						SelBegin.OriginalSegment = nm.Guideline.Clone();
						SelBegin.OriginalFocalPositions = nm.Number.Focal.FocalPositions;
						if (nm.IsBasis)
						{
							SaveNumberValues(SavedNumbers);
                        }
					}
                }
            }

            if (IsDragging)
            {
                var activeHighlight = SelCurrent.ActiveHighlight;
	            var activeKind = activeHighlight.Kind;
	            if (activeHighlight.Mapper is SKNumberMapper nm)
	            { 
		            if (activeKind.IsLine())
		            {
			            if (nm.IsBasis)
			            {
				            var curT = nm.DomainMapper.Guideline.TFromPoint(_highlight.OrginalPoint, false).Item1;
                            var orgT = nm.DomainMapper.Guideline.TFromPoint(activeHighlight.OrginalPoint, false).Item1;
				            nm.MoveBasisSegmentByT(SelBegin.OriginalSegment, curT - orgT);
				            BasisChanged(nm);
                        }
			            else
			            {
				            var curT = nm.DomainMapper.BasisSegment.TFromPoint(_highlight.OrginalPoint, false).Item1;
                            nm.MoveSegmentByT(SelBegin.OriginalSegment, curT - activeHighlight.T);
                        }
		            }
		            else if (activeKind.IsBasis())
		            {
			            nm.SetValueByKind(_highlight.SnapPoint, activeKind);
                        BasisChanged(nm);
		            }
		            else
		            {
			            nm.SetValueByKind(_highlight.SnapPoint, activeKind);
                    }
	            }
                else if (activeHighlight.Mapper is SKDomainMapper dm)
	            {
		            if (activeKind.IsDomainPoint())
		            {
			            dm.SetValueByKind(_highlight.SnapPoint, activeKind);
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
	        if (IsPaused) {return false;}

            _rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);

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

            OnSelectionChange?.Invoke(this, new EventArgs());
            ClearMouse();

            return true;
        }
        public bool MouseDoubleClick(MouseEventArgs e)
        {
	        if (IsPaused) {return false;}

            if (CurrentKey == Keys.Space && e.Button == MouseButtons.Left)
            {
                //Data.ResetZoom();
            }
            return true;
        }
        public bool MouseWheel(MouseEventArgs e)
        {
	        if (IsPaused) {return false;}

            var scale = 1f + (Math.Sign(e.Delta) * ScaleTickSize);
            var rawMousePoint = e.Location.ToSKPoint();
            var mousePoint = GetTransformedPoint(_rawMousePoint);
            //Data.SetPanAndZoom(Data.Matrix, mousePoint, new SKPoint(0, 0), scale);
            return true;
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


        private void BasisChanged(SKNumberMapper nm)
        {
	        LockBasisOnDrag = _isControlDown;
	        if (LockBasisOnDrag)
	        {
		        nm.AdjustBySegmentChange(SelBegin);
	        }

	        SyncMatchingBasis(nm.DomainMapper, nm.Number.BasisFocal);
        }
        private void FlipBasis()
        {
	        var dm = WorkspaceMapper.DomainMapperByIndex(0);
            var domain = dm.Domain;
	        domain.BasisFocal.FlipAroundStartPoint();
            dm.BasisSegment.FlipAroundStartPoint();
            SyncMatchingBasis(dm, domain.BasisFocal);
        }

        private void SyncMatchingBasis(SKDomainMapper domainMapper, IFocal focal)
        {
	        if (DoSyncMatchingBasis)
	        {
		        var nbRange = domainMapper.UnitRangeOnDomainLine;
		        foreach (var sibDomain in Workspace.ActiveSiblingDomains(domainMapper.Domain))
		        {
			        if (sibDomain.BasisFocalId == focal.Id)
			        {
				        WorkspaceMapper.DomainMapper(sibDomain.Id).UnitRangeOnDomainLine = nbRange;
			        }
		        }
	        }
        }
        private bool _isControlDown;
        private bool _isShiftDown;
        private bool _isAltDown;
        private UIMode PreviousMode = UIMode.Any;
        //private SKMatrix _startMatrix;
        private KeyEventArgs _lastKeyUp;

        public bool KeyDown(KeyEventArgs e)
        {
	        if (CurrentKey == Keys.Escape)
	        {
		        IsPaused = !IsPaused;
	        }
	        if (IsPaused) {return false;}

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
	                LockBasisOnDrag = true;
	                break;
                case Keys.F:
	                WorkspaceMapper.ShowFractions = !WorkspaceMapper.ShowFractions;
	                break;
                case Keys.W:
	                LockTicksOnDrag = true;
	                break;
                case Keys.T:
	                Program.NextTest(this);
	                break;
                case Keys.N:
	                FlipBasis();
	                break;
                case Keys.P:
	                Runner.TogglePause();
                    break;
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
                LockBasisOnDrag = false;
                LockTicksOnDrag = false;
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
            SavedNumbers.Clear();
            SetSelectable(UIMode);
            if (Workspace != null)
            {
	            SelBegin?.Reset();
	            SelCurrent?.Reset();
            }
        }
        public void ClearHighlights()
        {
	        SelBegin.Clear();
	        SelCurrent.Clear();
	        SelHighlight.Clear();
	        SelSelection.Clear();
        }
        public override void ClearAll()
        {
            base.ClearAll();
	        ClearMouse();
	        ClearHighlights();
	        foreach (var workspaceMapper in WorkspaceMappers.Values)
	        {
		        workspaceMapper.ClearAll();
	        }
	        WorkspaceMappers.Clear();
            Brain.ClearAll();
        }

        public void SaveNumberValues(Dictionary<int, Range> numValues, params int[] ignoreIds)
        {
	        numValues.Clear();
	        foreach (var kvp in Brain.NumberStore)
	        {
		        if (!ignoreIds.Contains(kvp.Key))
		        {
			        numValues.Add(kvp.Key, kvp.Value.Value);
		        }
	        }
        }
        public void RestoreNumberValues(Dictionary<int, Range> numValues, params int[] ignoreIds)
        {
	        foreach (var kvp in numValues)
	        {
		        var id = kvp.Key;
		        var storedValue = kvp.Value;
		        if (!ignoreIds.Contains(id))
		        {
			        Brain.NumberStore[id].Value = storedValue;
		        }
	        }
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
