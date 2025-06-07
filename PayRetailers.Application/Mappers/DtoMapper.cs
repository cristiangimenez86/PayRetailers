using PayRetailers.Application.DTOs;
using PayRetailers.Domain.Entities;

namespace PayRetailers.Application.Mappers;

public static class DtoMapper
{
    public static AccountDto ToAccountDto(Account account)
    {
        return new AccountDto
        {
            CustomerAccount = account.CustomerAccount,
            Name = account.CustomerDetails.Name,
            Surname = account.CustomerDetails.Surname,
            Provider = account.Provider,
            CustomerDetails = new CustomerDetailsDto
            {
                Address = account.CustomerDetails.Address,
                PhoneNumber = account.CustomerDetails.PhoneNumber,
                Email = account.CustomerDetails.Email
            },
            Transactions = account.Transactions
                .Select(ToTransactionDto)
                .ToList()
        };
    }

    private static TransactionDto ToTransactionDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            CustomerSourceAccount = transaction.SourceAccount,
            CustomerDestinationAccount = transaction.DestinationAccount,
            Amount = transaction.Amount,
            Currency = transaction.Currency,
            Type = transaction.Type,
            Status = transaction.Status
        };
    }

    public static LimitCheckDto ToLimitCheckDto(Account account, decimal limit)
    {
        return new LimitCheckDto
        {
            CurrentLimit = limit,
            IsExceeded = account.IsLimitExceeded,
            Difference = Math.Abs(account.LimitDifference),
            Currency = "USD"
        };
    }
}