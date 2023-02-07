using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens;

public class NumberToken : ValueToken
{
    public NumberToken()
        : base(TokenType.Number)
    {
    }
}