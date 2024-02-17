namespace NumbersCore.CoreConcepts.Spatial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.CoreConcepts.Counter;
    using NumbersCore.Primitives;

    public abstract class SpatialDomain : Domain
    {
        //public static SpatialDomain Polyline2DDomain { get; } = new SpatialDomain(Focal.CreateZeroFocal(1), new Focal(long.MinValue, long.MaxValue), "Polyline2D");

        public SpatialDomain(Focal basisFocal, Focal maxFocal, string name) : base(CounterTrait.Instance, basisFocal, maxFocal, name)
        {
            IsVisible = false;
        }
        // methods like getSpatialSize, center, bounds etc. Work in 1D, 2D, 3D, polar, bezier etc
    }
}
