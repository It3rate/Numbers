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

    public class Polyline2DDomain : SpatialDomain
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
        private NumberChain _x;
        private NumberChain _y;
        public PolyNumberChain XYValues { get; }
        public Polyline2DDomain(int size) : this(Focal.CreateZeroFocal(1), Focal.CreateBalancedFocal(size)) { }
        public Polyline2DDomain(Focal basisFocal, Focal maxFocal) : base(basisFocal, maxFocal, "Polyline2D")
        {
            _x = new NumberChain(this, Polarity.Aligned);
            _y = new NumberChain(this, Polarity.Aligned);
            XYValues = new PolyNumberChain(_x, _y);
        }
        public void AddPosition(Number x, Number y)
        {
            XYValues.AddPosition(x, y);
        }
        public void AddPosition(Focal x, Focal y)
        {
            XYValues.AddPosition(x, y);
        }
        public void AddPosition(long xi, long xr, long yi, long yr)
        {
            XYValues.AddPosition(xi, xr, yi, yr);
        }
        public void AddIncrementalPosition(long x, long y)
        {
            XYValues.AddIncrementalPosition(x, y);
        }
        public (Number,Number) NumbersAt(int index)
        {
            var result =  XYValues.NumbersAt(index);
            return (result[0], result[1]);
        }
        public (Focal, Focal) FocalsAt(int index)
        {
            var result = XYValues.FocalsAt(index);
            return (result[0], result[1]);
        }
        public override Number CreateDefaultNumber(bool addToStore = true)
        {
            var num = new Number(new Focal(0, 1));
            return AddNumber(num, addToStore);
        }
        public float[][] GetContiguousValues() => XYValues.GetContiguousValues();
        public void ResetWithContiguousValues(IEnumerable<float> positions) => XYValues.ResetWithContiguousValues(positions);
        public long[] GetContiguousPositions() => XYValues.GetContiguousPositions();
        public void ResetWithContiguousPositions(IEnumerable<long> positions) => XYValues.ResetWithContiguousPositions(positions.ToArray());

        public void Reset()
        {
            foreach (var num in NumberStore.Values)
            {
            }
        }
    }
}
