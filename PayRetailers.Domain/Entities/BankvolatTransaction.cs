using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Entities;
public class BankvolatTransaction
{
    public required Guid Id { get; set; }
    public required string Target { get; set; }
    public required int Money { get; set; }
    public required string Currency { get; set; }
    public required BankvolatTransactionStatus Status { get; set; }
}
