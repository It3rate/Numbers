using System.Reflection.Emit;
using Numbers.Core;
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
	    private static int _idCounter = 1;
	    public int Id { get; }

        private List<int> ActiveIds { get; } = new List<int>();

	    public readonly Brain MyBrain;
        public bool IsActive { get; set; } = true;

        //private List<int> ActiveDomainIds { get; } = new List<int>();
        //   private List<int> ActiveNumberIds { get; } = new List<int>();
        //private List<int> ActiveTransformIds { get; } = new List<int>();

        public HighlightSet SelBegin = new HighlightSet();
        public HighlightSet SelCurrent = new HighlightSet();
        public HighlightSet SelHighlight = new HighlightSet();
        public HighlightSet SelSelection = new HighlightSet();

        public Stack<Selection> SelectionStack { get; } = new Stack<Selection>();
	    public Stack<Formula> FormulaStack { get; } = new Stack<Formula>();
        public Stack<Number> ResultStack { get; } = new Stack<Number>(); // can have multiple results?

        public Workspace(Brain brain)
        {
	        Id = _idCounter++;
            MyBrain = brain;
            MyBrain.Workspaces.Add(this);
        }

        public void ClearAll()
        {
	        MyBrain.ClearAll();
	        SelBegin.Clear();
	        SelCurrent.Clear();
	        SelHighlight.Clear();
            SelSelection.Clear();
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
		        ActiveIds.Add(domain.UnitFocalId);
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
		        ActiveIds.Remove(domain.UnitFocalId);
	        }
        }
        public void RemoveElements(params IMathElement[] elements)
        {
	        foreach (var element in elements)
	        {
		        ActiveIds.Remove(element.Id);
	        }
        }

    }
}
