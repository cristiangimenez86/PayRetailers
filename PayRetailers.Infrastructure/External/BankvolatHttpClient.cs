using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PayRetailers.Application.Contracts;
using PayRetailers.Domain.Entities;

namespace PayRetailers.Infrastructure.External;
public class BankvolatHttpClient(ILogger<BankvolatHttpClient> logger, HttpClient httpClient) : IBankvolatHttpClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public async Task<IReadOnlyCollection<BankvolatAccount>> GetAccountsAsync()
    {
        const string endpoint = "https://wiremock-api.azurewebsites.net/test-code/bankvolat/accounts"; //TODO: Get from environment variables

        try
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var accounts = await response.Content.ReadFromJsonAsync<List<BankvolatAccount>>(JsonOptions);
            if (accounts == null)
            {
                logger.LogWarning("No accounts found at {Endpoint}", endpoint);
                return [];
            }

            return accounts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching Bankvolat accounts");
            return [];
        }
    }

    public async Task<BankvolatPersonalDetails?> GetPersonalDetailsAsync(string account)
    {
        var endpoint = $"https://wiremock-api.azurewebsites.net/test-code/bankvolat/accounts/{Uri.EscapeDataString(account)}"; //TODO: Get from environment variables

        try
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BankvolatPersonalDetails>(JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching personal details for account {Account}", account);
            return null;
        }
    }

    public async Task<BankvolatTransaction?> GetTransactionByIdAsync(string account, string transactionId)
    {
        var endpoint = $"https://wiremock-api.azurewebsites.net/test-code/bankvolat/accounts/{Uri.EscapeDataString(account)}/transactions/{Uri.EscapeDataString(transactionId)}"; //TODO: Get from environment variables

        try
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BankvolatTransaction>(JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching transaction {TransactionId} for account {Account}", transactionId, account);
            return null;
        }
    }
}