namespace PayRetailers.Application.Contracts;
public interface ICacheService
{
    Task InitializeAsync();
    Task SetAsync<T>(string key, T value, TimeSpan expiration);
    Task<T?> GetAsync<T>(string key);
    Task RemoveAsync(string key);
}
