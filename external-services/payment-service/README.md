# Payment Service

## Projects
- `src/PaymentService.Api` - REST API for accepting payments.
- `src/PaymentService.Infrastructure` - EF Core context and migrations.
- `src/PaymentService.Client` - generated NuGet client library.

## Local commands
- Build: `dotnet build PaymentService.sln`
- Run API: `dotnet run --project src/PaymentService.Api/PaymentService.Api.csproj`
- Add migration:
  `dotnet ef migrations add <MigrationName> --project src/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj --startup-project src/PaymentService.Api/PaymentService.Api.csproj --context PaymentDbContext --output-dir Database/Migrations`
- Apply migration:
  `dotnet ef database update --project src/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj --startup-project src/PaymentService.Api/PaymentService.Api.csproj --context PaymentDbContext`

## Client generation and publish
- Generate client:
  `dotnet build src/PaymentService.Client/PaymentService.Client.csproj -p:GenerateClient=true`
- Pack and publish to local NuGet folder:
  `dotnet pack src/PaymentService.Client/PaymentService.Client.csproj -c Release -p:GenerateClient=true -o ../local-nuget`
- Shortcut script:
  `./publish-client.ps1`
