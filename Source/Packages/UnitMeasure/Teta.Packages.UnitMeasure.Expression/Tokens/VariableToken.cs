using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens
{
	public class VariableToken : ValueToken
	{
        public VariableToken(string value)
            : base(TokenType.Operand)
        {
            Source = value;
        }

		public string Source { get; }

        public override string ToString()
        {
            return Source;
        }
    }
}