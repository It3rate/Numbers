namespace NumbersCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NumbersCore.Primitives;

    public static class OperationKindExtension
    {
        // FALSE (output is always false)
        private static Func<bool, bool, bool> FALSE = (x, y) => false;

        // AND (true if both inputs are true)
        private static Func<bool, bool, bool> AND = (x, y) => x && y;

        // AND-NOT (true if the first input is true and the second is false)
        private static Func<bool, bool, bool> AND_NOT = (x, y) => x && !y;

        // FIRST INPUT (output is the first input)
        private static Func<bool, bool, bool> FIRST_INPUT = (x, y) => x;

        // NOT-AND (true if the first input is false and the second is true) equivalent to Select-and-Complement
        private static Func<bool, bool, bool> NOT_AND = (x, y) => !x && y;

        // SECOND INPUT (output is the second input)
        private static Func<bool, bool, bool> SECOND_INPUT = (x, y) => y;

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
            var result = FIRST_INPUT;
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

                case OperationKind.FIRST_INPUT:
                    result = FIRST_INPUT;
                    break;

                case OperationKind.NOT_AND:
                    result = NOT_AND;
                    break;

                case OperationKind.SECOND_INPUT:
                    result = SECOND_INPUT;
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

                case OperationKind.NOT_SECOND_INPUT:
                    result = NOT_SECOND_INPUT;
                    break;

                case OperationKind.IF_THEN:
                    result = IF_THEN;
                    break;

                case OperationKind.NOT_FIRST_INPUT:
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
}
