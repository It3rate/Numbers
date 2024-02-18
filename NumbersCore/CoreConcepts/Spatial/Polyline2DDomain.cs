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
        public void ResetWithContiguousPositions(IEnumerable<long> positions) => XYValues.ResetWithContiguousPositions(positions);

        public void Reset()
        {
            foreach (var num in NumberStore.Values)
            {
            }
        }
    }
}
