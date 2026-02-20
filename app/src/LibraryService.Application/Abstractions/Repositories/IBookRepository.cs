using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface IBookRepository
{
    Task<IReadOnlyCollection<Book>> GetAllAsync(CancellationToken cancellationToken);

    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Book> AddAsync(Book entity, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(Book entity, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
