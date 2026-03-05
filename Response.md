## Project Structure Overview: Demo Library Service

This is a .NET 8 library management system following Clean Architecture principles.

### Root Structure
```
├── app/                    # Main application solution
├── docs/                   # Documentation
├── external-services/      # External service mocks/simulators
├── infrastructure/         # Docker infrastructure
├── skills/                 # Agent skills (Atlassian, UTF-8 BOM)
└── web-app/               # Simple web frontend
```

### Application Layers ([`app/`](app/))

**Solution**: [`LibraryService.sln`](app/LibraryService.sln)

| Layer | Project | Purpose |
|-------|---------|---------|
| **Api** | [`LibraryService.Api`](app/src/LibraryService.Api/) | REST API host, controllers, HTTP concerns |
| **Application** | [`LibraryService.Application`](app/src/LibraryService.Application/) | CQRS commands/queries, DTOs, business logic |
| **Domain** | [`LibraryService.Domain`](app/src/LibraryService.Domain/) | Entity models, domain primitives |
| **Infrastructure** | [`LibraryService.Infrastructure`](app/src/LibraryService.Infrastructure/) | EF Core, repositories, external services |

### Key Components

**Controllers** ([`Api/Controllers`](app/src/LibraryService.Api/Controllers/)):
- [`BooksController`](app/src/LibraryService.Api/Controllers/BooksController.cs)
- [`ClientsController`](app/src/LibraryService.Api/Controllers/ClientsController.cs)
- [`EbooksController`](app/src/LibraryService.Api/Controllers/EbooksController.cs)
- [`JournalsController`](app/src/LibraryService.Api/Controllers/JournalsController.cs)
- [`PaymentsController`](app/src/LibraryService.Api/Controllers/PaymentsController.cs)
- [`SubscriptionsController`](app/src/LibraryService.Api/Controllers/SubscriptionsController.cs)

**Domain Entities** ([`Domain/Entities`](app/src/LibraryService.Domain/Entities/)):
- [`Book`](app/src/LibraryService.Domain/Entities/Book.cs), [`Journal`](app/src/LibraryService.Domain/Entities/Journal.cs)
- [`Client`](app/src/LibraryService.Domain/Entities/Client.cs)
- [`Subscription`](app/src/LibraryService.Domain/Entities/Subscription.cs), [`SubscriptionType`](app/src/LibraryService.Domain/Entities/SubscriptionType.cs)
- [`Payment`](app/src/LibraryService.Domain/Entities/Payment.cs), [`PaymentStatus`](app/src/LibraryService.Domain/Entities/PaymentStatus.cs)

**Database** ([`Infrastructure/Database`](app/src/LibraryService.Infrastructure/Database/)):
- EF Core migrations in [`Migrations/`](app/src/LibraryService.Infrastructure/Database/Migrations/)
- Manual SQL scripts in [`ManualScripts/`](app/src/LibraryService.Infrastructure/Database/ManualScripts/)
- Entity configurations in [`Configurations/`](app/src/LibraryService.Infrastructure/Database/Configurations/)

### Tests ([`app/test/`](app/test/))
- [`LibraryService.Tests.Unit`](app/test/LibraryService.Tests.Unit/) - Unit tests for handlers, services
- [`LibraryService.Tests.Integration`](app/test/LibraryService.Tests.Integration/) - Integration tests for controllers

### External Services ([`external-services/`](external-services/))
- [`ebook-service/`](external-services/ebook-service/) - Mock ebook catalog with OData support
- [`payment-service/`](external-services/payment-service/) - Payment processing service with NuGet client

### Infrastructure ([`infrastructure/`](infrastructure/))
- [`docker-compose.yml`](infrastructure/docker-compose.yml) - PostgreSQL (app-db), Jira, Confluence containers

### API Documentation
Located at [`Api/ApiDocs/`](app/src/LibraryService.Api/ApiDocs/) with markdown docs, diagrams, and examples for each endpoint.

### CASE RESULTS
- Passed.
- Comments: Agent returned solid description.

