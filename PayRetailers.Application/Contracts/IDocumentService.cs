using PayRetailers.Application.DTOs;
using PayRetailers.Domain.Enums;

namespace PayRetailers.Application.Contracts;
public interface IDocumentService
{
    Task<IEnumerable<DocumentDto>> GetByAccountAsync(string account);
    Task<Guid> CreateAsync(string account, DocumentCreateDto dto);
    Task UpdateStatusAsync(string account, Guid documentId, DocumentStatus newStatus);
    Task DeleteAsync(string account, Guid documentId);
}