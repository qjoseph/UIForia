using System;

namespace Src {

    public class OperatorBindingInt : ExpressionBinding {

        public ExpressionBinding left;
        public ExpressionBinding right;
        public OperatorType operatorType;

        public int Evaluate() {
//            switch (operatorType) {
//                case OperatorType.Plus:
//                    return (int) left.Evaluate() + (int) right.Evaluate();
//                case OperatorType.Minus:
//                    return (int) left.Evaluate() - (int) right.Evaluate();
//                case OperatorType.Times:
//                    return (int) left.Evaluate() * (int) right.Evaluate();
//                case OperatorType.Divide:
//                    return (int) left.Evaluate() / (int) right.Evaluate();
//            }
            throw new Exception("Unknown operator");
        }

    }

}