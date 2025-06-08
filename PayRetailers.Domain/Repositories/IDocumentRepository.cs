using PayRetailers.Domain.Entities;

namespace PayRetailers.Domain.Repositories;
public interface IDocumentRepository
{
    Task<IEnumerable<Document>> GetByAccountAsync(string account);
    Task<Document?> GetByIdAsync(Guid id);
    Task<Guid> AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Document document);
    Task SaveChangesAsync();
}
