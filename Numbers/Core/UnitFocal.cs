namespace Numbers.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

    // Creates a unit from a Focal, this has positive direction, and negative in the unot direction. Assumes a shared zero point and equal lengths.
    // It is possible for unit/unot to not share zero and have to different lengths, and thus be two segments. This class will adjust to that as needed.
    // For now it is a unit multiplied and a unot that is the unit multiplied by 'i' (which flips the number line around zero).
	public class UnitFocal : IFocal
	{
		public Focal FocalBase { get; }
		public long Offset { get; set; } = 0;

		public long StartTickValue
		{
			get=>FocalBase.StartTickValue + Offset;
			set {var fb = FocalBase; fb.StartTickValue = value - Offset;}
		}
		public long EndTickValue 
		{
			get => FocalBase.EndTickValue + Offset;
			set { var fb = FocalBase; fb.EndTickValue = value - Offset; }
		}
		public long LengthInTicks => FocalBase.LengthInTicks;

		public UnitFocal(Focal unitFocal)
		{
			FocalBase = unitFocal;
		}

		public Pointing Direction => FocalBase.Direction;
		public RatioSeg RatioIn(Domain domain) => FocalBase.RatioIn(domain);

        // A unit tick is always positive direction (greater than zero). A unot is a unit flipped around zero, so same length pointing in opposite direction.
        public long UnitTick => StartTickValue >= EndTickValue ? StartTickValue : EndTickValue;
		public long ZeroTick => StartTickValue >= EndTickValue ? EndTickValue : StartTickValue;
		public long UnotTick => ZeroTick - UnitTick;
		public long UnitLengthInTicks => Math.Abs(EndTickValue - StartTickValue);
    }
}
