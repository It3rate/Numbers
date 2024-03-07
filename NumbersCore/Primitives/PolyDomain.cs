﻿namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using NumbersCore.Utils;

    public abstract class PolyDomain : IMathElement
    {
        // todo: Domains are a special case of PolyDomain, where there is only one domain.
        // These single domains are primitives (temperature), or combinations so common/complex the joins are not normally computed (weight (m*g), happiness)

        public virtual MathElementKind Kind => MathElementKind.PolyDomain;
        public int Id { get; internal set; }
        private static int _idCounter = 1 + (int)MathElementKind.PolyDomain;
        public int CreationIndex => Id - (int)Kind - 1;
        private bool _isDirty = true;
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                foreach (var domain in Domains)
                {
                    domain.IsDirty = value;
                }
                foreach (var chain in _numberChains)
                {
                    chain.IsDirty = value;
                }
            }
        }
        public List<Domain> Domains { get; } = new List<Domain>(); // todo: sync with PolyNumberChain
        protected List<NumberChain> _numberChains { get; } = new List<NumberChain>(); // these sets always have the same number of elements.

        public int PolyCount => Domains.Count;
        public int Count => _numberChains[0].Count;

        public PolyDomain(params Domain[] domains)
        {
            Id = _idCounter++;
            if (domains.Length == 0)
            {
                throw new ArgumentException("Must have at least one number chain in poly domain.");
            }
            foreach (var domain in domains)
            {
                Domains.Add(domain);
                var nc = new NumberChain(domain, Polarity.Aligned);
                _numberChains.Add(nc);
                domain.AddNumber(nc, false);
            }
        }

        public Domain GetDomainOf(NumberChain chain) { var idx = GetChainIndex(chain); return idx > -1 ? Domains[idx] : null; }
        protected Domain GetDomainByName(string name) => Domains.FirstOrDefault(dm => dm.Name == name);
        protected int GetDomainIndex(Domain domain) => Domains.IndexOf(domain);
        protected Domain GetDomainByIndex(int index) => (index >= 0 && index < Domains.Count) ? Domains[index] : null;
        public NumberChain GetChainOf(Domain domain) { var idx = GetDomainIndex(domain); return idx > -1 ? _numberChains[idx] : null; }
        protected NumberChain GetChainByName(string name) { var idx = Domains.FindIndex(dm => dm.Name == name); return idx > -1 ? _numberChains[idx] : null; }
        protected int GetChainIndex(NumberChain chain) => _numberChains.IndexOf(chain); // can't use Id as there could be repeats of the same chain eventually.
        protected NumberChain GetChainByIndex(int index) => (index >=0 && index < _numberChains.Count) ? _numberChains[index] : null;





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
                    chain.MergeItem(values[i++]);
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
                    chain.MergeItem(values[i++]);
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
                    chain.MergeItem(values[i++], values[i++]); // these are segments, so (start, end)
                }
            }
        }
        public void AddPosition(params Range[] values)
        {
            if (values.Length == PolyCount)
            {
                int i = 0;
                foreach (var chain in _numberChains)
                {
                    chain.MergeItem(values[i++]);
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
                        _numberChains[j].MergeItem(values[i * PolyCount + j]);
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
                        _numberChains[j].MergeItem(values[i * PolyCount + j]);
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
                        _numberChains[j].MergeItem(values[i * PolyCount + j * 2], values[i * PolyCount + j * 2 + 1]);
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
                    chain.MergeItem(start, values[i++]);
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
        /// Helper method to get polyline style version of number vales, for rendering etc.
        /// Indexes in the form of [a0,a1,a2,a3],[b0,b1,b2,b3],[c0...
        /// </summary>
        public float[][] GetContiguousValues()
        {
            var len = Count * PolyCount;
            var result = new List<float[]>(len);
            if (len > 0)
            {
                var nums = GetInterleavedNumbers();
                // add all start values for first number set, then all end points.
                var item = new float[PolyCount];
                for (int i = 0; i < PolyCount; i++)
                {
                    item[i] = (float)nums[i].StartValue;
                }
                result.Add(item);

                for (int i = 0; i < nums.Count; i += PolyCount)
                {
                    item = new float[PolyCount];
                    for (int j = 0; j < PolyCount; j++)
                    {
                        item[j] = (float)nums[i + j].EndValue;
                    }
                    result.Add(item);
                }
            }

            return result.ToArray();
        }
        public void ResetWithContiguousValues(IEnumerable<float> values)
        {
            Reset();
            var nextStarts = new double[PolyCount];
            var ranges = new Range[PolyCount];
            int polyCounter = 0;
            int index = 0;
            double firstVal = 0;
            foreach (var value in values)
            {
                if (index <= 1 && polyCounter < PolyCount * 2) // first set uses both points
                {
                    if (polyCounter % 2 == 0)
                    {
                        firstVal = value;
                    }
                    else
                    {
                        var idx = (polyCounter - 1) / 2;
                        nextStarts[idx] = value;
                        ranges[idx] = new Range(firstVal, value);
                    }
                }
                else // rest of numbers, last tail with this tail, creating segments from polylines
                {
                    ranges[polyCounter] = new Range(nextStarts[polyCounter], value);
                    nextStarts[polyCounter] = value;
                }

                polyCounter++;
                if (polyCounter == PolyCount)
                {
                    if (index > 0)
                    {
                        AddPosition(ranges);
                        // no need to clear ranges as they will overwrite
                    }
                    polyCounter = 0;
                    index++;
                }
            }
        }
        /// <summary>
        /// Helper method to get polyline style version of focal positions, for rendering etc.
        /// </summary>
        public long[] GetContiguousPositions()
        {
            // focals are p0,p1, p1,p2, p2,p3...
            // need: p0,p1,p2,p3...
            var len = Count * PolyCount;
            var result = new List<long>(len);
            if (len > 0)
            {
                var focals = GetInterleavedFocals();
                result.Add(focals[0].StartPosition);
                result.Add(focals[0].EndPosition);
                for (int i = 1; i < focals.Count; i += 2)
                {
                    result.Add(focals[i].StartPosition);
                    result.Add(focals[i].EndPosition);
                }
            }

            return result.ToArray();
        }
        public void ResetWithContiguousPositions(long[] positions)
        {
            // comes in as x0,y0,x1,y1,x2,y2
            // postions are (x0,y0,x1,y1)(x1,y1,x2,y2)(x2,y2,x3,y3)
            var posLen = PolyCount * 2;
            if (positions.Length >= posLen)
            {
                Reset();
                var nextStarts = new long[PolyCount];
                for (int i = 0; i < PolyCount; i++)
                {
                    nextStarts[i] = positions[i];
                }
                var positionSet = new long[posLen];
                for (int i = PolyCount; i < positions.Length; i += PolyCount)
                {
                    for (int j = 0; j < PolyCount; j++)
                    {
                        positionSet[j] = nextStarts[j];
                    }
                    for (int j = 0; j < PolyCount; j++)
                    {
                        positionSet[j + PolyCount] = positions[i + j];
                        nextStarts[j] = positions[i + j];
                    }
                    AddPosition(positionSet);
                }
            }
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
