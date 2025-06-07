using FluentAssertions;
using PayRetailers.Domain.Services;

namespace PayRetailers.Tests.UnitTests.Domain;

public class CurrencyConverterTests
{
    private readonly CurrencyConverter _converter = new();

    [Theory]
    [InlineData(100, "USD", "USD", 100)]
    [InlineData(100, "ARS", "USD", 0.13)]
    [InlineData(100, "EUR", "USD", 110)]
    [InlineData(1, "USD", "ARS", 769.2308)]
    [InlineData(1, "USD", "EUR", 0.9091)]
    [InlineData(100, "ARS", "EUR", 0.1182)]
    public void Convert_ShouldReturnCorrectResult(decimal amount, string from, string to, decimal expected)
    {
        var result = _converter.Convert(amount, from, to);

        result.Should().BeApproximately(expected, 0.0001m);
    }

    [Theory]
    [InlineData(100, "USD", 100)]
    [InlineData(200, "EUR", 220)]
    [InlineData(1000, "ARS", 1.3)]
    public void ConvertToUsd_ShouldReturnCorrectUsd(decimal amount, string currency, decimal expectedUsd)
    {
        var result = _converter.ConvertToUsd(amount, currency);

        result.Should().BeApproximately(expectedUsd, 0.0001m);
    }

    [Fact]
    public void Convert_ShouldThrow_WhenConversionIsNotSupported()
    {
        var act = () => _converter.Convert(50, "GBP", "USD");

        act.Should().Throw<NotSupportedException>()
            .WithMessage("Conversion from GBP to USD is not supported.");
    }
}