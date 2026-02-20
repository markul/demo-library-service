namespace LibraryService.Application.Books;

public record BookDto(Guid Id, string Title, string Author, int PublishedYear, string Isbn);

public record CreateBookRequest(string Title, string Author, int PublishedYear, string Isbn);

public record UpdateBookRequest(string Title, string Author, int PublishedYear, string Isbn);
