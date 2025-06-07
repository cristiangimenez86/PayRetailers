using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.DTOs;
public class DocumentUpdateDto
{
    public required DocumentStatus Status { get; set; }
}
