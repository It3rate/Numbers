using Numbers.Core;

namespace Numbers.Mind
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
     
    // Brain -> Traits, transforms

    public class Brain
    {
	    public static Brain BrainA = new Brain();
	    public static Brain BrainB = new Brain();

        public List<Workspace> Workspaces { get; } = new List<Workspace>();

        public Trait ValueTrait { get; private set; }
	    public Dictionary<int, Network> NetworkStore { get; } = new Dictionary<int, Network>();
        public Dictionary<int, Formula> FormulaStore { get; } = new Dictionary<int, Formula>();
	    public Dictionary<int, Trait> TraitStore { get; } = new Dictionary<int, Trait>();
	    public Dictionary<int, Transform> TransformStore { get; } = new Dictionary<int, Transform>();

	    private int traitCounter = 1 + (int)MathElementKind.Trait;
	    public int NextTraitId() => traitCounter++;

	    public void ClearAll()
	    {
            //NetworkStore.Clear();
            //FormulaStore.Clear();
            TraitStore.Clear();
            TransformStore.Clear();
	    }
    }
}
