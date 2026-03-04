# DEMO-18 EXACT ISSUE DETAILS

## Issue Information

| Field | Value |
|-------|-------|
| **Issue Key** | DEMO-18 |
| **Project** | DEMO |
| **Summary** | [Russian: Реализация эндпоинта поиска книг по названию] |
| **Description** | Link to Confluence page with implementation details |
| **Status** | In Progress |
| **Priority** | Medium |
| **Type** | Task |
| **Reporter** | Marat |
| **Created** | 2026-02-27T02:48:40.584-08:00 |
| **Updated** | 2026-02-27T04:12:51.746-08:00 |

## Summary (Exact Text)
**Original (Russian)**: Реализация эндпоинта поиска книг по названию
**Translation**: Implementation of endpoint for searching books by title

## Description (Exact Content)
```
[Link to Confluence page with implementation details]

api/ebooks/search?name= [Book Title or Part of Title]

Documentation:
[http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name]
```

## Linked Confluence Pages

### Page 4030468: Main Endpoint Documentation
**URL**: http://localhost:8090/spaces/DEMO/pages/4030468/GET+api+ebooks+search+name

**Content**:
- **Purpose**: Searches the external e-book OData catalog by book name
- **Endpoint**: `GET /api/ebooks/search?name={name}`
- **Parameters**: 
  - `name` (query, required): Book title or part of the title
- **Example Request**: `GET /api/ebooks/search?name=Hobbit`
- **Example Response**: 
  ```json
  HTTP 200 [
    {"id":2,"title":"The Hobbit"}
  ]
  ```
- **Responses**:
  - 200 OK - Success
  - 400 Bad Request - Missing `name` parameter
  - 502 Bad Gateway - External e-book service unavailable

### Page 4030465: Algorithm Implementation
**URL**: http://localhost:8090/spaces/DEMO/pages/4030465/%D0%90%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC+%D0%BF%D0%BE%D0%B8%D1%81%D0%BA%D0%B0+%D0%AD%D0%BB%D0%B5%D0%BA%D1%82%D1%80%D0%BE%D0%BD%D0%BD%D1%8B%D1%85+%D0%9A%D0%BD%D0%B8%D0%B3+%D0%BF%D0%BE+%D0%98%D0%BC%D0%B5%D0%BD%D0%B8

**Content** (Russian, translated):
- **Purpose**: `EbookCatalogService` is used to query OData catalog and return `EbookSearchDto`
- **Algorithm Steps**:
  1. Get request parameters from controller
  2. Validate and sanitize parameters
  3. Build OData query and execute
  4. Map response to DTO and return
- **Validation Rules**:
  - Check for empty/null title
  - If title contains apostrophe (`'` or `’`), truncate at apostrophe position
  - For case-insensitive search, use OData `tolower()`
  - If query execution fails, throw `HttpRequestException`
- **OData Query Example**: `/Books?$filter=contains(tolower(Title),'hobbit')&$select=Id,Title`
- **EbookSearchDto Schema**:
  - `Id` (int) - Unique identifier
  - `Title` (string) - Book title

## Detailed Requirements

### Functional Requirements
- **Endpoint**: `GET /api/ebooks/search?name={name}`
- **Parameter**: `name` (query, required) - Book title or part of the title
- **Response**: `EbookSearchDto` with `Id` and `Title` fields
- **Integration**: External e-book OData catalog service
- **Error Handling**: 
  - 400 Bad Request when `name` parameter is missing
  - 502 Bad Gateway when external service is unavailable

### Implementation Requirements
- Use `EbookCatalogService` for OData integration
- Case-insensitive search via OData `tolower()` function
- Apostrophe handling: truncate at apostrophe position
- Proper error handling with `HttpRequestException`
- Logging for query execution monitoring

## Notes
- The Jira issue summary is in Russian (Cyrillic)
- The description links to Confluence pages for detailed requirements
- The algorithm page (4030465) contains implementation details
- The main endpoint page (4030468) contains API documentation
- Implementation uses more fields than documented (Author, Genre, Price, PublishYear, Language)

## Conclusion
The DEMO-18 issue requires implementing a REST API endpoint to search the external e-book OData catalog by book name, following the algorithm and requirements documented in the linked Confluence pages.