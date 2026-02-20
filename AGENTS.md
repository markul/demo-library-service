# Repository Guidelines

## Project Structure & Module Organization
- Solution: `app/LibraryService.sln`.
- Core libraries: `app/src/LibraryService.Domain`, `app/src/LibraryService.Application`, `app/src/LibraryService.Infrastructure`.
- Service host: `app/src/LibraryService.Api`.
- Infrastructure: `infrastructure/docker-compose.yml` (includes `app-db`, `jira`, `confluence` and related databases).
- External integrations placeholder: `external-services/`.
- Tests folder: `app/test` (currently no test projects committed).

## Build, Test, and Development Commands
- Restore: `dotnet restore app/LibraryService.sln`
- Build: `dotnet build app/LibraryService.sln`
- Run API (local): `dotnet run --project app/src/LibraryService.Api/LibraryService.Api.csproj`
- Start only app database: `docker compose -f infrastructure/docker-compose.yml up -d app-db`
- Start full local infrastructure: `docker compose -f infrastructure/docker-compose.yml up -d`
- Stop infrastructure: `docker compose -f infrastructure/docker-compose.yml down`
- Validate compose config: `docker compose -f infrastructure/docker-compose.yml config`

## Database Guidelines
- Primary app database is PostgreSQL service `app-db` from `infrastructure/docker-compose.yml`.
- Default app connection string points to docker service host `app-db` in `app/src/LibraryService.Api/appsettings.json`.
- Local development override points to `localhost` in `app/src/LibraryService.Api/appsettings.Development.json`.
- Current bootstrap uses `Database.Migrate()` in `app/src/LibraryService.Api/Program.cs`.
- Keep startup aligned with migration-based lifecycle.

## Database Migrations
- Store EF migrations under `app/src/LibraryService.Infrastructure/Database/Migrations`.
- Keep generated SQL scripts under `app/src/LibraryService.Infrastructure/Database/ManualScripts`.
- Do not write EF migration files manually; always generate using `dotnet ef`.
- Migration workflow:
1. Ensure `dotnet-ef` is available: `dotnet tool update --global dotnet-ef`
2. Create a migration:
   `dotnet ef migrations add <MigrationName> --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj --context LibraryDbContext --output-dir Database/Migrations`
3. Apply migration locally:
   `dotnet ef database update --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj --context LibraryDbContext`
4. Generate forward SQL script:
   `dotnet ef migrations script <PreviousMigration> <MigrationName> --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj --context LibraryDbContext -o app/src/LibraryService.Infrastructure/Database/ManualScripts/<NNN>-<MigrationName>.sql -i`
5. Generate rollback SQL script:
   `dotnet ef migrations script <MigrationName> <PreviousMigration> --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj --context LibraryDbContext -o app/src/LibraryService.Infrastructure/Database/ManualScripts/<NNN>-<MigrationName>_Revert.sql -i`
6. Keep application startup consistent with migrations by replacing `EnsureCreated()` with `Database.Migrate()` once migration-based lifecycle is adopted.

## Coding Style & Naming Conventions
- Target framework: `net8.0`, nullable enabled, implicit usings enabled.
- Keep layered boundaries: `Api -> Application -> Domain`, with Infrastructure implementing Application abstractions.
- Keep namespaces aligned with folder structure.
- Prefer clear entity naming (`Book`, `Journal`, `Client`) and explicit DTOs for API contracts.
- Use UTF-8 BOM encoding in .cs files 

## Testing Guidelines
- Add unit/integration tests under `app/test` when expanding behavior.
- If an endpoint is added, updated, or removed, tests must be added or updated in the same change set.
- Prefer `xUnit` for new test projects and keep test code under `app/test/<ProjectName>`.
- Suggested naming for tests: `{MethodName}_{ExpectedResult}_{Condition}`.
- Run all tests (when present): `dotnet test app/LibraryService.sln`.
- Run tests with detailed output when debugging failures: `dotnet test app/LibraryService.sln -v normal`.
- For targeted execution, run a specific test project: `dotnet test app/test/<TestProject>/<TestProject>.csproj`.
- Ensure `dotnet build app/LibraryService.sln` succeeds before submitting changes.

## API Guidelines
- REST endpoints live under `app/src/LibraryService.Api/Controllers`.
- Keep endpoint routes consistent under `/api/books`, `/api/journals`, `/api/clients`, `/api/subscriptions`, `/api/subscriptions/types`, `/api/payments`.
- Update `app/src/LibraryService.Api/LibraryService.Api.http` when adding or changing endpoints.
- If an endpoint is added, updated, or removed, update API docs and tests in the same change set.
- Keep Swagger enabled for development in `Program.cs`.

## API Docs Guidelines
- API docs live under `app/src/LibraryService.Api/ApiDocs/<Area>`.
- If an endpoint is added, updated, or removed, API docs update is mandatory in the same change set.
- For each endpoint, add:
  - `<MethodName>.md` with purpose, parameters, examples, and responses.
  - Diagrams in `Diagrams/<MethodName>/` as both `.puml` and `.svg`.
- Examples in `Examples/<MethodName>/Input.md` and `Examples/<MethodName>/Output.md`.
- In markdown, reference `.svg` files (not `.puml`).

## Commit & Pull Request Guidelines
- Use concise, present-tense commit messages (example: `feat(api): add client update endpoint`).
- Keep PRs focused and include:
  - What changed.
  - Why the change was needed.
  - Any required config or DB updates.
- Include sample request/response or endpoint notes when API behavior changes.

## Security & Configuration Tips
- Do not commit secrets; use environment variables for sensitive values.
- Treat `appsettings*.json` as non-secret defaults only.
- Replace default local credentials (`app/app`) for non-local environments.
