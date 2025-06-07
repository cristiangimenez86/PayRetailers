using System.Text.Json.Serialization;
using PayRetailers.Domain.Mappers;

namespace PayRetailers.Domain.Enums;

[JsonConverter(typeof(TransactionTypeConverter))]
public enum TransactionType
{
    MoneyIn,
    MoneyOut
}