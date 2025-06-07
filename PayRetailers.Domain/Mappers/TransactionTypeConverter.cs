using PayRetailers.Domain.Enums;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace PayRetailers.Domain.Mappers;
public class TransactionTypeConverter : JsonConverter<TransactionType>
{
    public override TransactionType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()?.ToLower() switch
        {
            "money_in" => TransactionType.MoneyIn,
            "money_out" => TransactionType.MoneyOut,
            _ => throw new JsonException($"Invalid TransactionType value: {reader.GetString()}")
        };
    }

    public override void Write(Utf8JsonWriter writer, TransactionType value, JsonSerializerOptions options)
    {
        var stringValue = value switch
        {
            TransactionType.MoneyIn => "Money_in",
            TransactionType.MoneyOut => "Money_out",
            _ => throw new JsonException($"Invalid TransactionType enum: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
