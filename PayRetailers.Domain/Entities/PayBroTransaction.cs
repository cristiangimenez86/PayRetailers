using System.Text.Json.Serialization;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Entities;
public class PayBroTransaction
{
    [JsonPropertyName("dest_account")]
    public required string DestinationAccount { get; set; }
    public required int Amount { get; set; }
    public required string Currency { get; set; }
    public required PayBroTransactionStatus Status { get; set; }
}
