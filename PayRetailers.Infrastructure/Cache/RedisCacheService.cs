
using PayRetailers.Application.Contracts;

namespace PayRetailers.Infrastructure.Cache;
public class RedisCacheService : ICacheService
{
    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string key)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new NotImplementedException();
    }
}
