Added status endpoint at `GET /api/status` that returns `GetStatusResponseDto` with `IsActive` field.

**Files created:**
- [`GetStatusResponseDto.cs`](app/src/LibraryService.Application/Status/GetStatusResponseDto.cs) - DTO record with `IsActive` property
- [`StatusController.cs`](app/src/LibraryService.Api/Controllers/StatusController.cs) - Controller with GET endpoint returning `IsActive: true`

**Endpoint response:**
```json
{
  "isActive": true
}
```

Build succeeded and all 27 tests passed.

### CASE RESULTS
- Partially.
- Comments: Agent endpoint, but didn't add tests and didn't add docs.

