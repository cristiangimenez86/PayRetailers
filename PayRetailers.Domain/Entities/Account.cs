using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Domain.Entities;

public class Account(Provider provider, string customerAccount, decimal balance, string currency, decimal limitUsd, Customer customerDetails)
{
    //Account Details
    public Guid Id { get; init; } = Guid.NewGuid();
    public string CustomerAccount { get; init; } = customerAccount;
    public Provider Provider { get; init; } = provider;
    public decimal Balance { get; init; } = balance;
    public string Currency { get; init; } = currency;

    //Customer Details
    public Customer CustomerDetails { get; init; } = customerDetails;

    //Transactions
    private decimal _transactionsOutBalanceUsd;
    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions;
    public void AddTransaction(Transaction transaction, ICurrencyConverter converter)
    {
        _transactions.Add(transaction);

        if (transaction.Type != TransactionType.MoneyOut)
        {
            return;
        }

        var usdAmount = converter.ConvertToUsd(transaction.Amount, transaction.Currency);
        _transactionsOutBalanceUsd += usdAmount;
    }

    //Limit
    public decimal LimitDifference => _transactionsOutBalanceUsd - limitUsd;
    public bool IsLimitExceeded => _transactionsOutBalanceUsd > limitUsd;
}