using PayRetailers.Application.Contracts;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Services;

namespace PayRetailers.Application.Services;
public class PayBroBuilder(IPayBroHttpClient payBroHttpClient, ICurrencyConverter currencyConverter) : IPayBroBuilder
{
    public async Task<Account> BuildPayBroAccountAsync(string account)
    {
        var payBroAccounts = await payBroHttpClient.GetAccountsAsync(); //TODO: Get all accounts is not a good idea.
        var payBroAccount = payBroAccounts.FirstOrDefault(a => a.Account.Equals(account));
        if (payBroAccount is null)
        {
            throw new KeyNotFoundException($"PayBro account not found: {account}");
        }

        var customerEntity = new Customer
        {
            Name = payBroAccount.PersonalDetails.Name,
            Surname = payBroAccount.PersonalDetails.Surname,
            Address = payBroAccount.PersonalDetails.Address,
            Email = payBroAccount.PersonalDetails.Email,
            PhoneNumber = payBroAccount.PersonalDetails.PhoneNumber,
        };

        var accountEntity = new Account(Provider.PayBro, account, payBroAccount.TotalAmount, payBroAccount.Currency, customerEntity);

        await AddPayBroTransactionsAsync(account, accountEntity);
        return accountEntity;
    }

    private async Task AddPayBroTransactionsAsync(string account, Account accountEntity)
    {
        var payBroTransactions = await payBroHttpClient.GetTransactionsByAccountAsync(account);
        foreach (var payBroTransaction in payBroTransactions)
        {
            var transactionEntity = new Transaction
            {
                //Id = //Warning: PayBro transactions do not have an ID.
                SourceAccount = account,
                DestinationAccount = payBroTransaction.DestinationAccount,
                Amount = payBroTransaction.Amount,
                Currency = payBroTransaction.Currency,
            };

            transactionEntity.SetTransactionStatus(payBroTransaction.Status);
            transactionEntity.SetType(account, payBroTransaction.DestinationAccount);
            accountEntity.AddTransaction(transactionEntity, currencyConverter);
        }
    }
}
