using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IJournalRepository
{
    Task<IReadOnlyCollection<Journal>> GetAllAsync(CancellationToken cancellationToken);

    Task<Journal?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Journal> AddAsync(Journal entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Journal entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
