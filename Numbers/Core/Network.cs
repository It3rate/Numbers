namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Network
    {
	    public MathElementKind Kind => MathElementKind.Network;
	    private static int networkCounter = 1 + (int)MathElementKind.Network;
    }
}
