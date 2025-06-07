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

public class PayBroBuilderTests
{
    private readonly Mock<IPayBroHttpClient> _clientMock = new();
    private readonly Mock<ICurrencyConverter> _converterMock = new();
    private readonly IOptions<AccountSettings> _options = Options.Create(new AccountSettings { LimitUsd = 150 });

    private PayBroBuilder CreateService() =>
        new(_clientMock.Object, _converterMock.Object, _options);

    [Fact]
    public async Task BuildPayBroAccountAsync_ShouldReturnAccountWithTransactions()
    {
        // Arrange
        var accountId = "ACC123";

        _clientMock.Setup(c => c.GetAccountsAsync()).ReturnsAsync([
            new PayBroAccount
            {
                Account = accountId,
                TotalAmount = 500,
                Currency = "USD",
                PersonalDetails = new PayBroPersonalDetails
                {
                    Name = "Carlos",
                    Surname = "Gomez",
                    Address = "Calle Sur",
                    Email = "carlos@mail.com",
                    PhoneNumber = "888-123"
                }
            }
        ]);

        _clientMock.Setup(c => c.GetTransactionsByAccountAsync(accountId)).ReturnsAsync([
            new PayBroTransaction
            {
                DestinationAccount = "DEST001",
                Amount = 100,
                Currency = "USD",
                Status = PayBroTransactionStatus.Processed
            },
            new PayBroTransaction
            {
                DestinationAccount = "DEST002",
                Amount = 200,
                Currency = "EUR",
                Status = PayBroTransactionStatus.Pending
            }
        ]);

        _converterMock.Setup(c => c.ConvertToUsd(It.IsAny<decimal>(), It.IsAny<string>()))
                      .Returns<decimal, string>((amount, currency) => currency switch
                      {
                          "USD" => amount,
                          "EUR" => amount * 1.1m,
                          _ => throw new ArgumentOutOfRangeException()
                      });

        var builder = CreateService();

        // Act
        var result = await builder.BuildPayBroAccountAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result.CustomerAccount.Should().Be(accountId);
        result.Provider.Should().Be(Provider.PayBro);
        result.CustomerDetails.Name.Should().Be("Carlos");
        result.Transactions.Should().HaveCount(2);
        result.Transactions.First().Currency.Should().Be("USD");
    }

    [Fact]
    public async Task BuildPayBroAccountAsync_ShouldThrow_WhenAccountNotFound()
    {
        // Arrange
        _clientMock.Setup(c => c.GetAccountsAsync()).ReturnsAsync([]);

        var builder = CreateService();

        // Act
        var act = async () => await builder.BuildPayBroAccountAsync("UNKNOWN_ACC");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("PayBro account not found: UNKNOWN_ACC");
    }
}
