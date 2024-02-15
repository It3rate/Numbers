namespace NumbersCore.Primitives
{
    using NumbersCore.Utils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A series of ordered positions. These can be merged with other Focals using bool operations of OperationKind is not None.
    /// Perhaps these two abilities should be separated - Q. Are non overlapping ordered elements a core type of value, or just a special bool case?
    /// Can be empty (e.g result of bool AND with no overlap)
    /// </summary>
    public class FocalChain : Focal
    {
        // todo: account for direction of focal line?
        public virtual MathElementKind Kind => MathElementKind.FocalChain;
        public int Count { get; 
            private set; }

        private OperationKind _operationKind = OperationKind.None;
        public OperationKind OperationKind
        {
            get => _operationKind;
            set { _operationKind = value; }// SelfGenerate(); }
        }
        protected List<Focal> _focals = new List<Focal>();

        private List<long> _positions = new List<long>();
        public override long[] Positions => _positions.ToArray();
        public override long StartPosition
        {
            get => Count > 0 ? _focals[0].StartPosition : 0;
            set { if (Count > 0) { _focals[Count - 1].StartPosition = value; } }
        }
        public override long EndPosition
        {
            get => Count > 0 ? _focals[Count - 1].EndPosition : 0;
            set { if (Count > 0) { _focals[Count - 1].EndPosition = value; } } 
        }
        public Focal this[int index]
        {
            get
            {
                if (index >= 0 && index < Count) // test for count, not _focals length
                {
                    return _focals[index];
                }
                else
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }
            }
            set
            {
                if (index >= 0 && index < Count)
                {
                    _focals[index] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException("Index is out of range.");
                }
            }
        }



        public FocalChain(IEnumerable<Focal> focals = null, OperationKind operationKind = OperationKind.None)
        {
            _operationKind = operationKind;
            if (focals != null)
            {
                var positions = GetPositions(focals);
                RegenFocals(positions);
            }
        }
        public IEnumerable<Focal> UnmaskedFocals()
        {
            for (int i = 0; i < Count; i++)
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
            for (int i = 0; i < Count; i++)
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

        public FocalChain(long startTickPosition, long endTickPosition) : base(startTickPosition, endTickPosition)
        {
        }
        public void Reset(Focal left, Focal right, OperationKind operationKind)
        {
            Clear();
            OperationKind = OperationKind.None;
            Add(left);
            OperationKind = operationKind;
            Add(right);
        }
        public void Reset(Focal left, OperationKind operationKind)
        {
            Clear();
            OperationKind = OperationKind.None;
            Add(left);
            OperationKind = operationKind;
        }
        public Focal GetFocalByIndex(int index)
        {
            Focal result = null;
            if (index >= 0 && index < Count)
            {
                result = _focals[index];
            }
            return result;
        }
        public static long[] GetPositions(IEnumerable<Focal> focals)
        {
            var result = new long[focals.Count() * 2];
            int i = 0;
            foreach (var focal in focals)
            {
                result[i++] = focal.StartPosition;
                result[i++] = focal.EndPosition;
            }
            return result;
        }

        private void RegenFocals(long[] positions)
        {
            Clear();
            _positions.AddRange(positions);
            for (int i = 0; i < _positions.Count; i += 2)
            {
                var p0 = _positions[i];
                // odd number of positions creates a point at end. Anything depending odd stores on this should use positions directly.
                var p1 = i + 1 < _positions.Count ? _positions[i + 1] : p0; 
                var f = FillNextPosition(p0, p1);
            }
        }
        private void SelfGenerate()
        {
            var focals = new List<Focal>(UnmaskedFocals()); // these are overwritten in process, but internally works on positions, not focals.
            Clear();
            AddRange(focals);
        }

        /// <summary>
        /// Merge existing focals to each other by iterating over each one using the internal operation.
        /// </summary>
        public void AddRange(IEnumerable<Focal> focals)
        {
            var orgPositions = GetPositions(focals);
            for (int i = 0; i < orgPositions.Length; i += 2)
            {
                Add(orgPositions[i + 0], orgPositions[i + 1]);
            }
            //if (OperationKind != OperationKind.None && focals.Count > 1)
            //{
            //    var op = OperationKind.GetFunc();
            //    var leftPositions = new long[] { orgPositions[0], orgPositions[1] };
            //    for (int i = 2; i < orgPositions.Length; i += 2)
            //    {
            //        Add(orgPositions[i + 0], orgPositions[i + 1]);
            //        //var rightPositions = new long[] { orgPositions[i + 0], orgPositions[i + 1] };
            //        //var tt = BuildTruthTable(leftPositions, rightPositions);
            //        //leftPositions = ApplyOpToTruthTable(tt, op);
            //    }
            //    RegenFocals();
            //}
            //else
            //{
            //}
        }
        public void Add(Focal focal)
        {
            Add(focal.StartPosition, focal.EndPosition);
        }
        public void Add(long start, long end)
        {
            if (OperationKind != OperationKind.None)
            {
                var tt = BuildTruthTable(Positions, new long[] {start, end});
                var positions = ApplyOpToTruthTable(tt, OperationKind.GetFunc());
                RegenFocals(positions);
            }
            else
            {
                FillNextPosition(start, end);
                _positions.Add(start);
                _positions.Add(end);
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
                    lastResult = opResult;

                }
                else if (lastResult != opResult)
                {
                    result.Add(item.Item1);
                    lastResult = opResult;
                }
            }

            if(lastResult == true) // always close
            {
                result.Add(data.Last().Item1);
            }
            return result.ToArray();
        }

        // truth table only acts on valid parts of segments. Remember a -10i+5 has two parts, 0 to -10i and 0 to 5. This is the area bools apply to.
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

        private Focal FillNextPosition(long startPosition, long endPosition)
        {
            Focal result;
            if (_focals.Count > Count + 1)
            {
                result = _focals[Count];
                result.Reset(startPosition, endPosition);
            }
            else
            {
                result = new Focal(startPosition, endPosition);
                _focals.Add(result);
            }
            Count++;
            return result;
        }
        private void RemoveOverlaps()
        {

        }
        public void Clear()
        {
            _positions.Clear();
            Count = 0;
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

        FlipInPlace,
        Add,
        Subtract,
        Multiply,
        Divide,

        PowerAdd,
        PowerMultiply,

        AppendAll, // repeat are added together ('regular' multiplication)
        MultiplyAll, // repeat are multiplied together (exponents)
        Blend, // multiply as in area, blend from unot to unit
        Average,
        Root,
        Wedge,
        DotProduct,
        GeometricProduct,
    }
}