using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Mappers;
using System.Text.Json.Serialization;

namespace PayRetailers.Application.DTOs;
public class TransactionDto
{
    public required Guid Id { get; set; }
    public required string CustomerSourceAccount { get; set; }
    public required string CustomerDestinationAccount { get; set; }
    public required int Amount { get; set; }
    public required string Currency { get; set; }
    
    [JsonConverter(typeof(TransactionTypeConverter))]
    public required TransactionType Type { get; set; }  // Money_in | Money_out
    
    public required TransactionStatus Status { get; set; } = default!; // Pending | Approve | Processed
}
