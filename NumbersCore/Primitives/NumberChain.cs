using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Ordered numbers, array style, clamped to base Number size. Optionally can be merged and ordered with bool Operations.
    /// All Numbers are probably NumberChain, with zero based Unit and Unot values cancelling on overlap.
    /// Can represent a boolean evaluation of two numbers that result in non overlapping ordered segments, which share polarity and domain.
    /// Can also represent the start/endpoints of mergable repeats, like 3+3+3 or 3*3*3. 
    /// </summary>
    public class NumberChain : Number, IMathElement
    { 
        // should be able to access and update proportionally, where 8 even subdivisions will remain so even when changing the base number.
        // they can be recorded as on/off/on/off along the line, allowing construction of 2D continuous paths (though this would just be an optimization).
        // knowing if focals are touching (contiguous) matters.
        // all clamping, overlap removal and repears should be done in the focalSet
	    public override MathElementKind Kind => MathElementKind.NumberChain;

        private FocalChain _focalChain => (FocalChain)Focal;
        public int Count => _focalChain.Count;


        public NumberChain(Number targetNumber, params Focal[] focals) : base(new FocalChain(), targetNumber.Polarity)
        {
            Domain = targetNumber.Domain;
	        _focalChain.AddRange(focals);
        }

        public IEnumerable<Number> InternalNumbers()
        {
            foreach (var focal in _focalChain.Focals())
            {
                var nm = new Number(focal, Polarity);
                nm.Domain = Domain;
                yield return nm;
            }
        }

        public void Reset(Number left, OperationKind operationKind)
        {
            _focalChain.Reset(left.Focal);
            Polarity = left.Polarity;
        }

        // todo: account for polarity
        public void AddItem(Number num, OperationKind operationKind = OperationKind.None) { _focalChain.Add(num.Focal, operationKind); }
        public void AddItem(Focal focal, OperationKind operationKind = OperationKind.None) { _focalChain.Add(focal, operationKind); }

        public void Reset(params Focal[] focals)
        {
            _focalChain.Clear();
            _focalChain.AddRange(focals);
        }
        public Number this[int index] => index < _focalChain.Count ? Domain.CreateNumber(_focalChain[index], false) : null;

        public Number SumAll()
        {
            var result = Domain.CreateNumber(new Focal(0,0));
            foreach (var number in InternalNumbers())
            {
                result.Add(number);
            }
            return result;
        }
    
        // todo: Add/Multiply all the internal segments as well. Adding may be ok as is, multiply needs to interpolate stretches
        public override void Add(Number q) { base.Add(q); }
        public override void Subtract(Number q) { base.Subtract(q); }
        public override void Multiply(Number q) { base.Multiply(q); } 
        public override void Divide(Number q) { base.Divide(q);}

        public void Not(Number q) { Reset(Focal.UnaryNot(q.Focal)); }
        public override string ToString()
        {
            var v = Value;
            var midSign = v.End > 0 ? " + " : " ";
            var result = IsAligned ?
                $"nc:({v.UnotValue:0.##}i ... {midSign}{v.UnitValue}r)" :
                $"nc:~({v.UnitValue:0.##}r ... {midSign}{v.UnotValue:0.##}i)";
            return result;
        }
    }
}
