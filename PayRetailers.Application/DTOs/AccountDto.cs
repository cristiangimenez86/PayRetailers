using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.DTOs;
public class AccountDto
{
    public required string CustomerAccount { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required Provider Provider { get; set; }
    public required CustomerDetailsDto CustomerDetails { get; set; }
    public required List<TransactionDto> Transactions { get; set; } = [];
}
