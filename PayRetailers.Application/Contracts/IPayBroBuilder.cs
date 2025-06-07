using PayRetailers.Domain.Entities;

namespace PayRetailers.Application.Contracts;
public interface IPayBroBuilder
{
    Task<Account> BuildPayBroAccountAsync(string account);
}
