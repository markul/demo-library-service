Renamed `FindBooksByNameAsync` to `FindBooksAsync` everywhere it was referenced.

Updated files:
- [IEbookCatalogService.cs](c:\Users\vagrant\dev\git-markul\demo-library-service\app\src\LibraryService.Application\Abstractions\Services\IEbookCatalogService.cs)
- [GetEbookCatalogQuery.cs](c:\Users\vagrant\dev\git-markul\demo-library-service\app\src\LibraryService.Application\Ebooks\Queries\GetEbookCatalogQuery.cs)
- [EbookCatalogService.cs](c:\Users\vagrant\dev\git-markul\demo-library-service\app\src\LibraryService.Infrastructure\Services\EbookCatalogService.cs)
- [LibraryApiFactory.cs](c:\Users\vagrant\dev\git-markul\demo-library-service\app\test\LibraryService.Tests.Integration\Infrastructure\LibraryApiFactory.cs)
- [EbookCatalogServiceTests.cs](c:\Users\vagrant\dev\git-markul\demo-library-service\app\test\LibraryService.Tests.Unit\Infrastructure\EbookCatalogServiceTests.cs)

Validation:
- Ran `dotnet build app/LibraryService.sln` successfully with 0 errors.
- Existing NuGet vulnerability warnings (`NU1903`) remain in test projects; no new compile issues from the rename.

  ### CASE RESULTS
- Passed.
- Comments: Renamed correctly and validated build success.