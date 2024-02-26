//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using NumbersCore.Utils;

//namespace NumbersCore.Primitives
//{
//    /// <summary>
//    /// Poly represents dimension, so X and Y, or R,G,B. This holds multiple NumberChains, one for each dimension.
//    /// </summary>
//    public class PolyNumberChain : IMathElement
//    {
//        // todo: need a PolyDomain to handle the various domains, and the numbers need to lookup their domain rather than have a reference.
//        // todo: This is probably just a PolyDomain, no need to separate these
//        public virtual MathElementKind Kind => MathElementKind.PolyDomain;
//        public int Id { get; internal set; }
//        public int CreationIndex => Id - (int)Kind - 1;
//        public int PolyCount => _numberChains.Count;
//        public int Count => _numberChains[0].Count;
//        private List<NumberChain> _numberChains { get; } = new List<NumberChain>();

//        public PolyNumberChain(params NumberChain[] numberChains)
//        {
//            if (numberChains.Length == 0)
//            {
//                throw new ArgumentException("Must have at least one number chain in poly number chain.");
//            }
//            foreach (var chain in numberChains)
//            {
//                _numberChains.Add(chain);
//            }
//        }
//        public Number[] NumbersAt(int index)
//        {
//            Number[] result = null;
//            if (index < Count)
//            {
//                var list = new List<Number>();
//                foreach (var chain in _numberChains)
//                {
//                    list.Add(chain[index]);
//                }
//                result = list.ToArray();
//            }
//            return result;
//        }
//        public Focal[] FocalsAt(int index)
//        {
//            Focal[] result = null;
//            if (index < Count)
//            {
//                var list = new List<Focal>();
//                foreach (var chain in _numberChains)
//                {
//                    list.Add(chain.FocalAt(index));
//                }
//                result = list.ToArray();
//            }
//            return result;
//        }

//        public void AddPosition(params Number[] values)
//        {
//            if (values.Length == PolyCount)
//            {
//                int i = 0;
//                foreach (var chain in _numberChains)
//                {
//                    chain.AddItem(values[i++]);
//                }
//            }
//        }
//        public void AddPosition(params Focal[] values)
//        {
//            if (values.Length == PolyCount)
//            {
//                int i = 0;
//                foreach (var chain in _numberChains)
//                {
//                    chain.AddItem(values[i++]);
//                }
//            }
//        }
//        public void AddPosition(params long[] values)
//        {
//            if (values.Length == PolyCount * 2)
//            {
//                int i = 0;
//                foreach (var chain in _numberChains)
//                {
//                    chain.AddItem(values[i++], values[i++]);
//                }
//            }
//        }
//        public void AddPosition(params Range[] values)
//        {
//            if (values.Length == PolyCount)
//            {
//                int i = 0;
//                foreach (var chain in _numberChains)
//                {
//                    chain.AddItem(values[i++]);
//                }
//            }
//        }
//        public void AddPositions(params Number[] values)
//        {
//            if (values.Length % PolyCount == 0)
//            {
//                for (int i = 0; i < values.Length; i += PolyCount)
//                {
//                    for (var j = 0; j < PolyCount; j++)
//                    {
//                        _numberChains[j].AddItem(values[i * PolyCount + j]);
//                    }
//                }
//            }
//        }
//        public void AddPositions(params Focal[] values)
//        {
//            if (values.Length % PolyCount == 0)
//            {
//                for (int i = 0; i < values.Length; i += PolyCount)
//                {
//                    for (var j = 0; j < PolyCount; j++)
//                    {
//                        _numberChains[j].AddItem(values[i * PolyCount + j]);
//                    }
//                }
//            }
//        }
//        public void AddPositions(long[] values)
//        {
//            if (values.Length % (PolyCount * 2) == 0)
//            {
//                for (int i = 0; i < values.Length; i += PolyCount * 2)
//                {
//                    for (var j = 0; j < PolyCount; j++)
//                    {
//                        _numberChains[j].AddItem(values[i * PolyCount + j * 2], values[i * PolyCount + j * 2 + 1]);
//                    }
//                }
//            }
//        }
//        public void RemoveLastPosition()
//        {
//            foreach (var chain in _numberChains)
//            {
//                chain.RemoveLastPosition();
//            }
//        }
//        /// <summary>
//        /// Helper method to add the next focals by only specifying endpoints. 
//        /// It will use the previous Focal's endpoint as the current startpoint.
//        /// </summary>
//        public void AddIncrementalPosition(params long[] values)
//        {
//            if (Count > 0 && values.Length == PolyCount)
//            {
//                int i = 0;
//                foreach (var chain in _numberChains)
//                {
//                    var start = chain.Last().EndPosition;
//                    chain.AddItem(start, values[i++]);
//                }
//            }
//        }

//        public List<Number> GetInterleavedNumbers()
//        {
//            var result = new List<Number>();
//            for (int i = 0; i < Count; i++)
//            {
//                for (var j = 0; j < PolyCount; j++)
//                {
//                    result.Add(_numberChains[j][i]);
//                }
//            }
//            return result;
//        }
//        public List<Focal> GetInterleavedFocals()
//        {
//            var result = new List<Focal>();
//            for (int i = 0; i < Count; i++)
//            {
//                for (var j = 0; j < PolyCount; j++)
//                {
//                    result.Add(_numberChains[j].FocalAt(i));
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// Helper method to get polyline version of number vales, for rendering etc.
//        /// </summary>
//        public float[][] GetContiguousValues()
//        {
//            var len = Count * PolyCount;
//            var result = new List<float[]>(len);
//            if(len > 0)
//            {
//                var nums = GetInterleavedNumbers();
//                // add all start values for first number set, then all end points.
//                var item = new float[PolyCount];
//                for (int i = 0; i < PolyCount; i++)
//                {
//                    item[i] = (float)nums[i].StartValue;
//                }
//                result.Add(item);

//                for (int i = 0; i < nums.Count; i += PolyCount)
//                {
//                    item = new float[PolyCount];
//                    for (int j = 0; j < PolyCount; j++)
//                    {
//                        item[j] = (float)nums[i + j].EndValue;
//                    }
//                    result.Add(item);
//                }
//            }

//            return result.ToArray();
//        }
//        public void ResetWithContiguousValues(IEnumerable<float> values)
//        {
//            Reset();
//            var nextStarts = new double[PolyCount];
//            var ranges = new Range[PolyCount];
//            int polyCounter = 0;
//            int index = 0;
//            double firstVal = 0;
//            foreach (var value in values) 
//            {
//                if(index <= 1 && polyCounter < PolyCount * 2) // first set uses both points
//                {
//                    if(polyCounter % 2 == 0)
//                    {
//                        firstVal = value;
//                    }
//                    else
//                    {
//                        var idx = (polyCounter - 1) / 2;
//                        nextStarts[idx] = value;
//                        ranges[idx] = new Range(firstVal, value);
//                    }
//                }
//                else // rest of numbers, last tail with this tail, creating segments from polylines
//                {
//                    ranges[polyCounter] = new Range(nextStarts[polyCounter], value);
//                    nextStarts[polyCounter] = value;
//                }

//                polyCounter++;
//                if(polyCounter == PolyCount)
//                {
//                    if(index > 0)
//                    {
//                        AddPosition(ranges);
//                        // no need to clear ranges as they will overwrite
//                    }
//                    polyCounter = 0;
//                    index++;
//                }
//            }
//        }
//        /// <summary>
//        /// Helper method to get polyline version of focal positions, for rendering etc.
//        /// </summary>
//        public long[] GetContiguousPositions()
//        {
//            // focals are p0,p1, p1,p2, p2,p3...
//            // need: p0,p1,p2,p3...
//            var len = Count * PolyCount;
//            var result = new List<long>(len);
//            if (len > 0)
//            {
//                var focals = GetInterleavedFocals();
//                result.Add(focals[0].StartPosition);
//                result.Add(focals[0].EndPosition);
//                for (int i = 1; i < focals.Count; i += 2)
//                {
//                    result.Add(focals[i].StartPosition);
//                    result.Add(focals[i].EndPosition);
//                }
//            }

//            return result.ToArray();
//        }
//        public void ResetWithContiguousPositions(long[] positions)
//        {
//            // comes in as x0,y0,x1,y1,x2,y2
//            // postions are (x0,y0,x1,y1)(x1,y1,x2,y2)(x2,y2,x3,y3)
//            var posLen = PolyCount * 2;
//            if (positions.Length >= posLen)
//            {
//                Reset();
//                var nextStarts = new long[PolyCount];
//                for (int i = 0; i < PolyCount; i++)
//                {
//                    nextStarts[i] = positions[i];
//                }
//                var positionSet = new long[posLen];
//                for (int i = PolyCount; i < positions.Length; i += PolyCount)
//                {
//                    for (int j = 0; j < PolyCount; j++)
//                    {
//                        positionSet[j] = nextStarts[j];
//                    }
//                    for (int j = 0; j < PolyCount; j++)
//                    {
//                        positionSet[j + PolyCount] = positions[i + j];
//                        nextStarts[j] = positions[i + j];
//                    }
//                    AddPosition(positionSet);
//                }
//            }
//        }
//        public void Reset()
//        {
//            foreach (var chain in _numberChains)
//            {
//                chain.Reset();
//            }

//        }
//    }
//}
