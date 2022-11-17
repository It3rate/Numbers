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
		public MathElementKind Kind => MathElementKind.Focal;
		public int Id { get; }
        public Focal FocalBase { get; }
		public long Offset { get; set; } = 0;

		public Focal EquivalentFocal => FocalBase; // todo: Probably the whole offset idea is wrong, merge UnitFocal with Focal. Verify UnitTick etc work.

        public long StartTickPosition
		{
			get=>FocalBase.StartTickPosition + Offset;
			set
			{
				var fb = FocalBase;
				fb.StartTickPosition = value - Offset;
			}
		}
		public long EndTickPosition 
		{
			get => FocalBase.EndTickPosition + Offset;
			set
			{
				var fb = FocalBase;
				fb.EndTickPosition = value - Offset;
			}
		}
		public long LengthInTicks => FocalBase.LengthInTicks == 0 ? 1 : FocalBase.LengthInTicks;

		public UnitFocal(Focal unitFocal)
		{
			Id = unitFocal.Id;
			FocalBase = unitFocal;
		}

		public int Direction => FocalBase.Direction;

        // A unit tick is always positive direction (greater than zero). A unot is a unit flipped around zero, so same length pointing in opposite direction.
        public long UnitTick => StartTickPosition >= EndTickPosition ? StartTickPosition : EndTickPosition;
		public long ZeroTick => StartTickPosition;
		public long UnotTick => ZeroTick - UnitTick;
		public long UnitLengthInTicks => EndTickPosition - StartTickPosition == 0 ? 1 : EndTickPosition - StartTickPosition;
    }
}
