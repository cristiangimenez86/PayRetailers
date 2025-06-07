using PayRetailers.Domain.Entities;

namespace PayRetailers.Application.Contracts;
public interface IBankVolatBuilder
{
    Task<Account> BuildBankVolatAccountAsync(string account);
}
