using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Entities;
public class CacheAccountProvider
{
    public required string Account { get; set; }
    public required Provider Provider { get; set; }
}
