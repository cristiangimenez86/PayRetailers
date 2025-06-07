using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Options;
using PayRetailers.Application.Services;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Tests.Data;

namespace PayRetailers.Tests.UnitTests.Application;

public class AccountServiceTests
{
    private readonly Mock<ICacheService> _cacheMock = new();
    private readonly Mock<IPayBroBuilder> _payBroMock = new();
    private readonly Mock<IBankVolatBuilder> _bankVolatMock = new();
    private readonly IOptions<AccountSettings> _options = Options.Create(new AccountSettings { LimitUsd = 150 });

    private AccountService CreateService() =>
        new(_cacheMock.Object, _payBroMock.Object, _bankVolatMock.Object, _options);

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldReturnDto_WhenAccountIsValid()
    {
        // Arrange
        var data = AccountServiceTestData.GetValidCacheData();
        var account = AccountServiceTestData.CreateAccountWithTransactions(exceedLimit: false, out _);

        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
                  .ReturnsAsync(data);

        _payBroMock.Setup(b => b.BuildPayBroAccountAsync("ACC123"))
                   .ReturnsAsync(account);

        var service = CreateService();

        // Act
        var result = await service.GetAccountDetailsAsync("ACC123");

        // Assert
        result.CustomerAccount.Should().Be("ACC123");
        result.Name.Should().Be("Juan");
        result.Provider.Should().Be(Provider.PayBro);
        result.Transactions.Should().HaveCount(account.Transactions.Count);
    }

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldThrow_WhenCacheIsNull()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
                  .ReturnsAsync((List<CacheAccountProvider>?)null);

        var service = CreateService();

        // Act
        var act = async () => await service.GetAccountDetailsAsync("ACC123");

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Cache is not initialized.");
    }

    [Fact]
    public async Task GetAccountDetailsAsync_ShouldThrow_WhenAccountNotFound()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
                  .ReturnsAsync(new List<CacheAccountProvider>());

        var service = CreateService();

        // Act
        var act = async () => await service.GetAccountDetailsAsync("ACC123");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("No provider found for account: ACC123");
    }

    [Fact]
    public async Task CheckLimitAsync_ShouldReturnCorrectDto_WhenLimitIsNotExceeded()
    {
        // Arrange
        var data = AccountServiceTestData.GetValidCacheData();
        var account = AccountServiceTestData.CreateAccountWithTransactions(exceedLimit: false, out var usdOut);
        var limit = _options.Value.LimitUsd;

        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
            .ReturnsAsync(data);

        _payBroMock.Setup(b => b.BuildPayBroAccountAsync("ACC123"))
            .ReturnsAsync(account);

        var service = CreateService();

        // Act
        var result = await service.CheckLimitAsync("ACC123", exceeded: false);

        // Assert
        result.Should().NotBeNull();
        result.IsExceeded.Should().BeFalse();
        result.CurrentLimit.Should().Be(limit);
        result.Currency.Should().Be("USD");
        result.Difference.Should().BeApproximately(Math.Abs(account.LimitDifference), 0.001m);
    }

    [Fact]
    public async Task CheckLimitAsync_ShouldReturnCorrectDto_WhenLimitIsExceeded()
    {
        // Arrange
        var data = AccountServiceTestData.GetValidCacheData();
        var account = AccountServiceTestData.CreateAccountWithTransactions(exceedLimit: true, out var usdOut);
        var limit = _options.Value.LimitUsd;

        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
            .ReturnsAsync(data);

        _payBroMock.Setup(b => b.BuildPayBroAccountAsync("ACC123"))
            .ReturnsAsync(account);

        var service = CreateService();

        // Act
        var result = await service.CheckLimitAsync("ACC123", exceeded: true);

        // Assert
        result.Should().NotBeNull();
        result.IsExceeded.Should().BeTrue();
        result.CurrentLimit.Should().Be(limit);
        result.Currency.Should().Be("USD");
        result.Difference.Should().BeApproximately(account.LimitDifference, 0.001m);
    }

    [Fact]
    public async Task CheckLimitAsync_ShouldThrow_WhenCacheIsNull()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
            .ReturnsAsync((List<CacheAccountProvider>?)null);

        var service = CreateService();

        // Act
        var act = async () => await service.CheckLimitAsync("ACC123", exceeded: false);

        // Assert
        await act.Should().ThrowAsync<ApplicationException>()
            .WithMessage("Cache is not initialized.");
    }

    [Fact]
    public async Task CheckLimitAsync_ShouldThrow_WhenAccountNotFound()
    {
        // Arrange
        _cacheMock.Setup(c => c.GetAsync<List<CacheAccountProvider>>("AccountProviderList"))
            .ReturnsAsync(new List<CacheAccountProvider>()); // empty list

        var service = CreateService();

        // Act
        var act = async () => await service.CheckLimitAsync("ACC123", exceeded: false);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("No provider found for account: ACC123");
    }
}
