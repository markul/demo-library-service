# Ebook Service

ASP.NET Core `net8.0` Web API exposing an OData e-book catalog.

## Run

```bash
dotnet run --project external-services/ebook-service/EbookService.csproj
```

## OData endpoint

- `GET /odata/Books`
- `GET /odata/Books({id})`

Catalog is seeded in-memory with 100 books, 10 books for each of 10 non-IT genres.

## Example filters

- `GET /odata/Books?$filter=contains(Title,'The')`
- `GET /odata/Books?$filter=Price lt 40 and PublishYear ge 2010`
- `GET /odata/Books?$filter=Genre eq 'Fantasy'&$orderby=Price desc`
- `GET /odata/Books?$top=3&$skip=1&$count=true`
