using System;
using System.Collections.Generic;
using NumbersCore.CoreConcepts.Counter;
using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    // **** All ops, history, sequences, equations should fit on traits as focals.

    // Operations on source(s)
    // Select (add, can create or add to or shift selection)
    // Unary - Invert (apply only polarity change, no multiply), Negate, Not, flip arrow direction, mirror on start point/endpoint
    // Multiply (stretch)
    // Steps (part of an interpolation op, can break into subsegments)
    // Repeat (can add/duplicate segments if repeated op does)
    // Causal directions (one time, one way, two way, result only, locks)
    // Select partial (either unit, unot)
    // Links (link domains, basis, focals)
    // Bool ops (like contain, repel,interpolate until etc)
    // Branch (bool ops can split segment, resulting in choice?)

    public class Transform : ITransform
    {
	    public MathElementKind Kind => MathElementKind.Transform;

	    public int Id { get; set; }
	    public int CreationIndex => Id - (int)Kind - 1;
        public Brain Brain { get; }

        public TransformKind TransformKind { get; set; }
        public bool IsUnary => TransformKind.IsUnary();
        public int Repeats { get; set;  } = 1;
        public int Steps { get; set; } = 1;
	    public Number Left { get; set; } // the object being transformed
	    public Number Right { get; set; } // the amount to transform (can change per repeat)
        public NumberChain Result { get; set; } // current result of transform

        public UpCounter RepeatCounter { get; } = new UpCounter();
        public UpCounter StepCounter { get; } = new UpCounter();
        public Evaluation HaltCondition { get; set; } // the evaluation that decides if the transform can continue
        public bool IsActive { get; private set; }

        public IEnumerable<Number> UsedNumbers()
        {
            yield return Left;

            if (!IsUnary)
            {
                yield return Right;
            }

            yield return Result;
        }

        public Transform(Number left, Number right, TransformKind kind) // todo: add default numbers (0, 1, unot, -1 etc) in global domain.
        {
	        Left = left;
	        Right = right;
            Result = new NumberChain(Right.Domain.MinMaxNumber);// left.Clone(false);
	        TransformKind = kind;
	        Brain = right.Brain;
	        Id = Brain.NextTransformId();
        }

        public event TransformEventHandler StartTransformEvent;
	    public event TransformEventHandler TickTransformEvent;
	    public event TransformEventHandler EndTransformEvent;

        public bool Involves(Number num) => (Left.Id == num.Id || Right.Id == num.Id || Result.Id == num.Id);
        public void Apply()
        {
            ApplyStart();
            ApplyEnd();
        }
	    public void ApplyStart()
        {
            Result.SetWith(Left);
            OnStartTransformEvent(this);
		    IsActive = true;
		    RepeatCounter.AddOne();
	    }
	    public void ApplyPartial(long tickOffset) { OnTickTransformEvent(this); }
	    public void ApplyEnd()
	    {
            switch (TransformKind)
            {
                case TransformKind.Add:
                    Result.Add(Right);
                    break;
                case TransformKind.Subtract:
                    Result.Subtract(Right);
                    break;
                case TransformKind.Multiply:
                    Result.Multiply(Right);
                    break;
                case TransformKind.Divide:
                    Result.Divide(Right);
                    break;
                case TransformKind.And:
                    Result.And(Right);
                    break;
                case TransformKind.Or:
                    Result.Or(Right);
                    break;
                case TransformKind.Nand:
                    Result.Nand(Right);
                    break;
            }
		    OnEndTransformEvent(this);
		    IsActive = false;
	    }

	    public bool Evaluate() => true;
	    public bool IsComplete() => HaltCondition?.EvaluateFlags() ?? true;

	    protected virtual void OnStartTransformEvent(ITransform e)
	    {
		    StartTransformEvent?.Invoke(this, e);
	    }

	    protected virtual void OnTickTransformEvent(ITransform e)
	    {
		    TickTransformEvent?.Invoke(this, e);
	    }

	    protected virtual void OnEndTransformEvent(ITransform e)
	    {
		    EndTransformEvent?.Invoke(this, e);
	    }
        public override string ToString()
        {
            var symbol = TransformKind.GetSymbol();
            return $"{Left} {symbol} {Right} = {Result}";
        }
    }

    public delegate void TransformEventHandler(object sender, ITransform e);
    public interface ITransform : IMathElement
    {
	    Number Right { get; set; }
	    event TransformEventHandler StartTransformEvent;
	    event TransformEventHandler TickTransformEvent;
	    event TransformEventHandler EndTransformEvent;
        void Apply();
        void ApplyStart();
	    void ApplyEnd();
	    void ApplyPartial(long tickOffset);
	    bool IsComplete();
    }

    public enum TransformKind
    {
	    None, // leave repeat in place

        Negate,
        TogglePolarity,
        FlipInPlace,
        MirrorOnUnit,
        MirrorOnUnot,
        MirrorOnStart,
        MirrorOnEnd,
        FilterUnit, // always 0 to r value
        FilterUnot, // always 0 to i value
        FilterStart, // depends on polarity
        FilterEnd, // depends on polarity
        And,
        Or,
        Not,
        Nand,
        Xor, // 16 binary truth table ops

        // Binary
        Add,
        Subtract,
        Multiply,
        Divide,
        PowerAdd,
        PowerMultiply,
        Reciprocal,

        AppendAll, // repeat are added together ('regular' multiplication)
	    MultiplyAll, // repeat are multiplied together (exponents)
	    Blend, // multiply as in area, blend from unot to unit
        Average,
        Root,
        Wedge,
        DotProduct,
        GeometricProduct,
    }
    public static class TransformKindExtensions
    {
        public static bool IsUnary(this TransformKind kind)
        {
            return (kind > TransformKind.None) && (kind < TransformKind.Add);
        }
        public static string GetSymbol(this TransformKind kind)
        {
            var result = "☼";
            switch (kind)
            {
                case TransformKind.Add:
                    result = "+";
                    break;
                case TransformKind.Negate:
                case TransformKind.Subtract:
                    result = "-";
                    break;
                case TransformKind.Multiply:
                    result = "*";
                    break;
                case TransformKind.Divide:
                    result = "/";
                    break;
                case TransformKind.PowerMultiply:
                    result = "^";
                    break;
                case TransformKind.And:
                    result = "&";
                    break;
                case TransformKind.Or:
                    result = "|";
                    break;
                case TransformKind.Not:
                    result = "!";
                    break;
                case TransformKind.Nand:
                    result = "^&";
                    break;
            }
            return result;
        }
    }

    // todo: Evaluation of two segments are very much like the 16 bool operations, probably the same.
    public enum EvaluationKind
    {
        // can use overlap, equal, contain, not contain, minmax ranges, unit range, basis direction, number direction...

	    None, // Never continue
        InTarget, // continue unless result escapes containment of EvaluationTarget
        NotInTarget,  // continue unless result enters containment of EvaluationTarget
        TargetIn,  // continue unless EvaluationTarget escapes containment of result
        NotTargetIn,  // continue unless EvaluationTarget enters containment of result
        Always, // always continue
    }
}
