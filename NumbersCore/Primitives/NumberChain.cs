using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    using System;
    using System.Collections;
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

        public new bool IsDirty { get => _focalChain.IsDirty; set => _focalChain.IsDirty = value; } // base just calls this

        private FocalChain _focalChain => (FocalChain)Focal;
        public int Count => _focalChain.Count;

        public override Domain Domain // todo: lookup domain on PolyDomain
        {
            get => base.Domain;
            set => base.Domain = value;
        }

        public NumberChain(Number targetNumber, params Focal[] focals) : base(new FocalChain(), targetNumber.Polarity)
        {
            Domain = targetNumber.Domain;
            _focalChain.MergeRange(focals);
        }
        public NumberChain(Domain domain, Polarity polarity, params Focal[] focals) : base(new FocalChain(), polarity)
        {
            Domain = domain;
            _focalChain.MergeRange(focals);
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

        public Focal Last() => _focalChain.Last();
        // todo: account for polarity
        public Focal CreateFocalFromRange(Range value) => Domain.CreateFocalFromRange(value);

        public void ComputeWith(Number num, OperationKind operationKind)
        {
            if (operationKind.IsBoolOp())
            {
                ComputeBoolOp(num.Focal, operationKind); 
            }
            else if (operationKind.IsUnary())
            {
                switch (operationKind)
                {
                    case OperationKind.None:
                        break;
                    case OperationKind.Negate:
                        Negate();
                        break;
                    case OperationKind.Reciprocal:
                        break;
                    case OperationKind.TogglePolarity:
                        break;
                    case OperationKind.FlipInPlace:
                        break;
                    case OperationKind.MirrorOnUnit:
                        break;
                    case OperationKind.MirrorOnUnot:
                        break;
                    case OperationKind.MirrorOnStart:
                        break;
                    case OperationKind.MirrorOnEnd:
                        break;
                    case OperationKind.FilterUnit:
                        break;
                    case OperationKind.FilterUnot:
                        break;
                    case OperationKind.FilterStart:
                        break;
                    case OperationKind.FilterEnd:
                        break;
                    case OperationKind.UnaryNot:
                        _focalChain.ComputeWith(num.Focal, operationKind);
                        break;
                }
            }
            else if (operationKind.IsBinary())
            {
                switch (operationKind)
                {
                    case OperationKind.Add:
                        AddValue(num);
                        break;
                    case OperationKind.Subtract:
                        SubtractValue(num);
                        break;
                    case OperationKind.Multiply:
                        MultiplyValue(num);
                        break;
                    case OperationKind.Divide:
                        DivideValue(num);
                        break;
                    case OperationKind.Root:
                        break;
                    case OperationKind.Wedge:
                        break;
                    case OperationKind.DotProduct:
                        break;
                    case OperationKind.GeometricProduct:
                        break;
                    case OperationKind.Blend:
                        break;
                }
            }
            else if (operationKind.IsTernary())
            {
                switch (operationKind)
                {
                    case OperationKind.PowerAdd:
                        break;
                    case OperationKind.PowerMultiply:
                        break;
                }
            }
            else
            {
                switch (operationKind)
                {
                    case OperationKind.None:
                        break;
                    case OperationKind.AppendAll:
                        break;
                    case OperationKind.MultiplyAll:
                        break;
                    case OperationKind.Average:
                        break;
                }
            }
        }
        public void ComputeWith(Focal focal, OperationKind operationKind) => _focalChain.ComputeWith(focal, operationKind);
        public void ComputeWith(long start, long end, OperationKind operationKind) => ComputeWith(new Focal(start, end), operationKind);
        public void ComputeWith(Range range, OperationKind operationKind)
        {
            var focal = Domain.CreateFocalFromRange(range);
            _focalChain.ComputeWith(focal, operationKind);
        }
        public void ComputeBoolOp(Focal focal, OperationKind operationKind)
        {
            // bool ops are just comparing state, so they don't care about direction or polarity, thus happen on focals
            // however, this requires they have the same resolutions, so really should be on number chains.
            _focalChain.ComputeWith(focal, operationKind); 
        }
        public void AddPosition(long start, long end)
        {
            _focalChain.AddPosition(start, end);
        }
        public void AddPosition(Focal focal)
        {
           AddPosition(focal.StartPosition, focal.EndPosition);
        }
        public void AddPosition(Number num)
        {
            AddPosition(num.Focal.StartPosition, num.Focal.EndPosition);
        }
        public void AddPosition(Range range)
        {
            var focal = Domain.CreateFocalFromRange(range);
            AddPosition(focal.StartPosition, focal.EndPosition);
        }
        public void RemoveLastPosition() => _focalChain.RemoveLastPosition();

        public void Reset(params Focal[] focals)
        {
            _focalChain.Clear();
            _focalChain.MergeRange(focals);
        }
        public Number this[int index] => index < Count ? Domain.CreateNumber(_focalChain[index], false) : null;
        public Focal FocalAt(int index) => index < Count ? _focalChain[index] : null;

        public Number SumAll()
        {
            var result = Domain.CreateNumber(new Focal(0,0));
            foreach (var number in InternalNumbers())
            {
                result.AddValue(number);
            }
            return result;
        }
    
        // todo: Add/Multiply all the internal segments as well. Adding may be ok as is, multiply needs to interpolate stretches
        public override void AddValue(Number q) { base.AddValue(q); }
        public override void SubtractValue(Number q) { base.SubtractValue(q); }
        public override void MultiplyValue(Number q) { base.MultiplyValue(q); } 
        public override void DivideValue(Number q) { base.DivideValue(q);}

        public void Not(Number q) { Reset(Focal.UnaryNot(q.Focal)); }



        public static bool operator ==(NumberChain a, NumberChain b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            if (a is null || b is null)
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(NumberChain a, NumberChain b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            return obj is NumberChain other && Equals(other);
        }
        public bool Equals(NumberChain value)
        {
            return ReferenceEquals(this, value) ||
                (
                    Polarity == value.Polarity &&
                    FocalChain.Equals(this._focalChain, value._focalChain)
                );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _focalChain.GetHashCode() * 17 ^ ((int)Polarity + 27) * 397;
                return hashCode;
            }
        }

        public override string ToString()
        {
            var v = Value;
            var midSign = v.End > 0 ? " + " : " ";
            var result = IsAligned ?
                $"nc:({v.UnotValue:0.##}i {midSign}{v.UnitValue}r)" :
                $"nc:~({v.UnitValue:0.##}r {midSign}{v.UnotValue:0.##}i)";
            return result;
        }
    }
}
