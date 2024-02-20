namespace Numbers.Primitives
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;
    using NumbersCore.Utils;

    public class TextElement : IMathElement
    {
        public MathElementKind Kind => MathElementKind.None;
        private static int _idCounter = 1;
        public int Id { get; }
        public int CreationIndex => Id - (int)Kind;

        public string[] Lines { get; set; }
        // font, color etc
        public TextElement(params string[] lines)
        {
            Id = _idCounter++;
            Lines = lines;
        }
    }
}
