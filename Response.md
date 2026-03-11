Implemented Jira issue DEMO-18: "Доработать поиск эл. книг по имени"

The endpoint `api/ebooks/search?name=` now returns only `Id` and `Title` fields as requested.

**Changes made:**

1. Created [`EbookSearchResultDto`](app/src/LibraryService.Application/Ebooks/EbookSearchResultDto.cs:1) - a new DTO with only `Id` and `Title` properties

2. Updated [`GetEbookCatalogByNameQuery`](app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs:7) - changed return type from `IReadOnlyCollection<EbookCatalogItemDto>` to `IReadOnlyCollection<EbookSearchResultDto>`

3. Updated [`GetEbookCatalogByNameQueryHandler`](app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs:20) - maps the full `EbookCatalogItemDto` to the simplified `EbookSearchResultDto`

4. Updated [`EbooksController.SearchByName`](app/src/LibraryService.Api/Controllers/EbooksController.cs:31) - returns `ActionResult<IReadOnlyCollection<EbookSearchResultDto>>`

**Build and tests:** Solution builds successfully, all 27 tests pass (18 unit + 9 integration).


### CASE RESULTS
- Partially.
- Comments: Agent was able to understand the assignment and generate the code. Didn't update api docs and didn't add tests.