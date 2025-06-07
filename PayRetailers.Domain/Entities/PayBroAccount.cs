namespace PayRetailers.Domain.Entities;
public class PayBroAccount
{
    public required string Account { get; set; }
    public required decimal TotalAmount { get; set; }
    public required string Currency { get; set; }
    public required PayBroPersonalDetails PersonalDetails { get; set; }
}
