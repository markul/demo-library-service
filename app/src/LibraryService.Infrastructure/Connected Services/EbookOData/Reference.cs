#nullable disable
using System;
using Microsoft.OData.Client;

namespace LibraryService.Infrastructure.ConnectedServices.EbookOData;

public partial class Book
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Genre { get; set; }

    public decimal Price { get; set; }

    public int PublishYear { get; set; }

    public string Language { get; set; }
}

public partial class EbookContainer : DataServiceContext
{
    public EbookContainer(Uri serviceRoot)
        : base(serviceRoot, ODataProtocolVersion.V4)
    {
        Format.UseJson();
    }

    public DataServiceQuery<Book> Books => CreateQuery<Book>("Books");
}


