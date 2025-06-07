using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Mappers;

namespace PayRetailers.Domain.Entities;

public class Transaction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string SourceAccount { get; init; }
    public required string DestinationAccount { get; init; }
    public required int Amount { get; init; }
    public required string Currency { get; init; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }

    public void SetTransactionStatus(PayBroTransactionStatus status)
    {
        Status = PayBroStatusMapper.Map(status);
    }

    public void SetTransactionStatus(BankvolatTransactionStatus status)
    {
        Status = BankvolatStatusMapper.Map(status);
    }

    public void SetType(string sourceAccount, string destinationAccount)
    {
        Type = TransactionTypeMapper.Map(sourceAccount, destinationAccount);
    }
}