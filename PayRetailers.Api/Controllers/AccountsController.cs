using Microsoft.AspNetCore.Mvc;
using PayRetailers.Application.Contracts;

namespace PayRetailers.Api.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController(
    IAccountService accountService,
    ILogger<AccountsController> logger) : ControllerBase
{
    // GET /accounts/{account}
    [HttpGet("{account}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountDetails(string account)
    {
        var dto = await accountService.GetAccountDetailsAsync(account);
        return Ok(dto);
    }

    // GET /accounts/{account}/limits?exceeded=true
    [HttpGet("{account}/limits")]
    public async Task<IActionResult> CheckLimit(string account, [FromQuery] bool exceeded = true)
    {
        var dto = await accountService.CheckLimitAsync(account, exceeded);
        return Ok(dto);
    }

    // GET /accounts/{account}/balances?isFuture=true
    [HttpGet("{account}/balances")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFutureBalance(string account, [FromQuery] bool isFuture = true)
    {
        var dto = await accountService.GetFutureBalanceAsync(account, isFuture);
        return Ok(dto);
    }
}