using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Mappers;
public static class BankvolatStatusMapper
{
    public static TransactionStatus Map(BankvolatTransactionStatus status) => status switch
    {
        BankvolatTransactionStatus.Created => TransactionStatus.Pending,
        BankvolatTransactionStatus.Validated => TransactionStatus.Approve,
        BankvolatTransactionStatus.Finished => TransactionStatus.Processed,
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}
