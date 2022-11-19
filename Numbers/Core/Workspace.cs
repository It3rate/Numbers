using System.Collections.Generic;
using System.Linq;
using Numbers.UI;

namespace Numbers.Core
{
	public class Workspace
    {
	    private static int _idCounter = 1;
	    public int Id { get; }

        public Brain MyBrain => Brain.ActiveBrain;
        private List<int> ActiveIds { get; } = new List<int>();
        public int ActiveElementCount => ActiveIds.Count;

        public bool IsActive { get; set; } = true;

        public Workspace()
        {
	        Id = _idCounter++;
            MyBrain.Workspaces.Add(this);
        }

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

        public void AddTraits(bool includeChildren, params Trait[] traits)
        {
	        foreach (var trait in traits)
	        {
		        ActiveIds.Add(trait.Id);
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
		        ActiveIds.Remove(trait.Id);
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
		        ActiveIds.Add(domain.Id);
		        if (includeChildren)
		        {
			        ActiveIds.AddRange(domain.NumberIds);
		        }
	        }
        }
        public void RemoveDomains(bool includeChildren, params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        ActiveIds.Remove(domain.Id);
		        if (includeChildren)
		        {
			        foreach (var numberId in domain.NumberIds)
			        {
				        ActiveIds.Remove(numberId);
			        }
		        }
	        }
        }

        public void ClearAll()
        {
	        MyBrain.ClearAll();
            ActiveIds.Clear();
        }

    }
}
