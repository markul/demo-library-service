using EbookService.Models;
using EbookService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace EbookService.Controllers;

public class BooksController(IBookCatalogService catalogService) : ODataController
{
    [EnableQuery]
    public ActionResult<IQueryable<Book>> Get()
    {
        return Ok(catalogService.GetBooks());
    }

    [EnableQuery]
    public ActionResult<Book> Get([FromODataUri] int key)
    {
        var book = catalogService.GetBookById(key);
        if (book is null)
        {
            return NotFound();
        }

        return Ok(book);
    }
}
