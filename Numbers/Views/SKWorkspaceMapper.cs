using System.Collections.Generic;
using System.Linq;
using Numbers.Core;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Views
{
	public class SKWorkspaceMapper
    {
	    public int Id => Workspace.Id;

	    public Workspace Workspace { get; set; }
	    public Brain MyBrain => Workspace.MyBrain;
        public readonly CoreRenderer Renderer;
        public SKPoint TopLeft { get; set; }
	    public SKPoint BottomRight { get; set; }

	    public Dictionary<int, SKMapper> Mappers = new Dictionary<int, SKMapper>();
	    public SKDomainMapper DomainMapper(int id) => (SKDomainMapper)Mappers[id]; // todo: slow to get domain from domain id (nested in traits).
	    public SKTransformMapper TransformMapper(int id) => GetOrCreateTransformMapper(id);
	    public SKNumberMapper NumberMapper(int id) => GetOrCreateNumberMapper(id);

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

        public SKWorkspaceMapper(Workspace workspace, CoreRenderer renderer, float left, float top, float width, float height)
        {
            Workspace = workspace;
            Renderer = renderer;
            Renderer.Workspaces.Add(Workspace);

            TopLeft = new SKPoint(left, top);
		    BottomRight = new SKPoint(left + width, top + height);
            MyBrain.WorkspaceMappers.Add(Id, this);
	    }

        public Highlight GetSnapPoint(Highlight highlight, HighlightSet ignoreSet, SKPoint input, float maxDist = SnapDistance * 2f)
        {
            highlight.Reset();
            highlight.OrginalPoint = input;
            // number segments and units
            foreach (var nm in NumberMappersByDomain(true))
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
                }
            }

            foreach (var dm in DomainMappers())
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
                        var (t, _) = dm.DisplayLine.TFromPoint(dmTickPoint, false);
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
	            foreach (var transformMapper in TransformMappers())
		        {
			        transformMapper.Draw();
		        }
		        foreach (var domainMapper in DomainMappers())
		        {
			        domainMapper.Draw();
		        }
	        }
        }

        public IEnumerable<SKTransformMapper> TransformMappers(bool reverse = false)
        {
	        var vals = reverse ? Mappers.Values.Reverse() : Mappers.Values;
	        foreach (var mapper in vals)
	        {
		        if (mapper is SKTransformMapper tm)
		        {
			        yield return tm;
		        }
	        }
        }
        public IEnumerable<SKDomainMapper> DomainMappers(bool reverse = false)
        {
	        var vals = reverse ? Mappers.Values.Reverse() : Mappers.Values;
	        foreach (var mapper in vals)
	        {
		        if (mapper is SKDomainMapper dm)
		        {
			        yield return dm;
		        }
	        }
        }
        public IEnumerable<SKNumberMapper> AllNumberMappers()
        {
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKNumberMapper nm)
		        {
			        yield return nm;
                }
	        }
        }
        public IEnumerable<SKNumberMapper> NumberMappersByDomain(bool reverse = false)
        {
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKDomainMapper dm)
		        {
			        var ids = dm.Domain.NumberIds;
			        if (reverse)
			        {
				        for (int i = ids.Count - 1; i >= 0; i--)
				        {
					        yield return NumberMapper(ids[i]);
				        }
			        }
			        else
			        {
				        foreach (var numId in ids)
				        {
					        yield return NumberMapper(numId);
				        }
			        }
		        }
	        }
        }
        public IEnumerable<SKMapper> MappersOfKind(MathElementKind kind)
        {
	        var values = Mappers.Where(kvp => kvp.Value.MathElement.Kind == kind);
	        foreach (var kvp in values)
	        {
		        yield return kvp.Value;
	        }
        }
        public IEnumerable<SKMapper> MappersOfKindReversed(MathElementKind kind)
        {
	        var values = Mappers.Where(kvp => kvp.Value.MathElement.Kind == kind).Reverse();
	        foreach (var kvp in values)
	        {
		        yield return kvp.Value;
	        }
        }

        public SKDomainMapper DomainMapperByIndex(int index)
        {
	        SKDomainMapper result = null;
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKDomainMapper dm)
		        {
			        index--;
			        if (index < 0)
			        {
				        result = dm;
				        break;
			        }
		        }
	        }
	        return result;
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
            //var cx = Renderer.Width / 2f - 100;
            //var cy = Renderer.Height / 2f;
            //var armLen = 280;
            //// all this etc will be a workspace element eventually
            //var lines = new[] { new SKSegment(cx - armLen, cy, cx + armLen, cy), new SKSegment(cx, cy + armLen, cx, cy - armLen) };

            foreach (var trait in MyBrain.TraitStore.Values)
	        {
		        int index = 0;
		        foreach (var domain in trait.DomainStore.Values)
		        {
			        GetOrCreateDomainMapper(domain);
			        foreach (var number in domain.Numbers())
			        {
				        GetOrCreateNumberMapper(number);
			        }
			        index++;
		        }
		        foreach (var transform in trait.TransformStore.Values)
		        {
			        GetOrCreateTransformMapper(transform);
		        }
	        }
        }

        public SKDomainMapper GetOrCreateDomainMapper(Domain domain, SKSegment line = null, SKSegment unitLine = null)
        {
	        if (!Mappers.TryGetValue(domain.Id, out var result))
	        {
		        var seg = line ?? NextDefaultLine();
		        var uSeg = unitLine ?? line.SegmentAlongLine(.45f, .55f);
		        result = new SKDomainMapper(Workspace, domain, seg, uSeg);
		        Mappers[domain.Id] = result;
	        }
	        return (SKDomainMapper)result;
        }
        public SKNumberMapper GetOrCreateNumberMapper(int id)
        {
	        return GetOrCreateNumberMapper(MyBrain.NumberStore[id]);
        }
        public SKNumberMapper GetOrCreateNumberMapper(Number number)
        {
            if (!Mappers.TryGetValue(number.Id, out var result))
	        {
		        result = new SKNumberMapper(Workspace, number);
		        Mappers[number.Id] = result;
	        }

	        return (SKNumberMapper)result;
        }
        public SKTransformMapper GetOrCreateTransformMapper(int id)
        {
	        return GetOrCreateTransformMapper(MyBrain.TransformStore[id]);
        }
        public SKTransformMapper GetOrCreateTransformMapper(Transform transform)
        {
	        if (!Mappers.TryGetValue(transform.Id, out var result))
	        {
		        result = new SKTransformMapper(Workspace, transform);
		        Mappers[transform.Id] = result;
	        }
	        return (SKTransformMapper)result;
        }

        public void ClearAll()
        {
	        Mappers.Clear();
	        defaultLineT = 0.1f;
        }
    }
}
