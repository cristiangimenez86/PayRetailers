namespace PayRetailers.Domain.Services;
public interface ICurrencyConverter
{
    decimal ConvertToUsd(decimal amount, string currency);
    decimal Convert(decimal amount, string fromCurrency, string toCurrency);
}
