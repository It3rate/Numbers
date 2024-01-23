using System.Collections.Generic;
using System.Linq;
using Numbers.Agent;
using Numbers.Renderer;
using Numbers.Utils;
using NumbersCore.Primitives;
using NumbersCore.Utils;
//using OpenTK.Graphics.ES30;
using SkiaSharp;

namespace Numbers.Mappers
{
	public class SKWorkspaceMapper : SKMapper
    {
        private readonly Dictionary<int, SKDomainMapper> DomainMappers = new Dictionary<int, SKDomainMapper>();
        private readonly Dictionary<int, SKTransformMapper> TransformMappers = new Dictionary<int, SKTransformMapper>();
        public SKDomainMapper DomainMapper(Domain domain) => GetOrCreateDomainMapper(domain);
	    public SKTransformMapper TransformMapper(Transform transform) => GetOrCreateTransformMapper(transform);

	    public SKPoint TopLeft
	    {
		    get => Guideline.StartPoint;
		    set => Guideline.StartPoint = value;
	    }
	    public SKPoint BottomRight
	    {
		    get => Guideline.EndPoint;
		    set => Guideline.EndPoint = value;
	    }
	    public override SKPath GetHighlightAt(Highlight highlight)
	    {
		    return Renderer.GetRectPath(TopLeft, BottomRight);
	    }

        public SKPoint Center => TopLeft.MidpointTo(BottomRight);
	    public float Width => BottomRight.X - TopLeft.X;
	    public float Height => BottomRight.Y - TopLeft.Y;
        // these are pointing clockwise 
	    public SKSegment TopSegment => new SKSegment(TopLeft.X, TopLeft.Y, BottomRight.X, TopLeft.Y);
	    public SKSegment RightSegment => new SKSegment(BottomRight.X, TopLeft.Y, BottomRight.X, BottomRight.Y);
	    public SKSegment BottomSegment => new SKSegment(BottomRight.X, BottomRight.Y, TopLeft.X, BottomRight.Y);
	    public SKSegment LeftSegment => new SKSegment(TopLeft.X, BottomRight.Y, TopLeft.X, TopLeft.Y);

	    private static float defaultLineT = 0.1f;
        public const float SnapDistance = 5.0f;
        public bool ShowFractions { get; set; } = true;

        public SKWorkspaceMapper(MouseAgent agent, float left, float top, float width, float height) : base(agent, agent.Workspace)
        {
            Id = idCounter++;
            MouseAgent.WorkspaceMappers[Id] = this;
            Reset(new SKPoint(left, top), new SKPoint(left + width, top + height));
	    }

        public SKDomainMapper AddDomain(Domain domain, float offset, bool isHorizontal = true, int margins = 50)
        {
            Agent.Workspace.AddDomains(true, domain);
            var seg = isHorizontal ? GetHorizontalSegment(offset, margins) : GetVerticalSegment(offset, margins);
            var dm = GetOrCreateDomainMapper(domain, seg);
            dm.ShowGradientNumberLine = false;
            dm.ShowValueMarkers = true;
            dm.ShowBasisMarkers = false;
            dm.ShowBasis = false;
            return dm;
        }
        public SKDomainMapper AddHorizontal(Domain domain, int margins = 50)
        {
            return AddDomain(domain, 0.5f, true, margins);
        }
        public SKDomainMapper AddVertical(Domain domain, int margins = 50)
        {
            return AddDomain(domain, 0.5f, false, margins);
        }


        public Highlight GetSnapPoint(Highlight highlight, HighlightSet ignoreSet, SKPoint input, float maxDist = SnapDistance * 2f)
        {
            highlight.Reset();
            highlight.OrginalPoint = input;
            // number segments and units
            foreach (var nm in AllNumberMappers(true))
            {
	            if (nm.RenderSegment != null)
	            {
		            var seg = nm.RenderSegment;
		            var isSameMapper = ignoreSet.ActiveHighlight != null && ignoreSet.ActiveHighlight.Mapper == nm;
		            var kind = UIKind.Number | (nm.IsBasis ? UIKind.Basis : UIKind.None);

		            if (!isSameMapper && input.DistanceTo(seg.StartPoint) < maxDist)
		            {
			            highlight.Set(input, seg.StartPoint, nm, 0, kind | UIKind.Point);
			            goto Found;
		            }

		            if (!isSameMapper && input.DistanceTo(seg.EndPoint) < maxDist)
		            {
			            highlight.Set(input, seg.EndPoint, nm, 1, kind | UIKind.Major | UIKind.Point);
			            goto Found;
		            }

		            if (!isSameMapper && seg.DistanceTo(input, true) < maxDist)
		            {
			            var t = nm.DomainMapper.BasisSegment.TFromPoint(input, false).Item1;
			            highlight.Set(input, input, nm, t, kind | UIKind.Line);
                        goto Found;
                    }
	            }
            }

            foreach (var dm in DomainMappers.Values)
            {
                // Domain segment endpoints
                for (int i = 0; i < dm.EndPoints.Length; i++)
                {
                    var dmPt = dm.EndPoints[i];
                    if (input.DistanceTo(dmPt) < maxDist)
                    {
                        var kind = UIKind.Domain | UIKind.Point | (i > 0 ? UIKind.Major : UIKind.None);
                        highlight.Set(input, dmPt, dm, (float)i, kind);
                        goto Found;
                    }
                }
                // domain number line bold ticks
                foreach (var dmTickPoint in dm.TickPoints)
                {
                    if (input.DistanceTo(dmTickPoint) < maxDist / 2f)
                    {
                        var kind = UIKind.Tick | UIKind.Major;
                        var (t, _) = dm.Guideline.TFromPoint(dmTickPoint, false);
                        highlight.Set(input, dmTickPoint, dm, t, kind);
                        goto Found;
                    }
                }
            }

        Found:
            return highlight;
        }

        public void Draw()
        {
	        EnsureRenderers();
	        if (Workspace.IsActive)
	        {
	            foreach (var transformMapper in TransformMappers.Values)
		        {
			        transformMapper.Draw();
		        }
		        foreach (var domainMapper in DomainMappers.Values)
		        {
			        domainMapper.Draw();
		        }
	        }
        }

        public IEnumerable<SKTransformMapper> GetTransformMappers(bool reverse = false)
        {
	        var vals = reverse ? TransformMappers.Values.Reverse() : TransformMappers.Values;
	        foreach (var mapper in vals)
	        {
		        if (mapper is SKTransformMapper tm)
		        {
			        yield return tm;
		        }
	        }
        }
        public IEnumerable<SKDomainMapper> GetDomainMappers(bool reverse = false)
        {
	        var vals = reverse ? DomainMappers.Values.Reverse() : DomainMappers.Values;
	        foreach (var mapper in vals)
	        {
		        if (mapper is SKDomainMapper dm)
		        {
			        yield return dm;
		        }
	        }
        }
        public SKDomainMapper DomainMapperByIndex(int index)
        {
	        SKDomainMapper result = null;
	        if (index < DomainMappers.Count)
	        {
		        foreach (var dm in DomainMappers.Values)
		        {
			        if (--index < 0)
			        {
				        result = dm;
				        break;
			        }
		        }
	        }
	        return result;
        }

        public IEnumerable<SKNumberMapper> AllNumberMappers(bool reverse = false)
        {
	        foreach (var dm in GetDomainMappers(reverse))
	        {
		        foreach (var nm in dm.GetNumberMappers(reverse))
		        {
			        yield return nm;
		        }
	        }
        }

        public SKSegment GetHorizontalSegment(float t, int margins)
        {
	        var offset = Height * t;
	        var result = TopSegment + new SKPoint(0, offset);
	        return result.InsetSegment(margins);
        }
        public SKSegment GetVerticalSegment(float t, int margins)
        {
	        var offset = Width * t;
	        var result = LeftSegment + new SKPoint(offset, 0);
	        return result.InsetSegment(margins);
        }
        private SKSegment NextDefaultLine()
        {
	        var result = GetHorizontalSegment(defaultLineT, (int)(Width * .9f));
	        defaultLineT += 0.1f;
	        return result;
        }

        public void EnsureRenderers()
        {
            foreach (var trait in Brain.TraitStore.Values)
	        {
		        int index = 0;
		        foreach (var domain in trait.DomainStore.Values)
		        {
			        var dm = GetOrCreateDomainMapper(domain);
			        foreach (var number in domain.Numbers())
			        {
				        var nm = dm.GetOrCreateNumberMapper(number);
			        }
			        index++;
		        }
		        foreach (var transform in Brain.TransformStore.Values)
		        {
			        GetOrCreateTransformMapper(transform);
		        }
	        }
        }

        public SKDomainMapper GetOrCreateDomainMapper(Domain domain, SKSegment line = null, SKSegment unitLine = null)
        {
	        if (!DomainMappers.TryGetValue(domain.Id, out var result))
	        {
		        line = line ?? NextDefaultLine();
                var half_mmr = (float)(1.0 / domain.MinMaxRange.AbsLength());
		        var uSeg = unitLine ?? line.SegmentAlongLine(0.5f, 0.5f + half_mmr);
		        result = new SKDomainMapper(Agent, domain, line, uSeg);
		        DomainMappers[domain.Id] = result;
	        }
	        return (SKDomainMapper)result;
        }

        public bool RemoveDomainMapper(int id) => DomainMappers.Remove(id);

        public SKTransformMapper GetOrCreateTransformMapper(int id)
        {
	        return GetOrCreateTransformMapper(Brain.TransformStore[id]);
        }
        public SKTransformMapper GetOrCreateTransformMapper(Transform transform)
        {
	        if (!TransformMappers.TryGetValue(transform.Id, out var result))
	        {
		        result = new SKTransformMapper(Agent, transform);
		        TransformMappers[transform.Id] = result;
	        }
	        return (SKTransformMapper)result;
        }

        public void ClearAll()
        {
	        DomainMappers.Clear();
	        TransformMappers.Clear();
            defaultLineT = 0.1f;
        }
    }
}
