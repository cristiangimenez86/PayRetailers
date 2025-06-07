using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using PayRetailers.Application.Contracts;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Infrastructure.Cache;

namespace PayRetailers.Tests.UnitTests.Infrastructure;

public class MemoryCacheServiceTests
{
    private readonly Mock<ILogger<MemoryCacheService>> _loggerMock = new();
    private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
    private readonly Mock<IPayBroHttpClient> _payBroMock = new();
    private readonly Mock<IBankvolatHttpClient> _bankvolatMock = new();

    private MemoryCacheService CreateService() =>
        new(_loggerMock.Object, _memoryCache, _payBroMock.Object, _bankvolatMock.Object);

    [Fact]
    public async Task SetAndGet_ShouldCacheAndRetrieveValue()
    {
        var service = CreateService();

        var key = "TestKey";
        var value = new List<string> { "one", "two" };

        await service.SetAsync(key, value, TimeSpan.FromMinutes(1));

        var result = await service.GetAsync<List<string>>(key);

        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(value);
    }

    [Fact]
    public async Task Remove_ShouldDeleteFromCache()
    {
        var service = CreateService();

        var key = "TestKeyToRemove";
        await service.SetAsync(key, 123, TimeSpan.FromMinutes(5));

        await service.RemoveAsync(key);
        var result = await service.GetAsync<int>(key);

        result.Should().Be(default);
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadAccountsIntoCache()
    {
        _payBroMock.Setup(c => c.GetAccountsAsync()).ReturnsAsync([
            new PayBroAccount
            {
                Account = "PB1",
                Currency = "USD",
                TotalAmount = 10,
                PersonalDetails = new PayBroPersonalDetails
                {
                    Name = "Test",
                    Surname = "User",
                    Email = "test@mail.com",
                    PhoneNumber = "123456",
                    Address = "Calle 123"
                }
            }
        ]);

        _bankvolatMock.Setup(c => c.GetAccountsAsync()).ReturnsAsync([
            new BankvolatAccount
            {
                Account = "BV1",
                Currency = "USD",
                TotalAmount = 20,
                Transactions = []
            }
        ]);

        var service = CreateService();

        // Act
        await service.InitializeAsync();

        // Assert
        var result = await service.GetAsync<List<CacheAccountProvider>>("AccountProviderList");

        result.Should().NotBeNull().And.HaveCount(2);
        result.Should().ContainSingle(x => x.Account == "PB1" && x.Provider == Provider.PayBro);
        result.Should().ContainSingle(x => x.Account == "BV1" && x.Provider == Provider.Bankvolat);
    }


    [Fact]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyNotFound()
    {
        var service = CreateService();

        var result = await service.GetAsync<string>("NonExistingKey");

        result.Should().BeNull();
    }
}
