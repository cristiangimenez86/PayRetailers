using FluentAssertions;
using Moq;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Tests.UnitTests.Domain;

public class AccountTests
{
    private static Customer GetCustomer() => new()
    {
        Name = "Test",
        Surname = "User",
        Address = "123 Calle",
        Email = "test@mail.com",
        PhoneNumber = "123456"
    };

    [Fact]
    public void AddTransaction_ShouldAddMoneyOutAndAffectLimit()
    {
        // Arrange
        var converter = new Mock<ICurrencyConverter>();
        converter.Setup(c => c.ConvertToUsd(100, "USD")).Returns(100m);

        var account = new Account(Provider.PayBro, "ACC123", 1000, "USD", 150, GetCustomer());

        var tx = new Transaction
        {
            SourceAccount = "ACC123",
            DestinationAccount = "ACC999",
            Amount = 100,
            Currency = "USD"
        };
        tx.SetType("ACC123", "ACC999");
        tx.SetTransactionStatus(PayBroTransactionStatus.Processed);

        // Act
        account.AddTransaction(tx, converter.Object);

        // Assert
        account.Transactions.Should().HaveCount(1);
        account.IsLimitExceeded.Should().BeFalse();
        account.LimitDifference.Should().BeApproximately(-50m, 0.001m);
    }

    [Fact]
    public void AddTransaction_ShouldIgnoreMoneyInForLimit()
    {
        // Arrange
        var converter = new Mock<ICurrencyConverter>();
        var account = new Account(Provider.PayBro, "ACC123", 1000, "USD", 150, GetCustomer());

        var tx = new Transaction
        {
            SourceAccount = "ACC999",
            DestinationAccount = "ACC123",
            Amount = 500,
            Currency = "USD"
        };
        tx.SetType("ACC999", "ACC123");
        tx.SetTransactionStatus(PayBroTransactionStatus.Approve);

        // Act
        account.AddTransaction(tx, converter.Object);

        // Assert
        account.IsLimitExceeded.Should().BeFalse();
        account.LimitDifference.Should().Be(0);
    }

    [Fact]
    public void AddTransaction_ShouldExceedLimit_WhenUsdAmountIsHigh()
    {
        // Arrange
        var converter = new Mock<ICurrencyConverter>();
        converter.Setup(c => c.ConvertToUsd(300, "USD")).Returns(300m);

        var account = new Account(Provider.Bankvolat, "ACC999", 2000, "USD", 150, GetCustomer());

        var tx = new Transaction
        {
            SourceAccount = "ACC999",
            DestinationAccount = "ANY",
            Amount = 300,
            Currency = "USD"
        };
        tx.SetType("ACC999", "ANY");
        tx.SetTransactionStatus(BankvolatTransactionStatus.Created);

        // Act
        account.AddTransaction(tx, converter.Object);

        // Assert
        account.IsLimitExceeded.Should().BeTrue();
        account.LimitDifference.Should().BeApproximately(150, 0.001m);
    }
}
