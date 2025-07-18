public class DynamicCalculatorTests : IDisposable
{
    // Метод для получения калькулятора
    private static ICalculator GetCalculator()
    {
        return CalcBuilder.GetCalculator();
    }

    public DynamicCalculatorTests()
    {
    }

    public void Dispose()
    {
    }

    [Fact]
    public void TestCalculator_Addition()
    {
        var calc = GetCalculator();
        Assert.NotNull(calc);
        Assert.Equal(5, calc.Add(2, 3));
    }

    [Fact]
    public void TestCalculator_Subtraction()
    {
        var calc = GetCalculator();
        Assert.NotNull(calc);
        Assert.Equal(1, calc.Minus(3, 2));
    }

    [Fact]
    public void TestCalculator_Multiplication()
    {
        var calc = GetCalculator();
        Assert.NotNull(calc);
        Assert.Equal(6, calc.Mul(2, 3));
    }

    [Fact]
    public void TestCalculator_Division()
    {
        var calc = GetCalculator();
        Assert.NotNull(calc);
        Assert.Equal(2, calc.Div(6, 3));
    }

    [Fact]
    public void TestCalculator_Division_ByZero()
    {
        var calc = GetCalculator();
        Assert.NotNull(calc);
        Assert.Throws<DivideByZeroException>(() => calc.Div(6, 0));
    }
}
