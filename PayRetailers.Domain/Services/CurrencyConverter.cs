namespace PayRetailers.Domain.Services;

public class CurrencyConverter : ICurrencyConverter
{
    private static readonly Dictionary<(string From, string To), decimal> Rates = new()
    {
        { ("USD", "USD"), 1m },
        { ("ARS", "USD"), 0.0013m },
        { ("EUR", "USD"), 1.10m },

        { ("USD", "ARS"), 1 / 0.0013m }, // ≈ 769.23
        { ("EUR", "ARS"), 1.10m / 0.0013m }, // ≈ 846.15

        { ("USD", "EUR"), 1 / 1.10m }, // ≈ 0.909
        { ("ARS", "EUR"), 0.0013m / 1.10m }, // ≈ 0.00118

        { ("EUR", "EUR"), 1m },
        { ("ARS", "ARS"), 1m }
    };

    public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (Rates.TryGetValue((fromCurrency.ToUpperInvariant(), toCurrency.ToUpperInvariant()), out var rate))
            return Math.Round(amount * rate, 4); // 4 decimal places for precision

        throw new NotSupportedException($"Conversion from {fromCurrency} to {toCurrency} is not supported.");
    }

    public decimal ConvertToUsd(decimal amount, string currency)
    {
        return Convert(amount, currency, "USD");
    }
}