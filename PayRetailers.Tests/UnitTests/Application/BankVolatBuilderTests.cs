using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Options;
using PayRetailers.Application.Services;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Tests.UnitTests.Application;

public class BankVolatBuilderTests
{
    private readonly Mock<IBankvolatHttpClient> _clientMock = new();
    private readonly Mock<ICurrencyConverter> _converterMock = new();
    private readonly IOptions<AccountSettings> _options = Options.Create(new AccountSettings { LimitUsd = 150 });

    private BankVolatBuilder CreateService() =>
        new(_clientMock.Object, _converterMock.Object, _options);

    [Fact]
    public async Task BuildBankVolatAccountAsync_ShouldBuildAccountCorrectly()
    {
        // Arrange
        var accountId = "ACC456";
        var transactionId = Guid.NewGuid().ToString();

        _clientMock.Setup(x => x.GetAccountsAsync()).ReturnsAsync([
            new BankvolatAccount
            {
                Account = accountId,
                TotalAmount = 500,
                Currency = "EUR",
                Transactions = [ transactionId ]
            }
        ]);

        _clientMock.Setup(x => x.GetPersonalDetailsAsync(accountId)).ReturnsAsync(new BankvolatPersonalDetails
        {
            Name = "Ana",
            Surname = "Lopez",
            Address = "Calle Luna",
            Email = "ana@mail.com",
            PhoneNumber = "555123",
            Country = "ES"
        });

        _clientMock.Setup(x => x.GetTransactionByIdAsync(accountId, transactionId)).ReturnsAsync(new BankvolatTransaction
        {
            Id = Guid.Parse(transactionId),
            Target = "ACC999",
            Money = 100,
            Currency = "EUR",
            Status = BankvolatTransactionStatus.Validated
        });

        _converterMock.Setup(c => c.ConvertToUsd(It.IsAny<decimal>(), It.IsAny<string>()))
                      .Returns<decimal, string>((amount, currency) => currency == "EUR" ? amount * 1.1m : amount);

        var builder = CreateService();

        // Act
        var result = await builder.BuildBankVolatAccountAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.CustomerAccount.Should().Be(accountId);
        result.Provider.Should().Be(Provider.Bankvolat);
        result.CustomerDetails.Name.Should().Be("Ana");
        result.Transactions.Should().ContainSingle();
    }

    [Fact]
    public async Task BuildBankVolatAccountAsync_ShouldThrow_WhenAccountNotFound()
    {
        _clientMock.Setup(x => x.GetAccountsAsync()).ReturnsAsync([]);

        var builder = CreateService();

        var act = async () => await builder.BuildBankVolatAccountAsync("NOT_FOUND");

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Bankvolat account not found: NOT_FOUND");
    }

    [Fact]
    public async Task BuildBankVolatAccountAsync_ShouldThrow_WhenPersonalDetailsMissing()
    {
        var accId = "ACC999";

        _clientMock.Setup(x => x.GetAccountsAsync()).ReturnsAsync([
            new BankvolatAccount
            {
                Account = accId,
                TotalAmount = 100,
                Currency = "USD",
                Transactions = []
            }
        ]);

        _clientMock.Setup(x => x.GetPersonalDetailsAsync(accId)).ReturnsAsync((BankvolatPersonalDetails?)null);

        var builder = CreateService();

        var act = async () => await builder.BuildBankVolatAccountAsync(accId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Bankvolat account details not found: {accId}");
    }

    [Fact]
    public async Task BuildBankVolatAccountAsync_ShouldThrow_WhenTransactionNotFound()
    {
        var accId = "ACC000";
        var txId = "TX_NOT_FOUND";

        _clientMock.Setup(x => x.GetAccountsAsync()).ReturnsAsync([
            new BankvolatAccount
            {
                Account = accId,
                TotalAmount = 200,
                Currency = "USD",
                Transactions = [ txId ]
            }
        ]);

        _clientMock.Setup(x => x.GetPersonalDetailsAsync(accId)).ReturnsAsync(new BankvolatPersonalDetails
        {
            Name = "Pepe",
            Surname = "Sosa",
            Address = "Calle 123",
            Email = "pepe@mail.com",
            PhoneNumber = "123456",
            Country = "UY"
        });

        _clientMock.Setup(x => x.GetTransactionByIdAsync(accId, txId)).ReturnsAsync((BankvolatTransaction?)null);

        var builder = CreateService();

        var act = async () => await builder.BuildBankVolatAccountAsync(accId);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Transaction not found for account {accId} with ID {txId}");
    }
}
