using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Options;
using PayRetailers.Application.Services;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Repositories;
using PayRetailers.Domain.Services;
using PayRetailers.Infrastructure.Cache;
using PayRetailers.Infrastructure.DbContexts;
using PayRetailers.Infrastructure.Providers;
using PayRetailers.Infrastructure.Repositories;

namespace PayRetailers.Tests.IntegrationTests;

public abstract class TestBase : IAsyncLifetime
{
    protected IServiceProvider ServiceProvider { get; private set; } = null!;
    protected IServiceScope Scope { get; private set; } = null!;
    protected ApplicationDbContext DbContext => Scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    protected IAccountService AccountService => Scope.ServiceProvider.GetRequiredService<IAccountService>();
    protected IDocumentService DocumentService => Scope.ServiceProvider.GetRequiredService<IDocumentService>();

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        // Configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        services.Configure<AccountSettings>(configuration.GetSection("AccountSettings"));
        services.Configure<ProviderApiEndpoints>(configuration.GetSection("ProviderApiEndpoints"));

        // Infrastructure
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("TransactionsConnectionString"));
        });

        services.AddMemoryCache();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<ICurrencyConverter, CurrencyConverter>();

        services.AddHttpClient<IPayBroHttpClient, PayBroHttpClient>();
        services.AddHttpClient<IBankvolatHttpClient, BankvolatHttpClient>();

        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IPayBroBuilder, PayBroBuilder>();
        services.AddScoped<IBankVolatBuilder, BankVolatBuilder>();

        // Repositories
        services.AddScoped<IDocumentRepository, DocumentRepository>();

        // Logging
        services.AddLogging(config => config.AddConsole());

        ServiceProvider = services.BuildServiceProvider(validateScopes: true);
        Scope = ServiceProvider.CreateScope();

        // cache
        var cacheService = Scope.ServiceProvider.GetRequiredService<ICacheService>();
        await cacheService.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        Scope.Dispose();
        return Task.CompletedTask;
    }
}
