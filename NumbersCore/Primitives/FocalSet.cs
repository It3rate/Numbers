namespace NumbersCore.Primitives
{
    using NumbersCore.Utils;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    // todo: adapt this to be solution for bool intersection arrays and repeated add/mults 
    // needs base focal, and internal focals that are reused (as they may be link targets)
    // can be empty. Can not have overlaps. Can not exceed base focal length.
    // Focals are in order from start to end of line, and share its polarity.

    /// <summary>
    /// A series of ordered positions on a single line, the end point of one focal is the start point of the next one.
    /// </summary>
    public class FocalSet : Focal
    {
        public virtual MathElementKind Kind => MathElementKind.FocalSet;
        public override long StartPosition
        {
            get => _focals.Count > 0 ? _focals[0].StartPosition : 0;
            set { } // can't set values, they are calculated 
        }
        public override long EndPosition
        {
            get => _focals.Count > 0 ? _focals[_focals.Count - 1].EndPosition : 0;
            set { } // can't set values, they are calculated 
        }

        private OperationKind _operationKind = OperationKind.None;
        public OperationKind OperationKind
        {
            get => _operationKind;
            set { _operationKind = value; Regenerate(); }
        }
        private Focal _left;
        public Focal Left
        {
            get => _left;
            set { _left = value; Regenerate();}
        }
        private Focal _right;
        public Focal Right
        {
            get => _right;
            set { _right = value; Regenerate(); }
        }
        private long[] _positions;
        public override long[] Positions => _positions;

        public int SubFocalCount { get; private set; }

        protected List<Focal> _focals = new List<Focal>();

        public FocalSet(Focal left, Focal right, OperationKind operationKind)
        {
            _left = left;
            _right = right;
            _operationKind = operationKind;
            Regenerate();

        }
        public IEnumerable<Focal> Focals()
        {
            for (int i = 0; i < SubFocalCount; i++)
            {
                yield return _focals[i];
            }
        }
        /// <summary>
        /// Clamps focals to a sub area, useful for rendering.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>Returns cloned Focals that are not preserved.</returns>
        public IEnumerable<Focal> MaskedFocals(Focal mask)
        {
            for (int i = 0; i < SubFocalCount; i++)
            {
                if (_focals[i].EndPosition < mask.StartPosition || _focals[i].StartPosition > mask.EndPosition)
                {
                    continue;
                }
                var clone = _focals[i].Clone();
                if (clone.StartPosition < mask.StartPosition)
                {
                    clone.StartPosition = mask.StartPosition;
                }
                if (clone.EndPosition > mask.EndPosition)
                {
                    clone.EndPosition = mask.EndPosition;
                }
                yield return clone;
            }
        }

        public FocalSet(long startTickPosition, long endTickPosition) : base(startTickPosition, endTickPosition)
        {
        }
        public Focal GetFocalByIndex(int index)
        {
            Focal result = null;
            if (index >= 0 && index < SubFocalCount)
            {
                result = _focals[index];
            }
            return result;
        }
        public long[] GetPositions()
        {
            var result = new long[SubFocalCount * 2];
            int i = 0;
            foreach (var focal in _focals)
            {
                result[i++] = focal.StartPosition;
                result[i++] = focal.EndPosition;
            }
            return result;
        }
        private void ClearFocals()
        {
            SubFocalCount = 0;
        }


        private void Regenerate()
        {
            var op = OperationKind.GetFunc();
            var tt = BuildTruthTable(Left.Positions, Right.Positions);
            _positions = ApplyOpToTruthTable(tt, op);
            RegenFocals();
        }

        private void RegenFocals()
        {
            ClearFocals();
            for (int i = 0; i < _positions.Length; i += 2)
            {
                var f = FillNextPosition(_positions[i], _positions[i + 1]);
            }
        }
        private long[] ApplyOpToTruthTable(List<(long, bool, bool)> data, Func<bool, bool, bool> operation)
        {
            var result = new List<long>();
            var lastResult = false;
            var hadFirstTrue = false;
            foreach (var item in data)
            {
                var opResult = operation(item.Item2, item.Item3);
                if (!hadFirstTrue && opResult == true)
                {
                    result.Add(item.Item1);
                    hadFirstTrue = true;

                }
                else if (lastResult != opResult)
                {
                    result.Add(item.Item1);
                }
            }
            return result.ToArray();
        }
        private List<(long, bool, bool)> BuildTruthTable(long[] leftPositions, long[] rightPositions)
        {
            var result = new List<(long, bool, bool)>();
            var ss = new SortedSet<long>(leftPositions);
            ss.UnionWith(rightPositions);
            bool isLeftOn = false;
            bool isRightOn = false;
            foreach (var pos in ss)
            {
                if (leftPositions.Contains(pos)) { isLeftOn = !isLeftOn; }
                if (rightPositions.Contains(pos)) { isRightOn = !isRightOn; }
                result.Add((pos, isLeftOn, isRightOn));
            }
            return result;
        }
        private void AddSection(long startPosition, long endPosition)
        {
            FillNextPosition(startPosition, endPosition);
            RemoveOverlaps();
        }

        private Focal FillNextPosition(long startPosition, long endPosition)
        {
            Focal result;
            if (_focals.Count > SubFocalCount + 1)
            {
                result = _focals[SubFocalCount++];
                result.Reset(startPosition, endPosition);
            }
            else
            {
                result = new Focal(startPosition, endPosition);
                SubFocalCount++;
            }
            return result;
        }
        private void RemoveOverlaps()
        {

        }
        //public void AddPositions(long[] positions)
        //{
        //    _positions.AddRange(positions);
        //}
        //public void RemoveLastPosition()
        //{
        //    if (_positions.Count > 2)
        //    {
        //        _positions.RemoveAt(_positions.Count - 1);
        //    }
        //}
        //public void Clear()
        //{
        //    _positions.Clear();
        //}

        //public Focal FocalAt(int index)
        //{
        //    return index >= 0 && index < Count - 1 ? new Focal(_positions[index], _positions[index + 1]) : null;
        //}
        //public Focal PositionAt(int index)
        //{
        //    return index >= 0 && index < Count ? new Focal(_positions[index], _positions[index]) : null;
        //}

        //public long DirectedLength => Positions.Select(x => x).Sum();
        //public ulong Length => (ulong)_positions.Zip(_positions.Skip(1), (a, b) => Math.Abs(a - b)).Sum();

        //public override void StretchBy(Focal amount)
        //{
        //    var seg = amount.Seg;
        //    for (var i = 0; i < _positions.Count; i++)
        //    {
        //        var pseg = new Seg(0, _positions[i]);
        //        _positions[i] = (long)(pseg * seg).End;
        //    }
        //}

        //public override void SqueezeBy(IFocal amount)
        //{
        //    var seg = amount.Seg;
        //    for (var i = 0; i < _positions.Count; i++)
        //    {
        //        var pseg = new Seg(0, _positions[i]);
        //        _positions[i] = (long)(pseg / seg).End;
        //    }
        //}

        // these work with segs, so needs a polyseg, or maybe just array of segs?
        //public override Seg Seg
        //{
        //    get { throw new NotImplementedException(); }
        //    set { throw new NotImplementedException(); }
        //}
        //public override Seg ShiftedSeg(long value)
        //{
        //    throw new NotImplementedException();
        //}
        //public override Seg GetSegWithBasis(IFocal basis)
        //{
        //    throw new NotImplementedException();
        //}
        //public override void SetTicksWithSegBasis(Seg seg, IFocal basis)
        //{
        //    throw new NotImplementedException();
        //}
        //public override Seg UnitTRangeIn(IFocal basis)
        //{
        //    throw new NotImplementedException();
        //}



        //public FocalSet Clone()
        //{
        //    return new FocalSet((long[])Positions.Clone());
        //}

    }

    public enum OperationKind
    {
        None, 
        FALSE,
        AND,
        AND_NOT,
        FIRST_INPUT,
        NOT_AND,
        SECOND_INPUT,
        XOR,
        OR,
        NOR,
        XNOR,
        NOT_SECOND_INPUT,
        IF_THEN,
        NOT_FIRST_INPUT,
        THEN_IF,
        NAND,
        TRUE,
    }
}