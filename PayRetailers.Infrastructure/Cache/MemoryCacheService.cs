using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PayRetailers.Application.Contracts;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Infrastructure.Cache;

public class MemoryCacheService(
    ILogger<MemoryCacheService> logger,
    IMemoryCache memoryCache,
    IPayBroHttpClient payBroHttpClient,
    IBankvolatHttpClient bankvolatHttpClient
    ) : ICacheService
{
    private const string AccountProviderCacheKey = "AccountProviderList";

    public async Task InitializeAsync()
    {
        
        var payBroAccount = await payBroHttpClient.GetAccountsAsync();
        var bankvolatAccount = await bankvolatHttpClient.GetAccountsAsync();

        var accountProviderList = new List<CacheAccountProvider>();
        accountProviderList.AddRange(payBroAccount.Select(x => new CacheAccountProvider
        {
            Account = x.Account,
            Provider = Provider.PayBro
        }));

        accountProviderList.AddRange(bankvolatAccount.Select(x => new CacheAccountProvider
        {
            Account = x.Account,
            Provider = Provider.Bankvolat
        }));

        await SetAsync(AccountProviderCacheKey, accountProviderList, TimeSpan.FromDays(2));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        try
        {
            memoryCache.Set(key, value, expiration);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting cache for key {Key}", key);
        }

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        try
        {
            if (memoryCache.TryGetValue(key, out T? value))
            {
                return Task.FromResult(value);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting cache for key {Key}", key);
        }

        return Task.FromResult<T?>(default);
    }

    public Task RemoveAsync(string key)
    {
        try
        {
            memoryCache.Remove(key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cache for key {Key}", key);
        }

        return Task.CompletedTask;
    }
}