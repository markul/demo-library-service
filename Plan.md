# Implementation Plan for JIRA DEMO-18

## Issue Summary
**Issue:** DEMO-18 - Доработать поиск эл. книг по имени (Improve ebook search by name)

**Current Behavior:** The `/api/ebooks/search?name={name}` endpoint returns full `EbookCatalogItemDto` with all fields (Id, Title, Author, Genre, Price, PublishYear, Language).

**Required Change:** The endpoint should return only `Id` and `Title` fields.

---

## Implementation Steps

### 1. Create New Search Result DTO
**File:** `app/src/LibraryService.Application/Ebooks/EbookSearchResultDto.cs` (new)

Create a minimal DTO for search results:
```csharp
public sealed record EbookSearchResultDto(
    int Id,
    string Title);
```

### 2. Update EbookCatalogService
**File:** `app/src/LibraryService.Infrastructure/Services/EbookCatalogService.cs`

Modify `FindBooksByNameAsync` to return `IReadOnlyCollection<EbookSearchResultDto>` instead of `IReadOnlyCollection<EbookCatalogItemDto>`.

Update the `Map` method or create a new `MapSearchResult` method to return only Id and Title.

### 3. Update Query Handler
**File:** `app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs`

Update `GetEbookCatalogByNameQuery` to return `IReadOnlyCollection<EbookSearchResultDto>`.

### 4. Update Controller
**File:** `app/src/LibraryService.Api/Controllers/EbooksController.cs`

Update `SearchByName` action to return `IReadOnlyCollection<EbookSearchResultDto>`.

### 5. Update Integration Tests
**File:** `app/test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs`

Update `SearchEbooksByName_ShouldReturnMatchingItems` test to expect `EbookSearchResultDto` with only Id and Title.

### 6. Update API Documentation
Update Confluence page at `http://localhost:8090/spaces/DEMO/pages/4030468/api+ebooks+search+name+name+-+%D0%BF%D0%BE%D0%B8%D1%81%D0%BA%D0%B0+%D1%8D%D0%BB%D0%B5%D0%BA%D1%82%D1%80%D0%BE%D0%BD%D0%BD%D1%8B%D1%85+%D0%BA%D0%BD%D0%B8%D0%B3` to reflect the new response format with only Id and Title fields.

### 7. Verify
- Run `dotnet build app/LibraryService.sln`
- Run `dotnet test app/LibraryService.sln`
- Ensure all tests pass

---

## Files to Modify

| Action | File |
|--------|------|
| Create | `app/src/LibraryService.Application/Ebooks/EbookSearchResultDto.cs` |
| Modify | `app/src/LibraryService.Infrastructure/Services/EbookCatalogService.cs` |
| Modify | `app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs` |
| Modify | `app/src/LibraryService.Api/Controllers/EbooksController.cs` |
| Modify | `app/test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs` |