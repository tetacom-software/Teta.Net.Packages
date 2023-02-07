using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens
{
	public class FunctionToken : ValueToken
	{
		private readonly string _name;

        public FunctionToken(string name)
	        : base(TokenType.Function)
        {
	        _name = name;
        }

        public List<Token> Args { get; } = new List<Token>();

        public override string ToString()
		{
			return "function:" + _name;
		}
	}
}