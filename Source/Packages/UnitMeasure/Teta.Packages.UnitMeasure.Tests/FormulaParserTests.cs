using System.Diagnostics;
using Teta.Packages.UnitMeasure.Expression;

namespace Teta.Packages.UnitMeasure.Tests;

public class FormulaParserTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FirstParse()
    {
        var exp = new FormulaParser().Parse("x * (54.32 + x/1.3)/12 + 5");
        Execute(() =>
        {
            for (var i = 0; i < 100000; i++)
            {
                var result = exp.Evaluate(i);
                Assert.That(i * (54.32 + i / 1.3) / 12 + 5, Is.EqualTo(result));
            }
        });
    }

    private static void Execute(Action action)
    {
        var sb = Stopwatch.StartNew();
        action();
        sb.Stop();
        TestContext.WriteLine(sb.ElapsedMilliseconds);
    }
}