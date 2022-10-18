using System.Runtime.InteropServices.WindowsRuntime;

namespace Numbers.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class Transform : TransformBase
    {
	    // account for repeats of transform, use stack to enable back selection
        public TransformKind TransformKind { get; set; }
        public Selection Selection { get; set; }
        public Number Amount { get; set; }

        //public List<List<int>> History; // or start states, or this is just computable by running in reverse unless involving random.

        public override void ApplyStart() { }
	    public override void ApplyEnd() { }
	    public override void ApplyPartial(long tickOffset) { }

    }

    public delegate void TransformEventHandler(object sender, ITransform e);
    public interface ITransform
    {
	    int Repeats { get; set; }
	    event TransformEventHandler StartTransformEvent;
	    event TransformEventHandler TickTransformEvent;
	    event TransformEventHandler EndTransformEvent;

	    void ApplyStart();
	    void ApplyEnd();
	    void ApplyPartial(long tickOffset);
    }

    public abstract class TransformBase : ITransform
    {
	    public int Repeats { get; set; }

	    public event TransformEventHandler StartTransformEvent;
	    public event TransformEventHandler TickTransformEvent;
	    public event TransformEventHandler EndTransformEvent;

	    public virtual void ApplyStart() { }
	    public virtual void ApplyEnd() { }
	    public virtual void ApplyPartial(long tickOffset) { }
    }

    public enum TransformKind
    {
        Shift,
        PreserveRatio,
    }
}
