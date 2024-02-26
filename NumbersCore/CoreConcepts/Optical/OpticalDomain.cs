﻿namespace NumbersCore.CoreConcepts.Optical
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.CoreConcepts.Counter;
    using NumbersCore.CoreConcepts.Temperature;
    using NumbersCore.Primitives;
    using NumbersCore.Utils;

    public class OpticalDomain : Domain
    {
        public static OpticalDomain RedDomain { get; } = CreateDomain(Common.ByteScope(), "Red");
        public static OpticalDomain GreenDomain { get; } = CreateDomain(Common.ByteScope(), "Green");
        public static OpticalDomain BlueDomain { get; } = CreateDomain(Common.ByteScope(), "Blue");

        public static OpticalDomain HueDomain { get; } = CreateDomain(Common.DegreeScope(), "Hue");
        public static OpticalDomain SaturationDomain { get; } = CreateDomain(Common.CentScope(), "Saturation");
        public static OpticalDomain LightnessDomain { get; } = CreateDomain(Common.CentScope(), "Lightness");

        public OpticalDomain(Focal basisFocal, Focal maxFocal, string name) : base(OpticalTrait.Instance, basisFocal, maxFocal, name)
        {
        }
        public static OpticalDomain CreateDomain(DomainScope df, string name, bool isVisible = true)
        {
            var domain = new OpticalDomain(df.Basis, df.MinMax, name);
            return domain;
        }
        
            public static OpticalDomain CreateDomain(int unitSize, float minRange, float maxRange, int zeroPoint, string name, bool isVisible = true)
        {
            Trait trait = OpticalTrait.Instance;
            var basis = new Focal(zeroPoint, zeroPoint + unitSize);
            var minMax = new Focal((int)(-minRange * unitSize + zeroPoint), (int)(maxRange * unitSize + zeroPoint));
            var domain = new OpticalDomain(basis, minMax, name);
            domain.Trait = trait;
            domain.IsVisible = isVisible;
            return domain;
        }
    }
}
