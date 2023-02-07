using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens
{
    public class ExpressionToken : ValueToken
    {
        public ExpressionToken(char operationSymbol)
            : base(TokenType.Expression)
        {
            switch (operationSymbol)
            {
                case '+':
                    Operation = ArithmeticOperation.Plus;
                    Priority = OperationOrder.PlusMinus;
                    break;
                case '-':
                    Operation = ArithmeticOperation.Minus;
                    Priority = OperationOrder.PlusMinus;
                    break;
                case '*':
                    Operation = ArithmeticOperation.Mul;
                    Priority = OperationOrder.MulDiv;
                    break;
                case '/':
                    Operation = ArithmeticOperation.Div;
                    Priority = OperationOrder.MulDiv;
                    break;
            }
        }

        public ValueToken Left { get; set; }

        public ValueToken Right { get; set; }

        public ArithmeticOperation Operation { get; }

        public override string ToString()
        {
            return $"[{Left}]{Operation}[{Right}]";
        }
    }
}