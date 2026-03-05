## LibraryService Project Structure Overview

### Solution Architecture
```
demo-library-service/
├── app/
│   ├── LibraryService.sln              # Main solution file
│   ├── Directory.Build.props           # Shared build configuration
│   ├── NuGet.config                    # NuGet package sources
│   ├── src/
│   │   ├── LibraryService.Api/         # API Layer (Controllers, Startup)
│   │   ├── LibraryService.Application/ # Application Layer (Commands, Queries, Services)
│   │   ├── LibraryService.Domain/      # Domain Layer (Entities, Models)
│   │   └── LibraryService.Infrastructure/ # Infrastructure Layer (DB, Repositories, External Services)
│   └── test/
│       ├── LibraryService.Tests.Unit/  # Unit tests
│       └── LibraryService.Tests.Integration/ # Integration tests
├── infrastructure/
│   └── docker-compose.yml              # Docker services (app-db, jira, confluence)
├── external-services/
│   ├── ebook-service/                  # External ebook catalog service
│   └── payment-service/                # External payment service
└── web-app/                            # Frontend application
```

### Layer Breakdown

**[`LibraryService.Domain`](app/src/LibraryService.Domain/)** (Core)
- Entities: [`Book`](app/src/LibraryService.Domain/Entities/Book.cs), [`Client`](app/src/LibraryService.Domain/Entities/Client.cs), [`Journal`](app/src/LibraryService.Domain/Entities/Journal.cs), [`Payment`](app/src/LibraryService.Domain/Entities/Payment.cs), [`Subscription`](app/src/LibraryService.Domain/Entities/Subscription.cs), [`SubscriptionType`](app/src/LibraryService.Domain/Entities/SubscriptionType.cs)

**[`LibraryService.Application`](app/src/LibraryService.Application/)** (Business Logic)
- Commands/Queries using MediatR pattern
- Repository interfaces ([`IBookRepository`](app/src/LibraryService.Application/Abstractions/Repositories/IBookRepository.cs), [`IClientRepository`](app/src/LibraryService.Application/Abstractions/Repositories/IClientRepository.cs), etc.)
- Services ([`IEbookCatalogService`](app/src/LibraryService.Application/Abstractions/Services/IEbookCatalogService.cs))

**[`LibraryService.Infrastructure`](app/src/LibraryService.Infrastructure/)** (Data & External Access)
- EF Core [`LibraryDbContext`](app/src/LibraryService.Infrastructure/Database/LibraryDbContext.cs)
- Migrations under [`Database/Migrations/`](app/src/LibraryService.Infrastructure/Database/Migrations/)
- Repository implementations
- External service integrations (Ebook OData, Payment Service client)

**[`LibraryService.Api`](app/src/LibraryService.Api/)** (HTTP Layer)
- Controllers: [`BooksController`](app/src/LibraryService.Api/Controllers/BooksController.cs), [`ClientsController`](app/src/LibraryService.Api/Controllers/ClientsController.cs), [`EbooksController`](app/src/LibraryService.Api/Controllers/EbooksController.cs), [`JournalsController`](app/src/LibraryService.Api/Controllers/JournalsController.cs), [`PaymentsController`](app/src/LibraryService.Api/Controllers/PaymentsController.cs), [`SubscriptionsController`](app/src/LibraryService.Api/Controllers/SubscriptionsController.cs)
- API docs under [`ApiDocs/`](app/src/LibraryService.Api/ApiDocs/)
- Configuration: [`appsettings.json`](app/src/LibraryService.Api/appsettings.json), [`appsettings.Development.json`](app/src/LibraryService.Api/appsettings.Development.json)

### External Services
- **ebook-service**: Standalone service for ebook catalog (OData endpoints)
- **payment-service**: External payment processing service with client SDK

### Infrastructure
- Docker Compose with PostgreSQL (`app-db`), Jira, Confluence
- Database migrations stored in [`ManualScripts/`](app/src/LibraryService.Infrastructure/Database/ManualScripts/)

### Skills/Tools
- [`atllasian`](skills/atllasian/): Jira/Confluence/Bitbucket REST API access
- [`utf8-bom-converter`](skills/utf8-bom-converter/): File encoding normalization

This is a clean DDD-style .NET 8.0 solution with clear separation of concerns, EF Core migrations, and integration with external services.


### CASE RESULTS
- Passed.
- Comments: Agent renerated sufficient overview report.

