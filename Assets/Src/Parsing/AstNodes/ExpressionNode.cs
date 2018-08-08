using System;

namespace Src {

    public abstract class ExpressionNode : ASTNode {

        public readonly ExpressionNodeType expressionType;


        protected ExpressionNode(ExpressionNodeType expressionType) {
            this.expressionType = expressionType;
        }

        public abstract Type GetYieldedType(ContextDefinition context);

    }

}