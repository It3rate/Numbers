namespace NumbersCore.Primitives
{
    using System;

    public enum OperationKind
    {
        None,

        // bool
        FALSE,
        AND,
        AND_NOT,
        A,
        NOT_AND,
        B,
        XOR,
        OR,
        NOR,
        XNOR,
        NOT_B,
        IF_THEN,
        NOT_A,
        THEN_IF,
        NAND,
        TRUE,

        // unary
        Negate,
        Reciprocal,
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
        UnaryNot, // Flip In Place (same segment meaning, persepctive inverted)

        // Binary
        Add,
        Subtract,
        Multiply,
        Divide,
        Root,
        Wedge,
        DotProduct,
        GeometricProduct,
        Blend, // multiply as in area, blend from unot to unit


        // ternary
        PowerAdd,
        PowerMultiply,


        AppendAll, // repeat are added together ('regular' multiplication)
        MultiplyAll, // repeat are multiplied together (exponents)
        Average,
    }
    public static class OperationKindExtension
    {
        public static bool IsBoolOp(this OperationKind kind)
        {
            return kind >= OperationKind.FALSE && kind <= OperationKind.TRUE;
        }
        public static bool IsUnary(this OperationKind kind)
        {
            return kind >= OperationKind.Negate && kind <= OperationKind.UnaryNot;
        }
        public static bool IsBinary(this OperationKind kind)
        {
            return kind >= OperationKind.Add && kind <= OperationKind.Blend;
        }
        public static bool IsTernary(this OperationKind kind)
        {
            return kind >= OperationKind.PowerAdd && kind <= OperationKind.PowerMultiply;
        }
        public static bool IsMultipleOp(this OperationKind kind)
        {
            return kind >= OperationKind.AppendAll && kind <= OperationKind.Average;
        }
        public static string GetSymbol(this OperationKind kind)
        {
            var result = "☼";
            switch (kind)
            {
                case OperationKind.Add:
                    result = "+";
                    break;
                case OperationKind.Negate:
                case OperationKind.Subtract:
                    result = "-";
                    break;
                case OperationKind.Multiply:
                    result = "*";
                    break;
                case OperationKind.Divide:
                    result = "/";
                    break;
                case OperationKind.PowerMultiply:
                    result = "^";
                    break;
                case OperationKind.AND:
                    result = "&";
                    break;
                case OperationKind.OR:
                    result = "|";
                    break;
                case OperationKind.NOT_B:
                    result = "!";
                    break;
                case OperationKind.NAND:
                    result = "^&";
                    break;
            }
            return result;
        }

        // FALSE (output is always false)
        private static Func<bool, bool, bool> FALSE = (x, y) => false;

        // AND (true if both inputs are true)
        private static Func<bool, bool, bool> AND = (x, y) => x && y;

        // AND-NOT (true if the first input is true and the second is false)
        private static Func<bool, bool, bool> AND_NOT = (x, y) => x && !y;

        // FIRST INPUT (output is the first input)
        private static Func<bool, bool, bool> A = (x, y) => x;

        // NOT-AND (true if the first input is false and the second is true) equivalent to Select-and-Complement
        private static Func<bool, bool, bool> NOT_AND = (x, y) => !x && y;

        // SECOND INPUT (output is the second input)
        private static Func<bool, bool, bool> B = (x, y) => y;

        // XOR (true if inputs are different)
        private static Func<bool, bool, bool> XOR = (x, y) => x ^ y;

        // OR (true if at least one input is true)
        private static Func<bool, bool, bool> OR = (x, y) => x || y;

        // NOR (true if both inputs are false)
        private static Func<bool, bool, bool> NOR = (x, y) => !(x || y);

        // XNOR (true if inputs are the same)
        private static Func<bool, bool, bool> XNOR = (x, y) => !(x ^ y);

        // NOT SECOND INPUT (output is the negation of the second input)
        private static Func<bool, bool, bool> NOT_SECOND_INPUT = (x, y) => !y;

        // IF-THEN (true if the first input is false or both are true) equivalent to logical implication
        private static Func<bool, bool, bool> IF_THEN = (x, y) => !x || y;

        // NOT FIRST INPUT (output is the negation of the first input)
        private static Func<bool, bool, bool> NOT_FIRST_INPUT = (x, y) => !x;

        // THEN-IF (true if the second input is false or both are true) equivalent to converse implication
        private static Func<bool, bool, bool> THEN_IF = (x, y) => x || !y;

        // NAND (true if at least one input is false)
        private static Func<bool, bool, bool> NAND = (x, y) => !(x && y);

        // TRUE (output is always true)
        private static Func<bool, bool, bool> TRUE = (x, y) => true;
        public static Func<bool, bool, bool> GetFunc(this OperationKind kind)
        {
            var result = A;
            switch (kind)
            {
                case OperationKind.FALSE:
                    result = FALSE;
                    break;

                case OperationKind.AND:
                    result = AND;
                    break;

                case OperationKind.AND_NOT:
                    result = AND_NOT;
                    break;

                case OperationKind.A:
                    result = A;
                    break;

                case OperationKind.NOT_AND:
                    result = NOT_AND;
                    break;

                case OperationKind.B:
                    result = B;
                    break;

                case OperationKind.XOR:
                    result = XOR;
                    break;

                case OperationKind.OR:
                    result = OR;
                    break;

                case OperationKind.NOR:
                    result = NOR;
                    break;

                case OperationKind.XNOR:
                    result = XNOR;
                    break;

                case OperationKind.NOT_B:
                    result = NOT_SECOND_INPUT;
                    break;

                case OperationKind.IF_THEN:
                    result = IF_THEN;
                    break;

                case OperationKind.NOT_A:
                    result = NOT_FIRST_INPUT;
                    break;

                case OperationKind.THEN_IF:
                    result = THEN_IF;
                    break;

                case OperationKind.NAND:
                    result = NAND;
                    break;

                case OperationKind.TRUE:
                    result = TRUE;
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
