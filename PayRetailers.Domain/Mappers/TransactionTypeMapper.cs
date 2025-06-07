using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Mappers;
public static class TransactionTypeMapper
{
    public static TransactionType Map(string currentAccount, string destinationAccount)
        => currentAccount.Equals(destinationAccount, StringComparison.OrdinalIgnoreCase)
            ? TransactionType.MoneyIn
            : TransactionType.MoneyOut;
}
