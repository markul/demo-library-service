Plan

`DEMO-18` fetched from Jira.

Issue snapshot:
- Key: `DEMO-18`
- Summary: `Доработать поиск эл. книг по имени`
- Status: `В работе`
- Priority: `Medium`
- Created: `February 27, 2026 02:48:40 (-08:00)`
- Updated: `February 27, 2026 04:12:51 (-08:00)`
- Requirement: `GET /api/ebooks/search?name=` must return only `id` and `title`.

Spec check:
- Confluence page `4030468` (`GET /api/ebooks/search?name=`) confirms response shape is only `id` + `title`.

Implementation plan:
1. Add a dedicated search response DTO in Application layer (`Id`, `Title`) to avoid changing `/api/ebooks` full contract.
   - [EbookCatalogItemDto.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Ebooks/EbookCatalogItemDto.cs)
2. Update search query/handler contract to return the new DTO for `GetEbookCatalogByNameQuery`.
   - [GetEbookCatalogQuery.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs)
3. Extend ebook service abstraction with a title-only search method (or replace current name-search method if unused).
   - [IEbookCatalogService.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Abstractions/Services/IEbookCatalogService.cs)
4. Implement title-only OData projection (`Id`, `Title`) in Infrastructure for `/search` to reduce payload and align contract.
   - [EbookCatalogService.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Services/EbookCatalogService.cs)
5. Update controller search endpoint return type to the new DTO.
   - [EbooksController.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/Controllers/EbooksController.cs)
6. Update tests for endpoint contract and fake service implementation.
   - [LibraryControllersIntegrationTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs)
   - [LibraryApiFactory.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Integration/Infrastructure/LibraryApiFactory.cs)
   - [EbookCatalogServiceTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Infrastructure/EbookCatalogServiceTests.cs)
7. Update API docs/examples to match response shape and keep docs in sync.
   - [GetEbooksByName.md](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/ApiDocs/Ebooks/GetEbooksByName.md)
   - [Output.md](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/ApiDocs/Ebooks/Examples/GetEbooksByName/Output.md)
   - [LibraryService.Api.http](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/LibraryService.Api.http)

Validation:
1. `dotnet build app/LibraryService.sln`
2. `dotnet test app/LibraryService.sln`
3. Manual check: `GET /api/ebooks/search?name=Hobbit` returns only `[{"id":...,"title":"..."}]`

If you want, I can execute this plan now and make the code/doc/test changes end-to-end.


Implemented `DEMO-18` end-to-end, with step 3 done as a direct method replacement (not additive).

Changes made:
- Replaced `IEbookCatalogService.FindBooksByNameAsync` return type to title-only DTO: [IEbookCatalogService.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Abstractions/Services/IEbookCatalogService.cs)
- Added new DTO `EbookCatalogSearchItemDto` (`Id`, `Title`): [EbookCatalogSearchItemDto.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Ebooks/EbookCatalogSearchItemDto.cs)
- Updated query/handler contract for search endpoint: [GetEbookCatalogQuery.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs)
- Updated infrastructure search projection to request/map only `Id` and `Title`: [EbookCatalogService.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Services/EbookCatalogService.cs)
- Updated API action return type for `/api/ebooks/search`: [EbooksController.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/Controllers/EbooksController.cs)
- Updated integration fake service and tests for new contract (including assertion that extra fields are not returned): [LibraryApiFactory.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Integration/Infrastructure/LibraryApiFactory.cs), [LibraryControllersIntegrationTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs)
- Updated unit test to verify OData `$select=Id,Title`: [EbookCatalogServiceTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Infrastructure/EbookCatalogServiceTests.cs)
- Updated API docs/example output to `id` + `title` only: [GetEbooksByName.md](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/ApiDocs/Ebooks/GetEbooksByName.md), [Output.md](c:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/ApiDocs/Ebooks/Examples/GetEbooksByName/Output.md)

Validation:
- `dotnet build app/LibraryService.sln` passed.
- `dotnet test app/LibraryService.sln` passed.
- Existing NU1903 dependency vulnerability warnings are still present (pre-existing, not introduced by this change).


### CASE RESULTS
- Passed.
- Comments: Agent retrieved the issue from JIRA, checked linked confluence pages, created a solid implementation plan. Asked about prefered implementation of one of the steps. Agent then implemented the changes as per the plan. The changes were verified by running the unit tests and checking the API documentation. The changes were also validated by running the build and test commands. 