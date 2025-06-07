using Microsoft.Extensions.Options;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.DTOs;
using PayRetailers.Application.Mappers;
using PayRetailers.Application.Options;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.Services;

public class AccountService(
    ICacheService cacheService,
    IPayBroBuilder payBroBuilder,
    IBankVolatBuilder bankVolatBuilder,
    IOptions<AccountSettings> accountOptions)
    : IAccountService
{
    private readonly decimal _limitUsd = accountOptions.Value.LimitUsd;
    public async Task<AccountDto> GetAccountDetailsAsync(string account)
    {
        var accountEntity = await GetAccountAsync(account);
        return DtoMapper.ToAccountDto(accountEntity);
    }

    public async Task<LimitCheckDto> CheckLimitAsync(string account, bool exceeded)
    {
        var accountEntity = await GetAccountAsync(account);
        return DtoMapper.ToLimitCheckDto(accountEntity, _limitUsd);
    }

    public Task<IEnumerable<BalanceDto>> GetFutureBalanceAsync(string account, bool isFuture)
    {
        throw new NotImplementedException();
    }

    private async Task<Account> GetAccountAsync(string account)
    {
        var accountProviders = await cacheService.GetAsync<List<CacheAccountProvider>>("AccountProviderList")
                               ?? throw new ApplicationException("Cache is not initialized.");

        var accountProvider = accountProviders.FirstOrDefault(x => x.Account == account)
                              ?? throw new KeyNotFoundException($"No provider found for account: {account}");

        return accountProvider.Provider switch
        {
            Provider.PayBro => await payBroBuilder.BuildPayBroAccountAsync(account),
            Provider.Bankvolat => await bankVolatBuilder.BuildBankVolatAccountAsync(account),
            _ => throw new ApplicationException($"Unsupported provider: {accountProvider.Provider}")
        };
    }
}
