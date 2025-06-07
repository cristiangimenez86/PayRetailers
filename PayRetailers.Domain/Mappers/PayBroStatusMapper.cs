using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Mappers;
public static class PayBroStatusMapper
{
    public static TransactionStatus Map(PayBroTransactionStatus status) => status switch
    {
        PayBroTransactionStatus.Pending => TransactionStatus.Pending,
        PayBroTransactionStatus.Approve => TransactionStatus.Approve,
        PayBroTransactionStatus.Processed => TransactionStatus.Processed,
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };
}
