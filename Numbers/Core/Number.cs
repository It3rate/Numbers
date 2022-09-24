
namespace Numbers
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Numerics;


    public class Number
    {
        // number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
	    Trait Trait { get; }
        public int DomainId { get; set; }
	    public int FocalId { get; set; }

	    public Domain Domain => Trait.Domains[DomainId];
	    public Focal Focal => Trait.Focals[FocalId];

	    public Number(Trait trait, int domainId, int focalId)
	    {
		    Trait = trait;
		    DomainId = domainId;
		    FocalId = focalId;
	    }

        // need to account for non zero based units, and assign unit/unot based on direction
        private double StartValue => Trait.Start(Focal) / (double)Trait.UnitTicks(Domain);
        private double EndValue => Trait.End(Focal) / (double)-Trait.UnotTicks(Domain);
        public Complex Value => new Complex(EndValue, StartValue);
        public Complex Floor => new Complex(Math.Floor(EndValue), Math.Ceiling(StartValue));
        public Complex Ceiling => new Complex(Math.Ceiling(EndValue), Math.Floor(StartValue));
        public Complex Round => new Complex(Math.Round(EndValue), Math.Round(StartValue));
        public Complex Remainder => Value - Floor;

        public void SyncDomainTo(Number other) { }
        public void SyncFocalTo(Number other) { }
    }
}
