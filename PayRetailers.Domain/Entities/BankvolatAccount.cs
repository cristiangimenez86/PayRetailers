using System.Text.Json.Serialization;

namespace PayRetailers.Domain.Entities;
public class BankvolatAccount
{
    public required string Account { get; set; }
    public required int TotalAmount { get; set; }
    public required string Currency { get; set; }
    
    [JsonPropertyName("transacctions")]
    public List<string> Transactions { get; set; } = [];
}
