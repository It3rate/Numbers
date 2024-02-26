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
    using System.Xml.Linq;

    /// <summary>
    /// A series of ordered positions. These can be merged with other Focals using bool operations if OperationKind is not None.
    /// Can be empty (e.g result of bool AND with no overlap)
    /// </summary>
    public class FocalChain : Focal
    {
        // todo: account for direction of focal line?
        // Perhaps the number store and bool segment capabilities should be separated? The first is like a bidirectional relative accumulator, the second a non overlapping ordered group of segments.
        // - Q.Are non overlapping ordered elements a core type of value, or just a special bool case?
        //     the main difference is the bool case only allows forward direction for segments, while a path can meander. Both must be tip to tail.
        //     relative encoding may make more sense as this enforces tip to tail (2,4,3,1 vs 2,6,9,10)
        // todo: Maybe bool results need to be segments of alternating polarity? Currently there is a confusion between 'not considered' and 'false'.
        public virtual MathElementKind Kind => MathElementKind.FocalChain;


        protected List<Focal> _focals = new List<Focal>();
        private List<long> _positions = new List<long>();
        public int Count { get; private set; }
        public override bool IsDirty
        {
            get => _focals.Any(focal => focal.IsDirty);
            set
            {
                base.IsDirty = value;
                foreach (var focal in _focals) { focal.IsDirty = false; }
            }

        }
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

        public FocalChain(long startTickPosition, long endTickPosition) : base(startTickPosition, endTickPosition)
        {
        }

        public Focal this[int index]
        {
            get
            {
                Focal result = null;
                if (index >= 0 && index < Count)
                {
                    result = _focals[index];
                }
                return result;
            }
        }
        public FocalChain(IEnumerable<Focal> focals = null)
        {
            if (focals != null)
            {
                var positions = GetPositions(focals);
                RegenerateFocals(positions);
            }
        }
        public IEnumerable<Focal> Focals()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _focals[i];
            }
        }
        public long[] GetPositions() => _positions.ToArray();
        public override void Reset(long start, long end)
        {
            Clear();
            Add(start, end);
        }
        public override void Reset(Focal left)
        {
            Clear();
            Add(left);
        }
        public void Reset(Focal left, Focal right, OperationKind operationKind)
        {
            Clear();
            Add(left);
            Add(right, operationKind);
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
        public void Add(Focal focal, OperationKind operationKind = OperationKind.None)
        {
            Add(focal.StartPosition, focal.EndPosition, operationKind);
        }
        public void Add(long start, long end, OperationKind operationKind = OperationKind.None)
        {
            if (operationKind != OperationKind.None)
            {
                var tt = BuildTruthTable(_positions.ToArray(), new long[] {start, end});
                var positions = ApplyOpToTruthTable(tt, operationKind.GetFunc());
                RegenerateFocals(positions);
            }
            else
            {
                FillNextPosition(start, end);
                _positions.Add(start);
                _positions.Add(end);
            }
        }
        public Focal Last() => Count > 0 ? _focals[Count - 1] : null;
        public void RemoveLastPosition()
        {
            if(Count > 0)
            {
                Count--;
            }
        }
        private static long[] GetPositions(IEnumerable<Focal> focals)
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
        private void RegenerateFocals(long[] positions)
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
            var focals = new List<Focal>(Focals()); // these are overwritten in process, but internally works on positions, not focals.
            Clear();
            AddRange(focals);
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
        public void Clear()
        {
            _positions.Clear();
            Count = 0;
        }



        public static bool operator ==(FocalChain a, FocalChain b)
        {
            if (a is null || b is null)
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(FocalChain a, FocalChain b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is FocalChain other && Equals(other);
        }
        public bool Equals(FocalChain value)
        {
            var result = false;
            if( ReferenceEquals(this, value))
            {
                result = true;
            }
            else if (Count != value.Count)
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < _focals.Count; i++)
                {
                    if (_focals[i] != value._focals[i])
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _focals.GetHashCode();
                return hashCode;
            }
        }

    }
}