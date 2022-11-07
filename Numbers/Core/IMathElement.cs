namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMathElement
    {
	    MathElementKind Kind { get; }
	    int Id { get; }
    }

    public enum MathElementKind
    {
	    None = 0,
        Network   = 0x00100001,
	    Formula   = 0x00200001,
	    Trait     = 0x00400001,
	    Domain    = 0x00800001,
	    Number    = 0x01000001,
	    Transform = 0x02000001,
        Selection = 0x04000001,
	    Focal     = 0x08000001,
    }
}
