using System;
using System.Collections.Generic;
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

        public List<NumberChain> NumberChains { get; set; }

        //public int Count => PolyFocal.Count;

        //public PolyNumber(Domain domain, FocalSet focals)
        //{
        //    Domain = domain;
        //    PolyFocal = focals;
        //}
        //public void AddPosition(long position)
        //{
        //    PolyFocal.AddPosition(position);
        //}
        //public void AddPositions(long[] positions)
        //{
        //    PolyFocal.AddPositions(positions);
        //}
        //public void RemoveLastPosition()
        //{
        //    PolyFocal.RemoveLastPosition();
        //}
        //public void AddValue(double val)
        //{
        //    var pos = Domain.GetValueOf(val);
        //    PolyFocal.AddPosition(pos);
        //}

        //public Seg ValueAt(int index)
        //{
        //    return Domain.GetValueOf(PolyFocal.FocalAt(index));
        //}
        //public double ValueAtPosition(int index)
        //{
        //    return Domain.GetValueOf(PolyFocal.PositionAt(index)).End;
        //}
        //public double[] Positions()
        //{
        //    var result = new double[PolyFocal.Length];
        //    for (int i = 0; i < PolyFocal.Count; i++)
        //    {
        //        result[i] = ValueAtPosition(i);
        //    }
        //    return result;
        //}

        //public void Reset()
        //{
        //    PolyFocal.Clear();
        //}
    }
}
