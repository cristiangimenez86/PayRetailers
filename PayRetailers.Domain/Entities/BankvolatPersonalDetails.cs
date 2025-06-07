using System.Text.Json.Serialization;

namespace PayRetailers.Domain.Entities;
public class BankvolatPersonalDetails
{
    [JsonPropertyName("user_name")]
    public required string Name { get; set; }

    [JsonPropertyName("user_surname")]
    public required string Surname { get; set; }
    
    [JsonPropertyName("user_email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("user_phoneNumber")]
    public required string PhoneNumber { get; set; }
    
    [JsonPropertyName("user_address")]
    public required string Address { get; set; }
    
    [JsonPropertyName("user_country")]
    public string? Country { get; set; }
}