namespace NumbersCore.Utils
{
	public interface IMathElement
    {
	    MathElementKind Kind { get; }
	    int Id { get; }
	    int CreationIndex { get; }
    }

    public enum MathElementKind
    {
	    None = 0,
        Network    = 0x00100000,
        Workspace  = 0x00200000,
        Formula    = 0x00400000,
        Definition = 0x00800000,
        Trait      = 0x01000000,
	    Domain     = 0x02000000,
	    Relation   = 0x04000000,
        Number     = 0x08000000,
	    Transform  = 0x10000000,
	    NumberSet  = 0x20000000,
        Focal      = 0x40000000,
    }
}  
