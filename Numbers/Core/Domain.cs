using System.Numerics;

namespace Numbers
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Domain
    {
	    private static int domainCounter = 1;

	    //public Trait Trait { get; }
        public int Id { get; }
        public int UnitId { get; set; }
        public int UnotId { get; set; }
        public int RangeId { get; set; }

        public Domain(int unitId, int unotId, int rangeId)
        {
	        UnitId = unitId;
	        UnotId = unotId;
	        RangeId = rangeId;
	        Id = domainCounter++;
        }
        public Domain(Focal unit, Focal unot, Focal range) : this(unit.Id, unot.Id, range.Id) { }
    }
}
