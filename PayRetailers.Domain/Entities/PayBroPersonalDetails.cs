namespace PayRetailers.Domain.Entities;
public class PayBroPersonalDetails
{
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Address { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
}
