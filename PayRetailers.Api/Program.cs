using Microsoft.EntityFrameworkCore;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Services;
using PayRetailers.Domain.Mappers;
using PayRetailers.Domain.Repositories;
using PayRetailers.Domain.Services;
using PayRetailers.Infrastructure.Cache;
using PayRetailers.Infrastructure.DbContexts;
using PayRetailers.Infrastructure.External;
using PayRetailers.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new TransactionTypeConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPayBroBuilder, PayBroBuilder>();
builder.Services.AddScoped<IBankVolatBuilder, BankVolatBuilder>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TransactionsConnectionString")));

// Converters
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();

// HttpClients
builder.Services.AddHttpClient<IBankvolatHttpClient, BankvolatHttpClient>();
builder.Services.AddHttpClient<IPayBroHttpClient, PayBroHttpClient>();

// Cache registration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddMemoryCache();
    builder.Services.AddScoped<ICacheService, MemoryCacheService>();
}
else
{
    //Redis
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}

var app = builder.Build();

//Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cache and Database initialization
using (var scope = app.Services.CreateScope())
{
    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
    await cacheService.InitializeAsync();

    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
