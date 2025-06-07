namespace PayRetailers.Application.DTOs;
public class BalanceDto
{
    public required string CustomerAccount { get; set; }
    public required int Balance { get; set; }
    public required string Currency { get; set; }
}
