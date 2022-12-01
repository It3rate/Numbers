
using System;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    public class FocalRefx : FocalBase // todo: Eventually need to consider what the benefits are of having virtual positions like this. Maybe none?
    {
	    private static int _positionCounter = 1;
        public int StartId { get; set; } // ref to start point position
	    public int EndId { get; set; } // ref to end point position
        public override long StartTickPosition
	    {
		    get => MyTrait.PositionStore[StartId];
		    set => MyTrait.PositionStore[StartId] = value;
	    }
	    public override long EndTickPosition
        {
		    get => MyTrait.PositionStore[EndId];
		    set => MyTrait.PositionStore[EndId] = value;
	    }

        private FocalRefx(Trait trait, int startId, int endId) : base(trait)
	    {
		    StartId = startId;
		    EndId = endId;
	    }
	    public static FocalRefx CreateByIds(Trait trait, int startId, int endId)
	    {
		    var result = new FocalRefx(trait, startId, endId);
		    trait.FocalStore.Add(result.Id, result);
		    return result;
        }
	    public static FocalRefx CreateByValues(Trait trait, long startPosition, long endPosition)
	    {
		    trait.PositionStore.Add(_positionCounter++, startPosition);
		    trait.PositionStore.Add(_positionCounter++, endPosition);
            var result = new FocalRefx(trait, _positionCounter - 2, _positionCounter - 1);
            trait.FocalStore.Add(result.Id, result);
            return result;
	    }

	    public override IFocal Clone()
	    {
		    return CreateByValues(MyTrait, StartTickPosition, EndTickPosition);
	    }
    }
}
