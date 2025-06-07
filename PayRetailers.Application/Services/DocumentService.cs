using PayRetailers.Application.Contracts;
using PayRetailers.Application.DTOs;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Enums;
using PayRetailers.Domain.Repositories;

namespace PayRetailers.Application.Services;

public class DocumentService(IDocumentRepository documentRepository) : IDocumentService
{
    public async Task<IEnumerable<DocumentDto>> GetByAccountAsync(string account)
    {
        var documents = await documentRepository.GetByAccountAsync(account);

        return documents.Select(d => new DocumentDto
        {
            DocumentId = d.Id,
            DocumentType = d.Type,
            LastChange = d.LastChange,
            Status = d.Status
        });
    }

    public async Task<Guid> CreateAsync(string account, DocumentCreateDto dto)
    {
        var document = new Document
        {
            Account = account,
            Type = dto.DocumentType
        };

        document.SetStatus(DocumentStatus.New);

        return await documentRepository.AddAsync(document);
    }

    public async Task UpdateStatusAsync(string account, Guid documentId, DocumentStatus newStatus)
    {
        var doc = await documentRepository.GetAsync(documentId)
                  ?? throw new KeyNotFoundException("Document not found");

        if (doc.Account != account)
        {
            throw new Exception("Document does not belong to this account");
        }

        doc.SetStatus(newStatus);

        await documentRepository.SaveAsync();
    }
}
