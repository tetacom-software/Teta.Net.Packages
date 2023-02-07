using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens
{
	public abstract class Token
	{
		protected Token(TokenType tokenType)
		{
			TokenType = tokenType;
		}

		public int Priority { get; set; }

		public TokenType TokenType { get; }
	}
}