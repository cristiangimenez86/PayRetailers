using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.DTOs;
public class DocumentDto
{
    public required Guid DocumentId { get; set; }
    public required DocumentType DocumentType { get; set; }
    public required DateTimeOffset LastChange { get; set; }
    public required DocumentStatus Status { get; set; }
}
