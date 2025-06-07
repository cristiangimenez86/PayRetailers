using PayRetailers.Domain.Entities;

namespace PayRetailers.Application.Contracts;
public interface IPayBroHttpClient
{
    Task<IReadOnlyCollection<PayBroAccount>> GetAccountsAsync();
    Task<IReadOnlyCollection<PayBroTransaction>> GetTransactionsByAccountAsync(string account);
}
