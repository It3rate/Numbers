using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    /// <summary>
    /// Multiple Number Chains, one for each dimension.
    /// </summary>
    public class PolyNumberChain : IMathElement
    {
        public virtual MathElementKind Kind => MathElementKind.PolyNumberChain;
        public int Id { get; internal set; }
        public int CreationIndex => Id - (int)Kind - 1;
        public int PolyCount => _numberChains.Count;
        public int Count => _numberChains[0].Count;
        private List<NumberChain> _numberChains { get; }

        public PolyNumberChain(params NumberChain[] numberChains)
        {
            if (numberChains.Length == 0)
            {
                throw new ArgumentException("Must have at least one number chain in poly number chain.");
            }
            foreach (var chain in numberChains)
            {
                _numberChains.Add(chain);
            }
        }
        public Number[] NumbersAt(int index)
        {
            Number[] result = null;
            if (index < Count)
            {
                var list = new List<Number>();
                foreach (var chain in _numberChains)
                {
                    list.Add(chain[index]);
                }
                result = list.ToArray();
            }
            return result;
        }
        public Focal[] FocalsAt(int index)
        {
            Focal[] result = null;
            if (index < Count)
            {
                var list = new List<Focal>();
                foreach (var chain in _numberChains)
                {
                    list.Add(chain.FocalAt(index));
                }
                result = list.ToArray();
            }
            return result;
        }

        public void AddPosition(params Number[] values)
        {
            if (values.Length == PolyCount)
            {
                int i = 0;
                foreach (var chain in _numberChains)
                {
                    chain.AddItem(values[i++]);
                }
            }
        }
        public void AddPosition(params Focal[] values)
        {
            if (values.Length == PolyCount)
            {
                int i = 0;
                foreach (var chain in _numberChains)
                {
                    chain.AddItem(values[i++]);
                }
            }
        }
        public void AddPosition(params long[] values)
        {
            if (values.Length == PolyCount * 2)
            {
                int i = 0;
                foreach (var chain in _numberChains)
                {
                    chain.AddItem(values[i++], values[i++]);
                }
            }
        }
        public void AddPositions(params Number[] values)
        {
            if (values.Length % PolyCount == 0)
            {
                for (int i = 0; i < values.Length; i += PolyCount)
                {
                    for (var j = 0; j < PolyCount; j++)
                    {
                        _numberChains[j].AddItem(values[i * PolyCount + j]);
                    }
                }
            }
        }
        public void AddPositions(params Focal[] values)
        {
            if (values.Length % PolyCount == 0)
            {
                for (int i = 0; i < values.Length; i += PolyCount)
                {
                    for (var j = 0; j < PolyCount; j++)
                    {
                        _numberChains[j].AddItem(values[i * PolyCount + j]);
                    }
                }
            }
        }
        public void AddPositions(long[] values)
        {
            if (values.Length % (PolyCount * 2) == 0)
            {
                for (int i = 0; i < values.Length; i += PolyCount * 2)
                {
                    for (var j = 0; j < PolyCount; j++)
                    {
                        _numberChains[j].AddItem(values[i * PolyCount + j * 2], values[i * PolyCount + j * 2 + 1]);
                    }
                }
            }
        }
        public void RemoveLastPosition()
        {
            foreach (var chain in _numberChains)
            {
                chain.RemoveLastPosition();
            }
        }
        /// <summary>
        /// Helper method to add the next focals by only specifying endpoints. 
        /// It will use the previous Focal's endpoint as the current startpoint.
        /// </summary>
        public void AddIncrementalPosition(params long[] values)
        {
            if (Count > 0 && values.Length == PolyCount)
            {
                int i = 0;
                foreach (var chain in _numberChains)
                {
                    var start = chain.Last().EndPosition;
                    chain.AddItem(start, values[i++]);
                }
            }
        }

        public List<Number> GetInterleavedNumbers()
        {
            var result = new List<Number>();
            for (int i = 0; i < Count; i++)
            {
                for (var j = 0; j < PolyCount; j++)
                {
                    result.Add(_numberChains[j][i]);
                }
            }
            return result;
        }
        public List<Focal> GetInterleavedFocals()
        {
            var result = new List<Focal>();
            for (int i = 0; i < Count; i++)
            {
                for (var j = 0; j < PolyCount; j++)
                {
                    result.Add(_numberChains[j].FocalAt(i));
                }
            }
            return result;
        }

        /// <summary>
        /// Helper method to get polyline version of number vales, for rendering etc.
        /// </summary>
        public float[] GetContiguousValues()
        {
            var len = Count * PolyCount;
            var result = new List<float>(len);
            if(len > 0)
            {
                var nums = GetInterleavedNumbers();
                // add all start values for first number set, then all end points.
                for (int i = 0; i < PolyCount; i++)
                {
                    result.Add((float)nums[i].StartValue);
                }

                for (int i = 0; i < nums.Count; i++)
                {
                    result.Add((float)nums[i].EndValue);
                }
            }

            return result.ToArray();
        }
        public void ResetWithContiguousPositions(float[] positions)
        {
        }
        /// <summary>
        /// Helper method to get polyline version of focal positions, for rendering etc.
        /// </summary>
        public long[] GetContiguousPositions()
        {
            var len = Count * PolyCount;
            var result = new List<long>(len);
            if (len > 0)
            {
                var focals = GetInterleavedFocals();
                // add all start values for first number set, then all end points.
                for (int i = 0; i < PolyCount; i++)
                {
                    result.Add(focals[i].StartPosition);
                }

                for (int i = 0; i < focals.Count; i++)
                {
                    result.Add(focals[i].EndPosition);
                }
            }

            return result.ToArray();
        }
        public void ResetWithContiguousPositions(long[] positions)
        {
        }
        public void Reset()
        {
            foreach (var chain in _numberChains)
            {
                chain.Reset();
            }

        }
    }
}
