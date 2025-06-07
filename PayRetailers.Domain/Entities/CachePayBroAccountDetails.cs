namespace PayRetailers.Domain.Entities;
public class CachePayBroAccountDetails //To be stored in Cache
{
    public required string Account { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerSurname { get; set; }
    public required string CustomerAddress { get; set; }
    public required string CustomerPhoneNumber { get; set; }
    public required string CustomerEmail { get; set; }
}
