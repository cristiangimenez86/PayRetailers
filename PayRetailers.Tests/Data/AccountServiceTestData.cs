using Moq;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Tests.Data;

public static class AccountServiceTestData
{
    public static List<CacheAccountProvider> GetValidCacheData() =>
    [
        new CacheAccountProvider { Account = "ACC123", Provider = Provider.PayBro },
        new CacheAccountProvider { Account = "ACC456", Provider = Provider.Bankvolat }
    ];

    public static Account CreateAccountWithTransactions(bool exceedLimit, out decimal expectedUsdOut)
    {
        var limit = 150m;
        expectedUsdOut = 0;

        var customer = new Customer
        {
            Name = "Juan",
            Surname = "Pérez",
            Address = "Calle Falsa 123",
            PhoneNumber = "+34123456789",
            Email = "juan@mail.com"
        };

        var account = new Account(
            provider: Provider.PayBro,
            customerAccount: "ACC123",
            balance: 1000m,
            currency: "USD",
            limitUsd: limit,
            customerDetails: customer
        );

        var converter = new Mock<ICurrencyConverter>();
        converter.Setup(c => c.ConvertToUsd(It.IsAny<decimal>(), It.IsAny<string>()))
                 .Returns<decimal, string>((amount, currency) => currency switch
                 {
                     "USD" => amount,
                     "EUR" => amount * 1.1m,
                     "ARS" => amount * 0.0013m,
                     _ => throw new ArgumentOutOfRangeException()
                 });

        var tx1 = new Transaction
        {
            SourceAccount = "ACC123",
            DestinationAccount = "ACC999",
            Amount = 100, // USD
            Currency = "USD"
        };
        tx1.SetType(tx1.SourceAccount, tx1.DestinationAccount);
        tx1.SetTransactionStatus(PayBroTransactionStatus.Processed);
        account.AddTransaction(tx1, converter.Object);
        expectedUsdOut += converter.Object.ConvertToUsd(tx1.Amount, tx1.Currency);

        var tx2 = new Transaction
        {
            SourceAccount = "ACC123",
            DestinationAccount = "ACC998",
            Amount = exceedLimit ? 100 : 30, 
            Currency = "EUR"
        };
        tx2.SetType(tx2.SourceAccount, tx2.DestinationAccount);
        tx2.SetTransactionStatus(PayBroTransactionStatus.Processed);
        account.AddTransaction(tx2, converter.Object);
        expectedUsdOut += converter.Object.ConvertToUsd(tx2.Amount, tx2.Currency);

        return account;
    }
}