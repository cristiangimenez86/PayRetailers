using PayRetailers.Domain.Entities;

namespace PayRetailers.Contracts.PayBro;

/// <summary>
/// Response-message containing the accounts retrieved by the Worker.
/// </summary>
public record PayBroAccountsResponse(IReadOnlyCollection<PayBroAccount> Accounts);