using Teta.Packages.UnitMeasure.Expression.Models;

namespace Teta.Packages.UnitMeasure.Expression.Tokens;

public class OpenBracketToken : Token
{
    public OpenBracketToken()
        : base(TokenType.OpenBracket)
    {
        Priority = OperationOrder.Bracket;
    }

    public override string ToString()
    {
        return "(";
    }
}