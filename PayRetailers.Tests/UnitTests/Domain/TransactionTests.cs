using FluentAssertions;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Tests.UnitTests.Domain;

public class TransactionTests
{
    [Theory]
    [InlineData(PayBroTransactionStatus.Pending, TransactionStatus.Pending)]
    [InlineData(PayBroTransactionStatus.Approve, TransactionStatus.Approve)]
    [InlineData(PayBroTransactionStatus.Processed, TransactionStatus.Processed)]
    public void SetTransactionStatus_FromPayBro_ShouldMapCorrectly(PayBroTransactionStatus input, TransactionStatus expected)
    {
        var transaction = new Transaction
        {
            SourceAccount = "A",
            DestinationAccount = "B",
            Amount = 100,
            Currency = "USD"
        };

        transaction.SetTransactionStatus(input);

        transaction.Status.Should().Be(expected);
    }

    [Theory]
    [InlineData(BankvolatTransactionStatus.Created, TransactionStatus.Pending)]
    [InlineData(BankvolatTransactionStatus.Validated, TransactionStatus.Approve)]
    [InlineData(BankvolatTransactionStatus.Finished, TransactionStatus.Processed)]
    public void SetTransactionStatus_FromBankvolat_ShouldMapCorrectly(BankvolatTransactionStatus input, TransactionStatus expected)
    {
        var transaction = new Transaction
        {
            SourceAccount = "A",
            DestinationAccount = "B",
            Amount = 100,
            Currency = "USD"
        };

        transaction.SetTransactionStatus(input);

        transaction.Status.Should().Be(expected);
    }

    [Theory]
    [InlineData("ACC123", "OTHER", TransactionType.MoneyOut)]
    [InlineData("OTHER", "ACC123", TransactionType.MoneyIn)]
    public void SetType_ShouldDetermineDirectionCorrectly(string source, string destination, TransactionType expectedType)
    {
        var transaction = new Transaction
        {
            SourceAccount = source,
            DestinationAccount = destination,
            Amount = 50,
            Currency = "USD"
        };

        transaction.SetType("ACC123", "OTHER");

        transaction.Type.Should().Be(expectedType);
    }
}
