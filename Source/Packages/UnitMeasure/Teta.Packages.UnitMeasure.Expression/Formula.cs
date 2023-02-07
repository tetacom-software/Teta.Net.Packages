using Teta.Packages.UnitMeasure.Expression.Models;
using Teta.Packages.UnitMeasure.Expression.Tokens;

namespace Teta.Packages.UnitMeasure.Expression
{
	public class Formula
	{
        /// <summary>
        /// The operand contains the current value of the indicator
        /// </summary>
        private const string DefaultOperand = "x";

		internal List<ExpressionToken> Order;
		internal string Source;

		public readonly SortedList<string, ValueToken> Operands = new();

        public double Evaluate(double value)
        {
            Operands[DefaultOperand].Value = value;
            return Evaluate();
		}

        private double Evaluate()
        {
	        foreach (var oper in Order)
	        {
		        oper.Value = oper.Operation switch
		        {
			        ArithmeticOperation.Plus => oper.Left.Value + oper.Right.Value,
			        ArithmeticOperation.Minus => oper.Left.Value - oper.Right.Value,
			        ArithmeticOperation.Div => oper.Left.Value / oper.Right.Value,
			        ArithmeticOperation.Mul => oper.Left.Value * oper.Right.Value,
			        _ => oper.Value
		        };
	        }

	        return Order[^1].Value;
        }

		public override string ToString()
		{
			return Source;
		}
	}
}