using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Options;
using PayRetailers.Domain.Entities;

namespace PayRetailers.Infrastructure.Providers;
public class PayBroHttpClient(
    ILogger<PayBroHttpClient> logger, 
    HttpClient httpClient,
    IOptions<ProviderApiEndpoints> apiOptions) : IPayBroHttpClient
{
    private readonly string _baseUrl = apiOptions.Value.PayBroBaseUrl;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyCollection<PayBroAccount>> GetAccountsAsync()
    {
        var endpoint = $"{_baseUrl}/accounts";

        try
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var accounts = await response.Content.ReadFromJsonAsync<List<PayBroAccount>>(JsonOptions);
            if(accounts == null)
            {
                logger.LogWarning($"No accounts found at {endpoint}");
                return [];
            }
            
            return accounts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, nameof(PayBroHttpClient));
            return [];
        }
    }

    public async Task<IReadOnlyCollection<PayBroTransaction>> GetTransactionsByAccountAsync(string account)
    {
        var endpoint = $"{_baseUrl}/accounts/{Uri.EscapeDataString(account)}/transactions";

        try
        {
            using var response = await httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var transactions = await response.Content.ReadFromJsonAsync<List<PayBroTransaction>>(JsonOptions);
            if (transactions == null)
            {
                logger.LogWarning($"No transactions found for account {account}");
                return [];
            }

            return transactions;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
