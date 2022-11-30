using System.Collections.Generic;
using System.Linq;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	public class Workspace : IMathElement
    {
	    private static int _idCounter = 0;
	    public MathElementKind Kind => MathElementKind.Transform;
	    public int Id { get; }
	    public int CreationIndex => Id - (int)Kind;

        public Brain Brain { get; }

        private HashSet<int> ActiveIds { get; } = new HashSet<int>();
        public int ActiveElementCount => ActiveIds.Count;

        public bool IsActive { get; set; } = true;

        public Workspace(Brain brain)
        {
	        Brain = brain;
	        Id = _idCounter++;
            Brain.Workspaces.Add(this);
        }

        public bool IsElementActive(int id) => ActiveIds.Contains(id);
        public bool IsElementActive(IMathElement element) => ActiveIds.Contains(element.Id);

        public void AddElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Add(element.Id);
	        }
        }
        public void RemoveElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Remove(element.Id);
	        }
        }
        public void AddElementsById(params int[] elementIds)
        {
	        foreach (var id in elementIds)
	        {

		        ActiveIds.Add(id);
	        }
        }
        public void RemoveElementsById(params int[] elementIds)
        {
	        foreach (var id in elementIds)
	        {
		        ActiveIds.Remove(id);
	        }
        }

        public void AddTraits(bool includeChildren, params Trait[] traits)
        {
	        foreach (var trait in traits)
	        {
		        AddElementsById(trait.Id);
		        if (includeChildren)
		        {
			        AddDomains(includeChildren,trait.DomainStore.Values.ToArray());
		        }
	        }
        }
        public void RemoveTraits(bool includeChildren, params Trait[] traits)
        {
	        foreach (var trait in traits)
	        {
		        RemoveElementsById(trait.Id);
		        if (includeChildren)
		        {
			        RemoveDomains(includeChildren, trait.DomainStore.Values.ToArray());
		        }
	        }
        }

        public void AddDomains(bool includeChildren, params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        AddElementsById(domain.Id);
		        if (includeChildren)
		        {
			        AddElementsById(domain.NumberIds.ToArray());
		        }
	        }
        }
        public void RemoveDomains(bool includeChildren, params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        RemoveElementsById(domain.Id);
		        if (includeChildren)
		        {
			        RemoveElementsById(domain.NumberIds.ToArray());
		        }
	        }
        }

        public IEnumerable<Domain> ActiveSiblingDomains(Domain domain)
        {
	        var ds = domain.Trait.DomainStore;
	        var domainIds = ds.Keys.Where(key => ActiveIds.Contains(key));
	        foreach (var id in domainIds)
	        {
		        if (id != domain.Id)
		        {
			        yield return ds[id];
		        }
	        }
        }

        public void ClearAll()
        {
            ActiveIds.Clear();
        }

    }
}
