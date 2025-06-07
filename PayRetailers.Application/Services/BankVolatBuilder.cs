using Microsoft.Extensions.Options;
using PayRetailers.Application.Contracts;
using PayRetailers.Application.Options;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Application.Services;
public class BankVolatBuilder(
    IBankvolatHttpClient bankvolatHttpClient, 
    ICurrencyConverter currencyConverter,
    IOptions<AccountSettings> accountOptions) : IBankVolatBuilder
{
    private readonly decimal _limitUsd = accountOptions.Value.LimitUsd;
    public async Task<Account> BuildBankVolatAccountAsync(string account)
    {
        var bankVolatAccounts = await bankvolatHttpClient.GetAccountsAsync();
        var bankVolatAccount = bankVolatAccounts.FirstOrDefault(a => a.Account.Equals(account));
        if (bankVolatAccount is null)
        {
            throw new KeyNotFoundException($"Bankvolat account not found: {account}");
        }

        var bankVolatAccountDetails = await bankvolatHttpClient.GetPersonalDetailsAsync(account);
        if (bankVolatAccountDetails is null)
        {
            throw new KeyNotFoundException($"Bankvolat account details not found: {account}");
        }

        var customerEntity = new Customer
        {
            Name = bankVolatAccountDetails.Name,
            Surname = bankVolatAccountDetails.Surname,
            Address = bankVolatAccountDetails.Address,
            Email = bankVolatAccountDetails.Email,
            PhoneNumber = bankVolatAccountDetails.PhoneNumber,
        };

        var accountEntity = new Account(Provider.Bankvolat, account, bankVolatAccount.TotalAmount, bankVolatAccount.Currency, _limitUsd, customerEntity);

        await AddBankVolatTransactionsAsync(account, accountEntity, bankVolatAccount.Transactions);

        return accountEntity;
    }

    private async Task AddBankVolatTransactionsAsync(string account, Account accountEntity, List<string> transactionIds)
    {
        foreach (var transactionId in transactionIds) //Parallel foreach could be used here if needed
        {
            var transactionDetails = await bankvolatHttpClient.GetTransactionByIdAsync(account, transactionId);
            if (transactionDetails is null)
            {
                throw new KeyNotFoundException($"Transaction not found for account {account} with ID {transactionId}");
            }

            var transactionEntity = new Transaction
            {
                Id = transactionDetails.Id,
                SourceAccount = account,
                DestinationAccount = transactionDetails.Target,
                Amount = transactionDetails.Money,
                Currency = transactionDetails.Currency,
            };

            transactionEntity.SetTransactionStatus(transactionDetails.Status);
            transactionEntity.SetType(account, transactionDetails.Target);
            accountEntity.AddTransaction(transactionEntity, currencyConverter);
        }
    }
}
