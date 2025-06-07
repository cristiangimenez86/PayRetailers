namespace PayRetailers.Application.DTOs;
public class LimitCheckDto
{
    public required decimal CurrentLimit { get; set; }
    public required bool IsExceeded { get; set; }
    public required decimal Difference { get; set; }
    public required string Currency { get; set; }
}
