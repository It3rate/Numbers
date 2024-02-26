namespace NumbersCore.CoreConcepts.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public class PositionDomain : PolyDomain
    {
        // Q. Can numbers combine as separate domains within a single domain like this, or does every dimension require it's own (related) domain?
        // Two numbers in one domain combine through operations creating a result. X and Y have domains so similar they feel the same, but probably aren't.
        // Need a PolyDomain, that abstractly combines multiple domains in certain ways, and then populate (eg with RGB, XYZ, mph...)
        // These can combine upwards to 'organism' level complexity? The domain determines the bounds of behavior, the organism navigates it.

        // ** polynumbers on the same domain are branches or alternate paths, like TYLOX (local dimensions). On separate domains are combinations of properties.

        // Q. is there a difference between two numbers on a line and a branch? (like result of a NAND)? Maybe that is how you define a branch. 'T' would be three numbers like: _=
        // the difference is a result needs to be a single directional ordered line, divided. A branch can be any two numbers.
        // But a branch can come from a result, and a branch can OP transform to a result.
        // The poly domains aren't specific to traits, but maybe can require matching traits (like X,Y must both be distance, but the combo works for e.g. gear ratios as well.)
        public SpatialDomain XDomain { get; }
        public SpatialDomain YDomain { get; }
        public NumberChain X { get; }
        public NumberChain Y { get; }
        public PositionDomain(int xSize, int ySize) : base(
                SpatialDomain.GetPixelDomain(xSize, "X"),
                SpatialDomain.GetPixelDomain(ySize, "Y"))
        {
            XDomain = (SpatialDomain)GetDomainByName("X");
            YDomain = (SpatialDomain)GetDomainByName("Y");
            X = GetChainByName("X");
            Y = GetChainByName("Y");
        }
        public void AddXY(Number x, Number y)
        {
            AddPositions(x, y);
        }
        public void AddXY(Focal x, Focal y)
        {
            AddPosition(x, y);
        }
        public void AddXY(long xi, long xr, long yi, long yr)
        {
            AddPosition(xi, xr, yi, yr);
        }
        public void AddIncrementalXY(long x, long y)
        {
            AddIncrementalPosition(x, y);
        }
        public (Number,Number) XYAt(int index)
        {
            var result =  NumbersAt(index);
            return (result[0], result[1]);
        }
        public (Focal, Focal) XYFocalsAt(int index)
        {
            var result = FocalsAt(index);
            return (result[0], result[1]);
        }

    }
}
