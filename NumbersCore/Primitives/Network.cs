namespace NumbersCore.Primitives
{
	public class Network
    {
	    public MathElementKind Kind => MathElementKind.Network;
	    private static int networkCounter = 1 + (int)MathElementKind.Network;
    }
}
