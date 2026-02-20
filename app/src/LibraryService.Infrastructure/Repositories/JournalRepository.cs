using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class JournalRepository : IJournalRepository
{
    private readonly LibraryDbContext _dbContext;

    public JournalRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Journal>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Journals
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
    }

    public Task<Journal?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Journals
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Journal> AddAsync(Journal entity, CancellationToken cancellationToken)
    {
        _dbContext.Journals.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(Journal entity, CancellationToken cancellationToken)
    {
        _dbContext.Journals.Update(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Journals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.Journals.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
