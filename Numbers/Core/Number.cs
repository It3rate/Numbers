
namespace Numbers.Core
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Numerics;


	public class Number : IMathElement
    {
	    public MathElementKind Kind => MathElementKind.Number;
	    private static int numberCounter = 1 + (int)MathElementKind.Number;

		public static Dictionary<int, Number> NumberStore { get; } = new Dictionary<int, Number>();

		public Number this[int i] => NumberStore[i];

		public int Id { get; set; }
		public int DomainId { get; set; }

        // number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
        public int FocalId { get; set; }

		public Domain Domain { get; set; }
		public Trait Trait => Domain.Trait;
		public Focal Focal => Trait.FocalStore[FocalId];

		public Number(Domain domain, int focalId)
		{
			Id = numberCounter++;
			Domain = domain;
			FocalId = focalId;
			domain.NumberIds.Add(Id);
			NumberStore.Add(Id, this);
        }

		public long StartTickLength
        {
			get => Focal.StartTickValue - Domain.Unit.ZeroTick;
			set { var f = Focal; f.StartTickValue = value + Domain.Unit.ZeroTick; }
		}
		public long EndTickLength
        {
			get => Focal.EndTickValue - Domain.Unit.ZeroTick;
			set { var f = Focal; f.EndTickValue = value + Domain.Unit.ZeroTick; }
		}

        public double StartValue => StartTickLength / (double) Domain.Unit.UnitLengthInTicks;
		public double EndValue => EndTickLength / (double) Domain.Unit.UnitLengthInTicks;
		public Complex Value => new Complex(EndValue, StartValue);
		public Complex Floor => new Complex(Math.Floor(EndValue), Math.Ceiling(StartValue));
		public Complex Ceiling => new Complex(Math.Ceiling(EndValue), Math.Floor(StartValue));
		public Complex Round => new Complex(Math.Round(EndValue), Math.Round(StartValue));
		public Complex Remainder => Value - Floor;

		public RatioSeg Ratio => Focal.RatioIn(Domain);
        public Number SyncDomain(Number other)
		{
			return other;//.Clone();
		}

		public void SyncFocalTo(Number other)
		{
		}
        // Operations with segments and units allow moving the unit around freely, so for example,
        // you can shift a segment by aligning the unit with start or end,
        // and scale in place by moving the unit to left, right or center (equivalent to affine scale, where you move to zero, scale, then move back)
        // need to have overloads that allow shifting the unit temporarily
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
			var tmp = (EndTickLength * synced.EndTickLength - StartTickLength * synced.StartTickLength) / Domain.Unit.UnitLengthInTicks;
			StartTickLength = (EndTickLength * synced.StartTickLength - StartTickLength * synced.EndTickLength) / Domain.Unit.UnitLengthInTicks;
			EndTickLength = tmp;
		}

		public void Divide(Number other)
		{
			var synced = SyncDomain(other);
			double end = EndTickLength;
			double start = StartTickLength;
			var oEnd = synced.EndTickLength;
			var oStart = synced.StartTickLength;
            // removed nan & divByZero checks, should go away anyway in final
			if ((oStart < 0 ? -oStart : +oStart) < (oEnd < 0 ? -oEnd : +oEnd))
			{
				double wr = oStart / (double) oEnd;
				double wd = oEnd + wr * oStart;

				double _tmp = (end + start * wr) / wd;
				start = (start - end * wr) / wd;
				end = _tmp;
			}
			else
			{
				double wr = oEnd / (double) oStart;
				double wd = oStart + wr * oEnd;
				double tmp = (end * wr + start) / wd;
				start = (start * wr - end) / wd;
				end = tmp;
			}
			StartTickLength = (long)(-start * Domain.Unit.UnitLengthInTicks);
			EndTickLength = (long)(end * Domain.Unit.UnitLengthInTicks);
        }

		public Focal CloneFocal() => Trait.CloneFocal(Focal);
		public Number Clone() => new Number(Domain, CloneFocal().Id);

		public override string ToString()
		{
			var v = Value;
			return $"[{-v.Imaginary:0.00}->{v.Real:0.00}]";
		}
	}
}
