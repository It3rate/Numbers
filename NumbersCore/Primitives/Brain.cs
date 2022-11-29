using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
	// Brain -> Traits, transforms

    public class Brain
    {
	    public static List<Brain> Brains { get; } = new List<Brain>();
	    public static Brain ActiveBrain => BrainA;
	    public static Brain BrainA = new Brain();
	    public static Brain BrainB = new Brain();
	    public Brain()
	    {
		    Brains.Add(this);
	    }

	    public List<Workspace> Workspaces { get; } = new List<Workspace>();

        public Dictionary<int, Network> NetworkStore { get; } = new Dictionary<int, Network>();
        public Dictionary<int, Formula> FormulaStore { get; } = new Dictionary<int, Formula>();
        public Dictionary<int, Definition> DefinitionStore { get; } = new Dictionary<int, Definition>();
        public Dictionary<int, Trait> TraitStore { get; } = new Dictionary<int, Trait>();
	    public Dictionary<int, Transform> TransformStore { get; } = new Dictionary<int, Transform>();
	    public Dictionary<int, Number> NumberStore { get; } = new Dictionary<int, Number>();

	    private int _networkCounter = 1 + (int)MathElementKind.Network;
	    private int _formulaCounter = 1 + (int)MathElementKind.Formula;
	    private int _definitionCounter = 1 + (int)MathElementKind.Definition;
        private int _traitCounter = 1 + (int)MathElementKind.Trait;
	    private int _transformCounter = 1 + (int)MathElementKind.Transform;
	    private int _numberCounter = 1 + (int)MathElementKind.Number;
        public int NextNetworkId() => _networkCounter++;
        public int NextFormulaId() => _formulaCounter++;
        public int NextDefinitionId() => _definitionCounter++;
        public int NextTraitId() => _traitCounter++;
	    public int NextTransformId() => _transformCounter++;
	    public int NextNumberId() => _numberCounter++;

        // specialized traits
        public Trait ValueTrait { get; private set; }
	    public Trait CounterTrait { get; private set; }

        public void ClearAll()
	    {
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


        // Eventually these will all be loaded into the brain. They are innate properties, equivalent to the ones we're born with.
        private Trait _timeTrait;
        public Trait TimeTrait => _timeTrait ?? (_timeTrait = new Trait(this, "Timer"));
        private Domain _time60Domain;
        public Domain Time60Domain => _time60Domain ?? (_time60Domain = TimeTrait.AddDomain(60));
        private Domain _timeMsDomain;
        public Domain TimeMsDomain => _timeMsDomain ?? (_timeMsDomain = TimeTrait.AddDomain(1000));

        private Trait _countTrait;
        public Trait CountTrait => _countTrait ?? (_countTrait = new Trait(this, "Count"));
        private Domain _intDomain;
        public Domain IntDomain => _intDomain ?? (_intDomain = CountTrait.AddDomain(1));
    }
}
