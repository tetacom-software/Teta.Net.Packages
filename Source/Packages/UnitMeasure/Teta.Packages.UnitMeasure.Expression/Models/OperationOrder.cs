namespace Teta.Packages.UnitMeasure.Expression.Models
{
    /// <summary>
    /// The order of operations is a collection of rules that reflect conventions about
    /// which procedures to perform first in order to evaluate a given mathematical expression.
    /// </summary>
    public static class OperationOrder
    {
        public const int PlusMinus = 1;
        public const int MulDiv = 2;
        public const int Bracket = 3;
    }
}