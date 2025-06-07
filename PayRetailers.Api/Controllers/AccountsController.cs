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
        try
        {
            var dto = await accountService.GetAccountDetailsAsync(account);
            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(GetAccountDetails));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }

    // GET /accounts/{account}/limits?exceeded=true
    [HttpGet("{account}/limits")]
    public async Task<IActionResult> CheckLimit(string account, [FromQuery] bool exceeded = false)
    {
        try
        {
            var dto = await accountService.CheckLimitAsync(account, exceeded);
            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(CheckLimit));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }

    // GET /accounts/{account}/balances?isFuture=true
    [HttpGet("{account}/balances")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFutureBalance(string account, [FromQuery] bool isFuture)
    {
        try
        {
            var dto = await accountService.GetFutureBalanceAsync(account, isFuture);
            return Ok(dto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(GetFutureBalance));
            return Problem(statusCode: 500, title: "Internal Server Error");
        }
    }
}