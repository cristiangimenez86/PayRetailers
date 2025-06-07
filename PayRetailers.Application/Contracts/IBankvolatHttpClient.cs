using PayRetailers.Domain.Entities;

namespace PayRetailers.Application.Contracts;
public interface IBankvolatHttpClient
{
    Task<IReadOnlyCollection<BankvolatAccount>> GetAccountsAsync();
    Task<BankvolatPersonalDetails?> GetPersonalDetailsAsync(string account);
    Task<BankvolatTransaction?> GetTransactionByIdAsync(string account, string transactionId);
}
