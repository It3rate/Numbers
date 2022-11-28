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
        Network    = 0x00100001,
        Workspace  = 0x00200001,
        Formula    = 0x00400001,
        Definition = 0x00800001,
        Trait      = 0x01000001,
	    Domain     = 0x02000001,
	    Number     = 0x04000001,
	    Transform  = 0x08000001,
        Selection  = 0x10000001,
	    Focal      = 0x20000001,
    }
}
