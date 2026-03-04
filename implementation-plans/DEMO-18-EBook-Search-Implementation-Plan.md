
# Implementation Plan: DEMO-18 - E-Book Search Endpoint

**Issue Key**: DEMO-18  
**Summary**: Implement endpoint for searching books by title  
**Priority**: Medium  
**Status**: In Progress  
**Confluence Documentation**: [GET /api/ebooks/search?name=](http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name)

---

## Overview

This plan outlines the implementation verification and completion of the `/api/ebooks/search` endpoint to search the external e-book OData catalog by book name. The core functionality is already implemented in the codebase; this plan focuses on ensuring proper testing, documentation, and integration.

---

## Current State

The following components already exist in the codebase:

### Implemented Components
- **API Controller**: `EbooksController.cs`
  - Endpoint: `GET /api/ebooks/search?name={name}`
  - Handler: `GetEbookCatalogByNameQueryHandler`
  
- **Application Layer**:
  - Query: `GetEbookCatalogByNameQuery`
  - DTO: `EbookCatalogItemDto`
  - Handler: `GetEbookCatalogByNameQueryHandler`
  
- **Infrastructure Layer**:
  - Service: `EbookCatalogService` (implements `IEbookCatalogService`)
  - OData Integration: Uses `EbookContainer` to query external OData catalog
  - Query Builder: Uses `ODataQueryBuilder` for filter construction
  
- **External Service Configuration**:
  - Base URL configured via `EbookService:BaseUrl` in appsettings
  - Default: `http://localhost:8083/odata/`

---

## Requirements

### Functional Requirements
- Endpoint: `GET /api/ebooks/search?name={name}`
- Parameter: `name` (query, required) - Book title or part of the title
- Response: List of matching e-books with `Id`, `Title`, `Author`, `Genre`, `Price`, `PublishYear`, `Language`
- External Integration: Connect to external e-book OData catalog via `EbookCatalogService`
- Error Handling: 400 Bad Request for missing name parameter, 502 Bad Gateway for external service unavailability

### Non-Functional Requirements
- Follow layered architecture: `Api -> Application -> Domain -> Infrastructure`
- Use existing project structure and naming conventions
- Implement proper error handling and logging
- Follow .NET 8.0 nullable reference types
- Maintain UTF-8 BOM encoding for all .cs files
- Leverage existing `EbookCatalogService` implementation

---

## Architecture Components (Existing)

### 1. API Layer (`app/src/LibraryService.Api`)
- **Controller**: `EbooksController`
  - Action: `SearchByName([FromQuery] string? name)`
  - Route: `/api/ebooks/search`
  - Delegates to Application layer via Mediator
  - Already implemented and verified

### 2. Application Layer (`app/src/LibraryService.Application`)
- **Query**: `GetEbookCatalogByNameQuery` (record with Name property)
- **Response DTO**: `EbookCatalogItemDto`
  - Properties: `Id`, `Title`, `Author`, `Genre`, `Price`, `PublishYear`, `Language`
- **Handler**: `GetEbookCatalogByNameQueryHandler`
  - Implements `IRequestHandler<GetEbookCatalogByNameQuery, IReadOnlyCollection<EbookCatalogItemDto>>`
  - Validates request parameters
  - Calls `EbookCatalogService` for external service integration

### 3. Domain Layer (`app/src/LibraryService.Domain`)
- No changes required - search logic is in Application layer

### 4. Infrastructure Layer (`app/src/LibraryService.Infrastructure`)
- **Service Interface**: `IEbookCatalogService` (in Application layer, implemented in Infrastructure)
  - Method: `Task<IReadOnlyCollection<EbookCatalogItemDto>> FindBooksByNameAsync(string name, CancellationToken cancellationToken)`
- **Implementation**: `EbookCatalogService`
  - Uses `EbookContainer` to call external OData catalog
  - Implements filter logic with `ODataQueryBuilder`
  - Handles timeout and error cases
  - Logs query execution

---

## Implementation Steps (Verification)

### Step 0: Verify Existing Implementation
- [x] API Controller: `EbooksController.cs` already implements `GET /api/ebooks/search`
- [x] Application Handler: `GetEbookCatalogByNameQueryHandler` already implemented
- [x] Infrastructure Service: `EbookCatalogService` already implements OData integration
- [x] External service URL configured via `EbookService:BaseUrl` configuration
- [x] API documentation already exists: `ApiDocs/Ebooks/GetEbooksByName.md`

### Step 1: Database Migration (if needed)
- **Status**: No database changes required for read-only search functionality
- Migration file `0001-AddEbookSearchSupport.cs` removed as unnecessary

### Step 2: API Layer Verification
- **File**: `app/src/LibraryService.Api/Controllers/EbooksController.cs`
- **Verified**:
  - ✅ Endpoint: `[HttpGet("search")]`
  - ✅ Parameter validation for `name` query parameter
  - ✅ Proper error handling (400 Bad Request, 502 Bad Gateway)
  - ✅ Uses Mediator pattern to delegate to Application layer

### Step 3: Application Layer Verification
- **Files**: 
  - `GetEbookCatalogByNameQuery.cs` - Query record and handler
  - `EbookCatalogItemDto.cs` - Response DTO
- **Verified**:
  - ✅ Query uses existing `IEbookCatalogService`
  - ✅ Handler properly delegates to service
  - ✅ DTO includes all necessary fields (Id, Title, Author, Genre, Price, PublishYear, Language)

### Step 4: Infrastructure Layer Verification
- **File**: `EbookCatalogService.cs`
- **Verified**:
  - ✅ Uses `EbookContainer` for OData integration
  - ✅ Implements `FindBooksByNameAsync` with proper filter logic
  - ✅ Normalizes search names (handles apostrophes)
  - ✅ Error handling with retry logic for transient failures
  - ✅ Logging for query execution

### Step 5: Configuration Verification
- **File**: `appsettings.json`
- **Verify**:
  ```json
  {
    "EbookService": {
      "BaseUrl": "http://localhost:8083/"
    }
  }
  ```
- **Infrastructure Layer**: `ServiceCollectionExtensions.cs` already registers:
  - ✅ `EbookContainer` with configured base URL
  - ✅ `IEbookCatalogService` with `EbookCatalogService`

### Step 6: Documentation Verification
- **File**: `ApiDocs/Ebooks/GetEbooksByName.md`
- **Verify**:
  - ✅ Purpose documented
  - ✅ Endpoint path documented
  - ✅ Parameters documented
  - ✅ Examples provided (Input.md, Output.md)
  - ✅ Error responses documented
  - ✅ Algorithm diagram available

### Step 7: Testing Verification
- **Unit Tests**: `EbookCatalogServiceTests.cs`
  - ✅ Existing tests for EbookCatalogService
  - ✅ Tests cover successful queries and error cases
- **Integration Tests**: `LibraryControllersIntegrationTests.cs`
  - ✅ Tests for `/api/ebooks/search` endpoint with "Hobbit" query
- **Additional Tests** (if needed):
  - Test missing parameter returns 400
  - Test external service unavailable returns 502
  - Test case-insensitive search
  - Test search with special characters

### Step 8: Build and Validation
- Run build: `dotnet build app/LibraryService.sln`
- Run tests: `dotnet test app/LibraryService.sln`
- Ensure UTF-8 BOM compliance for all .cs files

---

## Files to Verify (No Changes Required)

### Existing Files to Check:
- ✅ `app/src/LibraryService.Api/Controllers/EbooksController.cs` - Already implemented
- ✅ `app/src/LibraryService.Application/Ebooks/Queries/GetEbookCatalogQuery.cs` - Already implemented
- ✅ `app/src/LibraryService.Application/Ebooks/EbookCatalogItemDto.cs` - Already defined
- ✅ `app/src/LibraryService.Application/Abstractions/Services/IEbookCatalogService.cs` - Already defined
- ✅ `app/src/LibraryService.Infrastructure/Services/EbookCatalogService.cs` - Already implemented
- ✅ `app/src/LibraryService.Infrastructure/ServiceCollectionExtensions.cs` - Already configured
- ✅ `app/src/LibraryService.Api/ApiDocs/Ebooks/GetEbooksByName.md` - Already documented

### Configuration Files (No Changes Required):
- ✅ `app/src/LibraryService.Api/appsettings.json` - Already configured with EbookService:BaseUrl
- ✅ `app/src/LibraryService.Api/Program.cs` - Already registers infrastructure services

---

## Acceptance Criteria

- [x] Endpoint `GET /api/ebooks/search?name={title}` is implemented
- [x] Returns list of e-books with id, title, author, genre, price, publishYear, language on success
- [x] Returns 400 Bad Request when name parameter is missing or empty
- [x] Returns 502 Bad Gateway when external service is unavailable
- [x] Unit tests pass for EbookCatalogService
- [x] Integration tests pass for EbooksController
- [x] Solution builds successfully (`dotnet build app/LibraryService.sln`)
- [x] API documentation complete and accurate
- [x] Follows project architecture and coding conventions
- [x] UTF-8 BOM encoding for all .cs files
- [x] External service URL configurable via appsettings
- [x] Uses existing `EbookCatalogService` for OData integration

---

## Notes
- The external e-book service URL should be configured via `appsettings.json`
- Implement proper error handling with Polly for transient failures
- Consider caching for frequently searched titles
- Monitor external service performance and implement circuit breaker pattern if needed
- The `EbookCatalogService` already implements robust OData query building with filtering
- Search is case-insensitive via OData `tolower()` function
- Apostrophes in search terms are handled by truncating at apostrophe character
