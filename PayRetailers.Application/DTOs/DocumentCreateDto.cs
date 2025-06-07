using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.DTOs;
public class DocumentCreateDto
{
    public required DocumentType DocumentType { get; set; }
}
