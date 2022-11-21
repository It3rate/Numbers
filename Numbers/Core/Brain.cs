using System.Collections.Generic;
using Numbers.Views;

namespace Numbers.Core
{
	// Brain -> Traits, transforms

    public class Brain
    {
	    public static Brain ActiveBrain => BrainA;
	    public static Brain BrainA = new Brain();
	    public static Brain BrainB = new Brain();

        public Trait ValueTrait { get; private set; }
        public List<Workspace> Workspaces { get; } = new List<Workspace>();
	    public Dictionary<int, Network> NetworkStore { get; } = new Dictionary<int, Network>();
        public Dictionary<int, Formula> FormulaStore { get; } = new Dictionary<int, Formula>();
	    public Dictionary<int, Trait> TraitStore { get; } = new Dictionary<int, Trait>();
	    public Dictionary<int, Transform> TransformStore { get; } = new Dictionary<int, Transform>();
	    public Dictionary<int, Number> NumberStore { get; } = new Dictionary<int, Number>();

        public Dictionary<int, SKWorkspaceMapper> WorkspaceMappers = new Dictionary<int, SKWorkspaceMapper>(); // todo: Move all mappers to SK side

	    private int traitCounter = 1 + (int)MathElementKind.Trait;
	    public int NextTraitId() => traitCounter++;

	    public void ClearAll()
	    {
		    foreach (var workspaceMapper in WorkspaceMappers.Values)
		    {
			    workspaceMapper.ClearAll();
		    }
		    WorkspaceMappers.Clear();

            foreach (var workspace in Workspaces)
            {
                workspace.ClearAll();
            }
            Workspaces.Clear();


            NetworkStore.Clear();
            FormulaStore.Clear();
            TraitStore.Clear();
            TransformStore.Clear();
            NumberStore.Clear();
        }

    }
}
