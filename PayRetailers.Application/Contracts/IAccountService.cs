using PayRetailers.Application.DTOs;

namespace PayRetailers.Application.Contracts;
public interface IAccountService
{
    Task<AccountDto> GetAccountDetailsAsync(string account);
    Task<LimitCheckDto> CheckLimitAsync(string account, bool exceeded);
    Task<IEnumerable<BalanceDto>> GetFutureBalanceAsync(string account, bool isFuture);
}
