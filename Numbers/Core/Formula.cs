﻿namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Formula : TransformBase
    {
        // account for repeats of formula, use stack to enable back selection
        public List<Transform> Transforms { get; } = new List<Transform>();

        public override void ApplyStart() { }
        public override void ApplyEnd() { }
        public override void ApplyPartial(long tickOffset) { }
    }
}
