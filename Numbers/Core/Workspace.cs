using System.Collections.Generic;
using System.Linq;
using Numbers.UI;

namespace Numbers.Core
{
	public class Workspace
    {
	    private static int _idCounter = 1;
	    public int Id { get; }

        private List<int> ActiveIds { get; } = new List<int>();

	    public readonly Brain MyBrain;
        public bool IsActive { get; set; } = true;

        public static Dictionary<int, Number> NumberStore { get; } = new Dictionary<int, Number>();

        public Workspace(Brain brain)
        {
	        Id = _idCounter++;
            MyBrain = brain;
            MyBrain.Workspaces.Add(this);
        }

        public void ClearAll()
        {
	        MyBrain.ClearAll();
        }

        public void AddElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Add(element.Id);
	        }
        }
        public void AddDomain(Domain domain)
        {
	        ActiveIds.Add(domain.Id);
	        ActiveIds.AddRange(domain.NumberIds);
        }
        public void AddFullDomains(params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        AddDomain(domain);
	        }
        }
        public void RemoveDomain(Domain domain)
        {
	        ActiveIds.Remove(domain.Id);
	        foreach (var numberId in domain.NumberIds)
	        {
		        ActiveIds.Remove(numberId);
	        }
        }
        public void RemoveFullDomains(params Domain[] domains)
        {
	        foreach (var domain in domains)
	        {
		        RemoveDomain(domain);
	        }
        }
        public void RemoveElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Remove(element.Id);
	        }
        }

        public void SaveNumberValues(Dictionary<int, Range> numValues, params int[] ignoreIds)
        {
	        numValues.Clear();
            foreach (var kvp in NumberStore)
            {
	            if(!ignoreIds.Contains(kvp.Key))
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
			        NumberStore[id].Value = storedValue;
		        }
            }
        }
    }
}
