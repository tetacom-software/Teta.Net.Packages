using System.Globalization;
using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens;

public class ValueToken : Token
{
    public ValueToken(TokenType tokenType)
        : base(tokenType)
    {
    }

    public double Value { get; set; }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}