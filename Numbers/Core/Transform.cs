﻿using System.Runtime.InteropServices.WindowsRuntime;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

    // Operations on selection(s)
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

    public class Transform : TransformBase
    {

        // account for repeat of transform, use stack to enable back selection
        public Selection Selection { get; set; }
        //public Number Amount { get; set; } // hmm, repeat are the amount, repeat are joined in place (area in 2d), then can be added (reg mult), multiplied (powers)

        //public List<List<int>> History; // or start states, or this is just computable by running in reverse unless involving random.

        public Transform(Selection selection, Number repeat, TransformKind kind) : base(repeat, kind)
        {
	        Selection = selection;
        }
        public override void ApplyStart() { }
	    public override void ApplyEnd() { }
	    public override void ApplyPartial(long tickOffset) { }

    }

    public delegate void TransformEventHandler(object sender, ITransform e);
    public interface ITransform : IMathElement
    {
	    int Id { get; set; }
        Number Repeat { get; set; }
        event TransformEventHandler StartTransformEvent;
	    event TransformEventHandler TickTransformEvent;
	    event TransformEventHandler EndTransformEvent;

	    void ApplyStart();
	    void ApplyEnd();
	    void ApplyPartial(long tickOffset);
    }

    public abstract class TransformBase : ITransform
    {
	    public MathElementKind Kind => MathElementKind.Transform;
	    private static int transformCounter = 1 + (int)MathElementKind.Transform;

	    public int Id { get; set; }
	    public TransformKind TransformKind { get; set; }
        public Number Repeat { get; set; }

        protected TransformBase( Number repeat, TransformKind kind) // todo: add default numbers (0, 1, unot, -1 etc) in global domain.
        {
	        Id = transformCounter++;
	        TransformKind = kind;
	        Repeat = repeat;
        }

        public event TransformEventHandler StartTransformEvent;
	    public event TransformEventHandler TickTransformEvent;
	    public event TransformEventHandler EndTransformEvent;

	    public virtual void ApplyStart() { }
	    public virtual void ApplyEnd() { }
	    public virtual void ApplyPartial(long tickOffset) { }
    }

    public enum TransformKind
    {
	    None, // leave repeat in place
	    AppendAll, // repeat are added together ('regular' multiplication)
        MultiplyAll, // repeat are multiplied together (exponents)
        Blend, // multiply as in area, blend from unot to unit
    }
}
