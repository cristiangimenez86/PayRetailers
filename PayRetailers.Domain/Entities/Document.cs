using PayRetailers.Domain.Enums;

namespace PayRetailers.Domain.Entities;

public class Document
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Account { get; init; }
    public required DocumentType Type { get; init; }
    public DateTimeOffset LastChange { get; private set; }
    public DocumentStatus Status { get; private set; }

    public DocumentStatus SetStatus(DocumentStatus newStatus)
    {
        Status = newStatus;
        LastChange = DateTimeOffset.UtcNow;
        return Status;
    }
}