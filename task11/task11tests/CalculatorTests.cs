using System;
using Xunit;
using task11;

namespace task11tests;

public class CalculatorTests
{
    private readonly ICalculator _calculator;

    public CalculatorTests()
    {
        _calculator = DynamicCompiler.CreateCalculatorInstance();
    }

    [Fact]
    public void DynamicCalculator_Add_ShouldReturnCorrectSum()
    {
        int result = _calculator.Add(15, 10);
        Assert.Equal(25, result);
    }

    [Fact]
    public void DynamicCalculator_Minus_ShouldReturnCorrectDifference()
    {
        int result = _calculator.Minus(30, 12);
        Assert.Equal(18, result);
    }

    [Fact]
    public void DynamicCalculator_Mul_ShouldReturnCorrectProduct()
    {
        int result = _calculator.Mul(6, 7);
        Assert.Equal(42, result);
    }

    [Fact]
    public void DynamicCalculator_Div_ShouldReturnCorrectQuotient()
    {
        int result = _calculator.Div(100, 4);
        Assert.Equal(25, result);
    }

    [Fact]
    public void DynamicCalculator_DivByZero_ShouldThrowException()
    {
        Assert.Throws<DivideByZeroException>(() => _calculator.Div(10, 0));
    }
}
