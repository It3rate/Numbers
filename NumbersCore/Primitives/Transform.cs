using NumbersCore.Utils;

namespace NumbersCore.Primitives
{
    // **** All ops, history, sequences, equations should fit on traits as focals.

    // ** All operations can be reduced to one or more proportional moves, maybe (select what point(s) move relative to what, for what lengths and repeat)
    // **        operations can be reduced to equations using only select>move primitives where amounts are recorded in segments as well.
    //   - fixed: elements remain at some proportional position regardless of other changes. (used in branches and joins).
    //   - shifts: points move together (non proportionately) to a proportional unit distance (this can just be two points doing multiplication)
    //   - addition: end proportional to beginning
    //   - multiplication: multiple points move proportionately  (by some 'additive' unit amount)
    //   - complex math: whole number line also moves proportionately
    //   - powers: repeating addition (becomes form of multiplication) or multiplication proportional moves, output becomes input.
    //   - turns: proportional move balances between two unit segments.

    // Operations on source(s)
    // Select (can add segments)
    // Multiply (stretch)
    // Add (jump, shift)
    // Repeat (can add segments if repeated op does)
    // Turn
    // Twist (3d)
    // Bool ops (can split segment)
    // Branch (splits segment) (connections are 0, 1 or 2 way links)
    // Blend (reduces segments)
    // Duplicate (adds segment)

    // Compare
    // Choose (can reduce unchosen segments)

    public delegate void TransformEventHandler(object sender, ITransform e);
    public interface ITransform : IMathElement
    {
	    Number Change { get; set; }
	    event TransformEventHandler StartTransformEvent;
	    event TransformEventHandler TickTransformEvent;
	    event TransformEventHandler EndTransformEvent;

	    void ApplyStart();
	    void ApplyEnd();
	    void ApplyPartial(long tickOffset);
    }

    public class Transform : TransformBase
    {
	    public override MathElementKind Kind => MathElementKind.Transform;

        public Transform(Selection source, Number change, TransformKind kind) : base(change, kind)
        {
	        Source = source;
        }

        public override void ApplyStart() { }
	    public override void ApplyEnd() { }
	    public override void ApplyPartial(long tickOffset) { }

    }

    public abstract class TransformBase : ITransform
    {
	    public abstract MathElementKind Kind { get; }

	    public int Id { get; set; }
	    public int CreationIndex => Id - (int)Kind - 1;
        public Brain Brain { get; }

        public TransformKind TransformKind { get; set; }
	    public Selection Source { get; set; } // the object being transformed
	    public Number Change { get; set; } // the amount to transform (can change per repeat)
	    public Number Repeat { get; set; } // the number of times to repeat (zero means unlimited, ended only by evaluation) 
	    public Number EvaluationTarget { get; set; } // the number to evaluate against to decide if complete

        protected TransformBase(Number change, TransformKind kind) // todo: add default numbers (0, 1, unot, -1 etc) in global domain.
        {
	        Brain = change.Brain;
	        Id = Brain.NextTransformId();
	        TransformKind = kind;
	        Change = change;
        }

        public event TransformEventHandler StartTransformEvent;
	    public event TransformEventHandler TickTransformEvent;
	    public event TransformEventHandler EndTransformEvent;

	    public virtual void ApplyStart() { OnStartTransformEvent(this); }
	    public virtual void ApplyPartial(long tickOffset) { OnTickTransformEvent(this); }
	    public virtual void ApplyEnd() { OnEndTransformEvent(this); }

	    public virtual bool Evaluate() => true;

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
    }

    public enum TransformKind
    {
	    None, // leave repeat in place
	    AppendAll, // repeat are added together ('regular' multiplication)
	    MultiplyAll, // repeat are multiplied together (exponents)
	    Blend, // multiply as in area, blend from unot to unit
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
