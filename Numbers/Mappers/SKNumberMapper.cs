using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows.Forms.VisualStyles;
using Numbers.Agent;
using Numbers.Drawing;
using Numbers.Renderer;
using NumbersCore.Primitives;
using OpenTK.Audio.OpenAL;
using SkiaSharp;

namespace Numbers.Mappers
{
    public class SKNumberMapper : SKMapper
    {
        public Number Number => (Number)MathElement;
        public MouseAgent MouseAgent => (MouseAgent)Agent;
        public virtual SKSegment RenderSegment { get; set; }

        public SKDomainMapper DomainMapper => WorkspaceMapper.GetDomainMapper(Number.Domain);
        public SKSegment UnitSegment => DomainMapper.BasisSegment;
        public bool IsBasis => Number.IsBasis;
        public int BasisSign => Number.BasisFocal.Direction;
        public SKSegment GetBasisSegment() => DomainMapper.BasisSegmentForNumber(Number);
        public Polarity Polarity { get => Number.Polarity; set => Number.Polarity = value; }
        public int UnitDirectionOnDomainLine => Guideline.DirectionOnLine(DomainMapper.Guideline);

        public int OrderIndex { get; set; } = -1;

        public SKNumberMapper(MouseAgent agent, Number number) : base(agent, number)
        {
            Id = number.Id;
        }


        public Polarity InvertPolarity()
        {
            return Number.InvertPolarity();
        }
        public void ResetNumber(Number number) => MathElement = number;
        public void EnsureSegment()
        {
            var val = Number.ValueInRenderPerspective;
            Reset(UnitSegment.SegmentAlongLine(val.StartF, val.EndF));
        }

        public event EventHandler OnSelected;
        public void Select()
        {
            OnSelected?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler OnDeselected;
        public void Deselect()
        {
            OnDeselected?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler OnChanged;
        public void Changed()
        {
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void Draw() { }
        public void DrawNumber(float offset, SKPaint paint, SKPaint invertPaint = null)
        {
			EnsureSegment();
            var pen2 = invertPaint ?? paint;
            if (DomainMapper.ShowSeparatedSegment)
            {
                var val = Number.ValueInRenderPerspective;
                var segDir = val.EndF >= 0 ? 1 : -1;
                var endSeg = UnitSegment.SegmentAlongLine(0, val.EndF).ShiftOffLine((offset + 6) * segDir);
                Renderer.DrawFromZeroHalfLine(endSeg, paint);

                segDir = val.StartF >= 0 ? 1 : -1;
                var startSeg = UnitSegment.SegmentAlongLine(0, val.StartF).ShiftOffLine((offset + 6) * segDir);
                Renderer.DrawFromZeroHalfLine(startSeg, pen2);

                RenderSegment = new SKSegment(startSeg.EndPoint, endSeg.EndPoint);
            }
            else
            {
			    var dir = UnitDirectionOnDomainLine;
                RenderSegment = Guideline.ShiftOffLine(offset * dir);
                Renderer.DrawDirectedLine(RenderSegment, paint, pen2);
            }
        }

        public void DrawUnit(bool aboveLine, bool showPolarity)
        {
            // BasisNumber is a special case where we don't want it's direction set by the unit direction of the line (itself).
            // So don't call EnsureSegment here.
            var dir = Number.Focal.Direction;
            var unitPen = Pens.UnitPenLight; // the basis defines the unit direction, so it is always unit color
	        var offset = Guideline.OffsetAlongLine(0,  unitPen.StrokeWidth / 2f * dir) - Guideline.StartPoint;
	        RenderSegment = aboveLine ? Guideline + offset : Guideline - offset;
	        if (Pens.UnitStrokePen != null)
	        {
		        Renderer.DrawSegment(RenderSegment, Pens.UnitStrokePen);
            }
            Renderer.DrawSegment(RenderSegment, unitPen);
            if (showPolarity)
            {
                var unotSeg = RenderSegment.Clone();
                unotSeg.FlipAroundStartPoint();
                Renderer.DrawSegment(unotSeg, Pens.UnotPenLight);
            }
        }

        public float TFromPoint(SKPoint point)
        {
	        var basisSeg = GetBasisSegment();
	        var pt = basisSeg.ProjectPointOnto(point, false);
            var (t, _) = basisSeg.TFromPoint(pt, false);
	        t = (float)(Math.Round(t * basisSeg.Length) / basisSeg.Length);
	        return t;
        }

        public void AdjustBySegmentChange(HighlightSet beginState) => AdjustBySegmentChange(beginState.OriginalSegment, beginState.OriginalFocal);
        public void AdjustBySegmentChange(SKSegment originalSegment, Focal originalFocal)
        {
	        var change = originalSegment.RatiosAsBasis(Guideline);
	        var ofp = originalFocal;
	        Number.Focal.Reset(
		        (long)(ofp.StartPosition + change.Start * ofp.LengthInTicks),
		        (long)(ofp.EndPosition + (change.End - 1.0) * ofp.LengthInTicks));
        }

        public void SetValueByKind(SKPoint newPoint, UIKind kind)
        {
	        if (kind.IsBasis())
	        {
		        SetValueOfBasis(newPoint, kind);
	        }
	        else if (kind.IsMajor())
            {
	            SetEndValueByPoint(newPoint);
            }
	        else
            {
	            SetStartValueByPoint(newPoint);
            }
        }

        public void MoveSegmentByT(SKSegment orgSeg, float diffT)
        {
            var basisSeg = GetBasisSegment();
	        var orgStartT = -basisSeg.TFromPoint(orgSeg.StartPoint, false).Item1;
	        var orgEndT = basisSeg.TFromPoint(orgSeg.EndPoint, false).Item1;
	        Number.StartValue = orgStartT - diffT;
	        Number.EndValue = orgEndT + diffT;
        }
        public void MoveBasisSegmentByT(SKSegment orgSeg, float diffT)
        {
	        var dl = DomainMapper.Guideline;
	        var orgStartT = dl.TFromPoint(orgSeg.StartPoint, false).Item1;
	        var orgEndT = dl.TFromPoint(orgSeg.EndPoint, false).Item1;
	        Guideline.StartPoint = dl.PointAlongLine(orgStartT + diffT);
	        Guideline.EndPoint = dl.PointAlongLine(orgEndT + diffT);
        }

        public void SetStartValueByPoint(SKPoint newPoint)
        {
            Number.StartValue = -TFromPoint(newPoint);
        }
        public void SetEndValueByPoint(SKPoint newPoint)
        {
            Number.EndValue = TFromPoint(newPoint);
        }
        public void SetValueOfBasis(SKPoint newPoint, UIKind kind)
        {
	        var pt = DomainMapper.Guideline.ProjectPointOnto(newPoint);
	        if (kind.IsMajor())
	        {
		        Guideline.EndPoint = pt;
	        }
	        else
	        {
		        Guideline.StartPoint = pt;
	        }
        }

        public override SKPath GetHighlightAt(Highlight highlight)
        {
	        SKPath result;
	        if (highlight.Kind.IsLine())
	        {
		        result = Renderer.GetSegmentPath(RenderSegment, 0f);
	        }
	        else
	        {
		        result = Renderer.GetCirclePath(highlight.SnapPoint);
            }
	        return result;
        }
        public override string ToString()
        {
            return "nm:" + Number.ToString();
        }
    }
}
