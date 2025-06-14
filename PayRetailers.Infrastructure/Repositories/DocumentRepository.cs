﻿using Microsoft.EntityFrameworkCore;
using PayRetailers.Domain.Entities;
using PayRetailers.Domain.Repositories;
using PayRetailers.Infrastructure.DbContexts;

namespace PayRetailers.Infrastructure.Repositories;
public class DocumentRepository(ApplicationDbContext context) : IDocumentRepository
{
    public async Task<IEnumerable<Document>> GetByAccountAsync(string account)
    {
        return await context.Documents
            .Where(x=>x.Account.Equals(account))
            .ToListAsync();
    }

    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await context.Documents.FindAsync(id);
    }

    public async Task<Guid> AddAsync(Document document)
    {
        await context.Documents.AddAsync(document);
        await context.SaveChangesAsync();
        return document.Id;
    }

    public async Task UpdateAsync(Document document)
    {
        context.Documents.Update(document);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Document document)
    {
        context.Documents.Remove(document);
        await context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
