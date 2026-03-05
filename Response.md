Updated status endpoint at `GET /api/status` to return `IsActive: true` when there are active subscriptions.

**Files created/modified:**
- [`GetStatusResponseDto.cs`](app/src/LibraryService.Application/Status/GetStatusResponseDto.cs) - DTO record with `IsActive` property
- [`GetStatusQuery.cs`](app/src/LibraryService.Application/Status/Queries/GetStatusQuery.cs) - Query and handler that checks for active subscriptions
- [`StatusController.cs`](app/src/LibraryService.Api/Controllers/StatusController.cs) - Controller using MediatR to handle the query

**Logic:**
- Queries all subscriptions via `ISubscriptionRepository`
- Returns `IsActive: true` if any subscription has `IsActive: true`, otherwise `false`

**Endpoint response examples:**
```json
// With active subscriptions
{ "isActive": true }

// Without active subscriptions
{ "isActive": false }
```

Build succeeded and all 27 tests passed.

### CASE RESULTS
- Passed/Partially.
- Comments: Agent updated status endpoint, used Mediatr, but didn't add tests or update docs.

