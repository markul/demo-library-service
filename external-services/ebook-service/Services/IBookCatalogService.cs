using EbookService.Models;

namespace EbookService.Services;

public interface IBookCatalogService
{
    IQueryable<Book> GetBooks();

    Book? GetBookById(int id);
}
