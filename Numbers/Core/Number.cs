
namespace Numbers
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Numerics;


	public class Number
	{
		private static int numberCounter = 1;
		public int Id { get; set; }

		// number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
		public int FocalId { get; set; }

		public Domain Domain { get; set; }
		public Trait Trait => Domain.Trait;
		public Focal Focal => Trait.Focals[FocalId];

		public Number(Domain domain, int focalId)
		{
			Domain = domain;
			FocalId = focalId;
			Id = numberCounter++;
		}

		public long StartTickLength
        {
			get => Focal.StartTickValue - Domain.Unit.HighTicks;
			set { var f = Focal; f.StartTickValue = value + Domain.Unit.HighTicks; }
		}
		public long EndTickLength
        {
			get => Focal.EndTickValue - Domain.Unit.LowTicks;
			set { var f = Focal; f.EndTickValue = value + Domain.Unit.LowTicks; }
		}

        public double StartValue => StartTickLength / (double) Domain.Unit.AbsLengthTicks;
		public double EndValue => EndTickLength / (double) Domain.Unit.AbsLengthTicks;
		public Complex Value => new Complex(EndValue, StartValue);
		public Complex Floor => new Complex(Math.Floor(EndValue), Math.Ceiling(StartValue));
		public Complex Ceiling => new Complex(Math.Ceiling(EndValue), Math.Floor(StartValue));
		public Complex Round => new Complex(Math.Round(EndValue), Math.Round(StartValue));
		public Complex Remainder => Value - Floor;

		public Number SyncDomain(Number other)
		{
			return other.Clone();
		}

		public void SyncFocalTo(Number other)
		{
		}

		public void Add(Number other)
		{
			var synced = SyncDomain(other);
			StartTickLength += synced.StartTickLength;
			EndTickLength += synced.EndTickLength;
		}
		public void Subtract(Number other)
		{
			var synced = SyncDomain(other);
			StartTickLength -= synced.StartTickLength;
			EndTickLength -= synced.EndTickLength;
		}
		public void Multiply(Number other)
		{
			var synced = SyncDomain(other);
			var tmp = (EndTickLength * synced.EndTickLength - StartTickLength * synced.StartTickLength) / Domain.Unit.AbsLengthTicks;
			StartTickLength = (EndTickLength * synced.StartTickLength - StartTickLength * synced.EndTickLength) / Domain.Unit.AbsLengthTicks;
			EndTickLength = tmp;
		}

		public Number Clone()
		{
			return new Number(Domain, FocalId);
		}

		public override string ToString()
		{
			var v = Value;
			return $"[{-v.Imaginary:0.00}->{v.Real:0.00}]";
		}
	}
}
