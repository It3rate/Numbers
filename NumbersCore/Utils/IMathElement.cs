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
        Formula    = 0x00200001,
        Definition = 0x00400001,
        Trait      = 0x00800001,
	    Domain     = 0x01000001,
	    Number     = 0x02000001,
	    Transform  = 0x04000001,
        Selection  = 0x08000001,
	    Focal      = 0x10000001,
    }
}
