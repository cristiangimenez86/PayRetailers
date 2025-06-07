using PayRetailers.Domain.Entities;

namespace PayRetailers.Domain.Repositories;
public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetByAccountAsync(string account);
    Task<Document?> GetAsync(Guid id);
    Task<Guid> AddAsync(Document document);
    Task SaveAsync();
}
