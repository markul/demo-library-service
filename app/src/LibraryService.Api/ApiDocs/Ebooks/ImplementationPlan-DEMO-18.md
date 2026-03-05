# Implementation Plan: DEMO-18

## Jira Issue Summary

| Field | Value |
|-------|-------|
| **Key** | DEMO-18 |
| **Summary** | Реализовать поиск книг по названию (Implement book search by name) |
| **Type** | Задача (Task) |
| **Status** | В работе (In Progress) |
| **Priority** | Medium |
| **Assignee** | Marat |
| **Reporter** | Marat |
| **Created** | 2026-02-27 |
| **Updated** | 2026-03-05 |

## Requirement

Implement endpoint: `GET /api/ebooks/search?name={name}`

Searches the external e-book OData catalog by book name and returns matching items with their Id and Title.

## Implementation Status: ✅ COMPLETED

The feature has been fully implemented. Below is the summary of what was done:

### 1. Application Layer ✅

| Component | File | Status |
|-----------|------|--------|
| Service Interface | [`IEbookCatalogService.cs`](../../../LibraryService.Application/Abstractions/Services/IEbookCatalogService.cs) | ✅ Implemented |
| DTO | [`EbookCatalogItemDto.cs`](../../../LibraryService.Application/Ebooks/EbookCatalogItemDto.cs) | ✅ Implemented |
| Query (all books) | [`GetEbookCatalogQuery.cs`](../../../LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs) | ✅ Implemented |
| Query (by name) | [`GetEbookCatalogByNameQuery`](../../../LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs:7) | ✅ Implemented |
| Query Handler | [`GetEbookCatalogByNameQueryHandler`](../../../LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs:20) | ✅ Implemented |

### 2. Infrastructure Layer ✅

| Component | File | Status |
|-----------|------|--------|
| Service Implementation | [`EbookCatalogService.cs`](../../../LibraryService.Infrastructure/Services/EbookCatalogService.cs) | ✅ Implemented |
| OData Connected Service | [`Connected Services/EbookOData/`](../../../LibraryService.Infrastructure/Connected Services/EbookOData/) | ✅ Configured |
| DI Registration | [`ServiceCollectionExtensions.cs`](../../../LibraryService.Infrastructure/ServiceCollectionExtensions.cs) | ✅ Registered |

### 3. API Layer ✅

| Component | File | Status |
|-----------|------|--------|
| Controller | [`EbooksController.cs`](../../../LibraryService.Api/Controllers/EbooksController.cs) | ✅ Implemented |
| Endpoint | `GET /api/ebooks/search?name={name}` | ✅ Implemented |

### 4. API Documentation ✅

| Document | File | Status |
|----------|------|--------|
| Endpoint Documentation | [`GetEbooksByName.md`](./GetEbooksByName.md) | ✅ Created |
| Algorithm Diagram | [`Diagrams/GetEbooksByName/Algorithm.svg`](./Diagrams/GetEbooksByName/Algorithm.svg) | ✅ Created |
| Input Example | [`Examples/GetEbooksByName/Input.md`](./Examples/GetEbooksByName/Input.md) | ✅ Created |
| Output Example | [`Examples/GetEbooksByName/Output.md`](./Examples/GetEbooksByName/Output.md) | ✅ Created |
| HTTP File | [`LibraryService.Api.http`](../../../LibraryService.Api/LibraryService.Api.http) | ✅ Updated |

### 5. Tests ✅

| Test Type | File | Status |
|-----------|------|--------|
| Unit Tests | [`EbookCatalogServiceTests.cs`](../../../../test/LibraryService.Tests.Unit/Infrastructure/EbookCatalogServiceTests.cs) | ✅ Implemented |
| Integration Tests | [`LibraryControllersIntegrationTests.cs`](../../../../test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs) | ✅ Implemented |

## Test Coverage

### Unit Tests (EbookCatalogServiceTests.cs)
- `GetBooksAsync_ShouldReturnMappedBooks_WhenODataBooksAreReturned` ✅
- `GetBooksAsync_ShouldCallBooksEntitySet` ✅
- `FindBooksByNameAsync_ShouldApplyTitleFilter_WhenNameIsProvided` ✅
- `FindBooksByNameAsync_ShouldThrowArgumentException_WhenNameIsEmpty` ✅
- `FindBooksByNameAsync_ShouldRemoveApostropheAndTrailingSubstring_WhenNameContainsApostrophe` ✅

### Integration Tests (LibraryControllersIntegrationTests.cs)
- `GetEbooks_ShouldReturnAllEbookCatalogItems` ✅
- `SearchEbooksByName_ShouldReturnMatchingItems` ✅
- `SearchEbooksByName_ShouldReturnBadRequest_WhenNameIsMissing` ✅

## API Specification

### Endpoint
```
GET /api/ebooks/search?name={name}
```

### Parameters
| Name | Type | Required | Description |
|------|------|----------|-------------|
| `name` | query | Yes | Book title or part of the title to search for |

### Responses

| Status Code | Description |
|-------------|-------------|
| 200 OK | Success - returns array of matching ebooks |
| 400 Bad Request | Missing `name` query parameter |
| 502 Bad Gateway | External e-book service unavailable or invalid payload |

### Example Request
```http
GET /api/ebooks/search?name=Hobbit
```

### Example Response
```json
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "id": 2,
    "title": "The Hobbit",
    "author": "J.R.R. Tolkien",
    "genre": "Fantasy",
    "price": 12.99,
    "publishYear": 1937,
    "language": "English"
  }
]
```

## Architecture

The implementation follows Clean Architecture principles:

```
┌─────────────────────────────────────────────────────────────────┐
│                        API Layer                                │
│  EbooksController.SearchByName()                               │
│  - Validates query parameter                                    │
│  - Handles HTTP errors (400, 502)                              │
│  - Returns ActionResult<IReadOnlyCollection<EbookCatalogItemDto>>│
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Application Layer                            │
│  GetEbookCatalogByNameQuery / GetEbookCatalogByNameQueryHandler│
│  - MediatR CQRS pattern                                         │
│  - Delegates to IEbookCatalogService.FindBooksByNameAsync()    │
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                          │
│  EbookCatalogService : IEbookCatalogService                    │
│  - Builds OData query with $filter contains(tolower(Title),...)│
│  - Executes query against external OData service                │
│  - Maps results to EbookCatalogItemDto                         │
│  - Handles DataServiceQueryException, DataServiceClientException│
└─────────────────────────────────────────────────────────────────┘
```

## Conclusion

**Status: ✅ IMPLEMENTATION COMPLETE**

All components for the feature `GET /api/ebooks/search?name=` have been implemented:
- Application layer with CQRS queries and handlers
- Infrastructure layer with OData integration
- API controller with proper error handling
- Complete API documentation with diagrams and examples
- Unit and integration tests

The feature is ready for verification and can be marked as complete in Jira.
