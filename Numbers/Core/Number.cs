
using Numbers.Mind;

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

	    public Number this[int i] => Workspace.NumberStore[i];

		public int Id { get; set; }
		public int DomainId { get; set; }

        // number to the power of x, where x is also a focal. Eventually this is equations, lazy solve them.
        public int FocalId { get; set; }

		public Domain Domain { get; set; }
		public Trait Trait => Domain.Trait;
		public Focal Focal => Trait.FocalStore[FocalId];
		public Focal Unit => Trait.FocalStore[Domain.UnitFocalId];
		public long ZeroTick => Unit.StartTickPosition;
		public long UnitLength => Unit.LengthInTicks;
		public long AbsUnitLength => Unit.AbsLengthInTicks;

        public bool IsUnitOrUnot => Domain.UnitId == Id;
		public bool IsUnit => IsUnitOrUnot && Direction == 1;
		public bool IsUnot => IsUnitOrUnot && Direction == -1;
        public bool IsUnitPerspective => Domain.IsUnitPerspective;
        public int Direction => StartTickPosition <= EndTickPosition ? 1 : -1;
        public int Sign()
        { 
	        var unitDir = Domain.UnitFocal.Direction;
	        var sv = -StartValue * unitDir;
	        var ev = EndValue * unitDir;
            return StartValue < EndValue ? 1 : StartValue == EndValue ? 0 : -1;
        }

        public Number(Domain domain, int focalId)
		{
			Id = numberCounter++;
			Domain = domain;
			FocalId = focalId;
			domain.NumberIds.Add(Id);
			Workspace.NumberStore.Add(Id, this);
        }

        public long StartTickPosition
        {
	        get => Focal.StartTickPosition;
	        set => Focal.StartTickPosition = value;
        }
        public long EndTickPosition
        {
	        get => Focal.EndTickPosition;
	        set => Focal.EndTickPosition = value;
        }
        public long StartValueInTicks
        {
			get => -Focal.StartTickPosition + ZeroTick;
			set => Focal.StartTickPosition = ZeroTick - value;
		}
		public long EndValueInTicks
		{
			get => Focal.EndTickPosition - ZeroTick;
			set => Focal.EndTickPosition = value + ZeroTick;
		}
		public long TickCount => Focal.EndTickPosition - Focal.StartTickPosition;
		
        public double StartT
        {
	        get => (Focal.StartTickPosition - ZeroTick) / (double)UnitLength;
	        set => Focal.StartTickPosition = (long)Math.Round(value * UnitLength + ZeroTick);
		}
		public double EndT
		{
			get => (Focal.EndTickPosition - ZeroTick) / (double)UnitLength;
			set => Focal.EndTickPosition = (long)Math.Round(value * UnitLength + ZeroTick);
		}

        public double StartValue
        {
	        get => StartValueInTicks / (double) UnitLength;
	        set => StartValueInTicks = (long)Math.Round(value * UnitLength); 
        }
        public double EndValue
		{
			get => EndValueInTicks / (double) UnitLength;
			set => EndValueInTicks = (long) Math.Round(value * UnitLength);
        }
        public Complex Value
        {
	        get => new Complex(EndValue, StartValue);
	        set { StartValue = value.Imaginary; EndValue = value.Real; }
        }
        public Complex ValueInUnitPerspective => new Complex(EndValue, -StartValue);
        public Complex ValueInUnotPerspective => new Complex(-EndValue, StartValue);

        public long WholePartStart => (long)Math.Round(StartValue);
        public long NumeratorPartStart => StartValueInTicks % DenominatorPart;
        public long WholePartEnd => (long)Math.Round(EndValue);
        public long NumeratorPartEnd => EndValueInTicks % DenominatorPart;
        public long DenominatorPart => Math.Abs(Domain.Unit.TickCount);

        public Complex Floor => new Complex(Math.Floor(EndValue), Math.Ceiling(StartValue));
		public Complex Ceiling => new Complex(Math.Ceiling(EndValue), Math.Floor(StartValue));
		public Complex Round => new Complex(Math.Round(EndValue), Math.Round(StartValue));
		public Complex Remainder => Value - Floor;

		public RatioSeg Ratio => Domain.FocalAsRatio(Focal);
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
			StartValueInTicks += synced.StartValueInTicks;
			EndValueInTicks += synced.EndValueInTicks;
		}

		public void Subtract(Number other)
		{
			var synced = SyncDomain(other);
			StartValueInTicks -= synced.StartValueInTicks;
			EndValueInTicks -= synced.EndValueInTicks;
		}

		public void Multiply(Number other)
		{
			var synced = SyncDomain(other);
			var tmp = (EndValueInTicks * synced.EndValueInTicks - StartValueInTicks * synced.StartValueInTicks) / UnitLength;
			StartValueInTicks = (EndValueInTicks * synced.StartValueInTicks - StartValueInTicks * synced.EndValueInTicks) / UnitLength;
			EndValueInTicks = tmp;
		}

		public void Divide(Number other)
		{
			var synced = SyncDomain(other);
			double end = EndValueInTicks;
			double start = StartValueInTicks;
			var oEnd = synced.EndValueInTicks;
			var oStart = synced.StartValueInTicks;
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
			StartValueInTicks = (long)(-start * UnitLength);
			EndValueInTicks = (long)(end * UnitLength);
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
