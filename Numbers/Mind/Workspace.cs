﻿using Numbers.Core;
using Numbers.Renderer;
using Numbers.UI;
using SkiaSharp;

namespace Numbers.Mind
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Workspace
    {
	    private List<int> ActiveIds { get; } = new List<int>();

	    public readonly Brain MyBrain;
	    public readonly RendererBase Renderer;
        public bool IsActive { get; set; } = true;

	    //public Dictionary<int, TransformRenderer> TransformRenderers = new Dictionary<int, TransformRenderer>();
	    public Dictionary<int, SKMapper> Mappers = new Dictionary<int, SKMapper>();
	    public SKDomainMapper DomainMapper(int id) => (SKDomainMapper)Mappers[id];
	    public SKTransformMapper TransformMapper(int id) => (SKTransformMapper)Mappers[id];
	    public SKNumberMapper NumberMapper(int id) => (SKNumberMapper)Mappers[id];


        //private List<int> ActiveDomainIds { get; } = new List<int>();
        //   private List<int> ActiveNumberIds { get; } = new List<int>();
        //private List<int> ActiveTransformIds { get; } = new List<int>();

        public Stack<Selection> SelectionStack { get; } = new Stack<Selection>();
	    public Stack<Formula> FormulaStack { get; } = new Stack<Formula>();
        public Stack<Number> ResultStack { get; } = new Stack<Number>(); // can have multiple results?

        public Workspace(Brain brain, RendererBase renderer)
        {
	        MyBrain = brain;
	        Renderer = renderer;
        }

        public IEnumerable<SKTransformMapper> TransformMappers()
        {
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKTransformMapper)
		        {
			        yield return (SKTransformMapper)mapper;
		        }
	        }
        }
        public IEnumerable<SKDomainMapper> DomainMappers()
        {
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKDomainMapper)
		        {
			        yield return (SKDomainMapper)mapper;
		        }
	        }
        }
        public IEnumerable<SKNumberMapper> NumberMappers()
        {
	        foreach (var mapper in Mappers.Values)
	        {
		        if (mapper is SKDomainMapper dm)
		        {
			        foreach (var numId in dm.Domain.NumberIds)
			        {
				        yield return NumberMapper(numId);
			        }
		        }
	        }
        }

        public Highlight HoverHighlight { get; set; } = new Highlight();
        public const float SnapDistance = 5.0f;
        public Highlight GetSnapPoint(SKPoint input, float maxDist = SnapDistance * 2f)
        {
	        HoverHighlight.Reset();
	        foreach (var nm in NumberMappers())
	        {
		        if (input.DistanceTo(nm.StartPoint) < maxDist)
		        {
			        HoverHighlight.Set(input, nm, 0);
			        break;
		        }
		        else if (input.DistanceTo(nm.EndPoint) < maxDist)
		        {
			        HoverHighlight.Set(input, nm, 1);
			        break;
		        }
            }
	        return HoverHighlight;
        }

        public void Draw()
        {
	        foreach (var transformMapper in TransformMappers())
	        {
		        transformMapper.Draw();
	        }
	        foreach (var domainMapper in DomainMappers())
	        {
		        domainMapper.Draw();
	        }

	        if (HoverHighlight.IsSet)
	        {
                Renderer.Canvas.DrawPath(HoverHighlight.HighlightPath(), Renderer.Pens.HoverPen);
	        }
        }

        public void AddElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Add(element.Id);
	        }
        }
        public void AddFullDomains(params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        ActiveIds.Add(domain.Id);
		        ActiveIds.AddRange(domain.NumberIds);
		        ActiveIds.Add(domain.UnitId);
	        }
        }
        public void RemoveFullDomains(params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        ActiveIds.Remove(domain.Id);
		        foreach (var numberId in domain.NumberIds)
		        {
			        ActiveIds.Remove(numberId);
		        }
		        ActiveIds.Remove(domain.UnitId);
	        }
        }
        public void RemoveElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Remove(element.Id);
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

        public SKDomainMapper GetOrCreateDomainMapper(Domain domain, SKPoint startPoint, SKPoint endPoint)
        {
	        if (!Mappers.TryGetValue(domain.Id, out var result))
	        {
		        result = new SKDomainMapper(this, domain, startPoint, endPoint);// DomainRenderer(this, domain, startPoint, endPoint);
		        Mappers[domain.Id] = result;
	        }
	        return (SKDomainMapper)result;
        }
        public SKNumberMapper GetOrCreateNumberMapper(Number number)
        {
	        if (!Mappers.TryGetValue(number.Id, out var result))
	        {
		        result = new SKNumberMapper(this, number); // DomainRenderer(this, domain, startPoint, endPoint);
		        Mappers[number.Id] = result;
	        }

	        return (SKNumberMapper)result;
        }
        public SKTransformMapper GetOrCreateTransformMapper(Transform transform)
        {
	        if (!Mappers.TryGetValue(transform.Id, out var result))
	        {
		        result = new SKTransformMapper(this, transform);
		        Mappers[transform.Id] = result;
	        }
	        return (SKTransformMapper)result;
        }

        public void EnsureRenderers()
        {
	        var cx = Renderer.Width / 2f - 100;
	        var cy = Renderer.Height / 2f;
	        var armLen = 280;
	        // all this etc will be a workspace element eventually
	        var lines = new[] { new SKSegment(cx - armLen, cy, cx + armLen, cy), new SKSegment(cx, cy + armLen, cx, cy - armLen) };

	        foreach (var trait in MyBrain.TraitStore.Values)
	        {
		        int index = 0;
		        foreach (var domain in trait.DomainStore.Values)
		        {
			        GetOrCreateDomainMapper(domain, lines[index].StartPoint, lines[index].EndPoint);
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
    }
}
