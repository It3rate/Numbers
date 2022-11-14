﻿
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

		public long StartTickPos
        {
			get => Focal.StartTickValue - Domain.UnitFocal.ZeroTick;
			set => Focal.StartTickValue = value + Domain.UnitFocal.ZeroTick;
		}
		public long EndTickPos
        {
			get => Focal.EndTickValue - Domain.UnitFocal.ZeroTick;
			set => Focal.EndTickValue = value + Domain.UnitFocal.ZeroTick;
		}

		public double StartT
		{
			get => Focal.StartTickValue / (double)Domain.MaxRange.LengthInTicks;
			set => Focal.StartTickValue = (long)Math.Round(value * (double)Domain.MaxRange.LengthInTicks + Domain.MaxRange.StartTickValue);
		}
		public double EndT
		{
			get => Focal.EndTickValue / (double)Domain.MaxRange.LengthInTicks;
			set => Focal.EndTickValue = (long)Math.Round(value * Domain.MaxRange.LengthInTicks + Domain.MaxRange.StartTickValue);
		}

        public double StartValue => StartTickPos / (double) Domain.UnitFocal.UnitLengthInTicks;
		public double EndValue => EndTickPos / (double) Domain.UnitFocal.UnitLengthInTicks;
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
			StartTickPos += synced.StartTickPos;
			EndTickPos += synced.EndTickPos;
		}

		public void Subtract(Number other)
		{
			var synced = SyncDomain(other);
			StartTickPos -= synced.StartTickPos;
			EndTickPos -= synced.EndTickPos;
		}

		public void Multiply(Number other)
		{
			var synced = SyncDomain(other);
			var tmp = (EndTickPos * synced.EndTickPos - StartTickPos * synced.StartTickPos) / Domain.UnitFocal.UnitLengthInTicks;
			StartTickPos = (EndTickPos * synced.StartTickPos - StartTickPos * synced.EndTickPos) / Domain.UnitFocal.UnitLengthInTicks;
			EndTickPos = tmp;
		}

		public void Divide(Number other)
		{
			var synced = SyncDomain(other);
			double end = EndTickPos;
			double start = StartTickPos;
			var oEnd = synced.EndTickPos;
			var oStart = synced.StartTickPos;
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
			StartTickPos = (long)(-start * Domain.UnitFocal.UnitLengthInTicks);
			EndTickPos = (long)(end * Domain.UnitFocal.UnitLengthInTicks);
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
