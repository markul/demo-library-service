# Implementation Plan for DEMO-19

> **Note**: This is a template implementation plan. The actual Jira issue details need to be retrieved from Jira at http://localhost:8080 to complete this plan.

## Jira Issue Information (to be filled)

- **Issue Key**: DEMO-19
- **Summary**: 
- **Description**: 
- **Issue Type**: 
- **Priority**: 
- **Assignee**: 
- **Reporter**: 
- **Created**: 
- **Updated**: 

---

## Project Architecture Overview

### Layered Architecture
```
LibraryService.Api (REST API layer)
    ↓
LibraryService.Application (Business logic layer)
    ↓
LibraryService.Domain (Domain entities)
    ↓
LibraryService.Infrastructure (Data access & external services)
```

### Key Technologies
- **Framework**: .NET 8.0
- **ORM**: Entity Framework Core 8.0.11
- **Database**: PostgreSQL 16
- **Mediator Pattern**: MediatR 12.4.1
- **API Documentation**: Swagger 6.6.2

### Project Structure
```
app/
├── src/
│   ├── LibraryService.Api/
│   │   ├── Controllers/          # REST endpoints
│   │   ├── ApiDocs/              # API documentation
│   │   └── Program.cs            # Application entry point
│   ├── LibraryService.Application/
│   │   ├── Abstractions/         # Interfaces (repositories, services)
│   │   ├── Books/                # Book domain operations
│   │   ├── Journals/             # Journal domain operations
│   │   ├── Clients/              # Client domain operations
│   │   ├── Subscriptions/        # Subscription domain operations
│   │   ├── Payments/             # Payment domain operations
│   │   └── Ebooks/               # Ebook catalog integration
│   └── LibraryService.Infrastructure/
│       ├── Database/             # EF Core context & migrations
│       ├── Repositories/         # Repository implementations
│       └── Services/             # External service clients
└── test/                         # Test projects (to be added)
```

---

## Implementation Steps

### Step 1: Database Changes (if applicable)

#### 1.1 Create Migration
```bash
dotnet ef migrations add <MigrationName> \
  --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj \
  --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj \
  --context LibraryDbContext \
  --output-dir Database/Migrations
```

#### 1.2 Generate SQL Scripts
```bash
# Forward migration
dotnet ef migrations script <PreviousMigration> <MigrationName> \
  --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj \
  --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj \
  --context LibraryDbContext \
  -o app/src/LibraryService.Infrastructure/Database/ManualScripts/<NNN>-<MigrationName>.sql \
  -i

# Rollback migration
dotnet ef migrations script <MigrationName> <PreviousMigration> \
  --project app/src/LibraryService.Infrastructure/LibraryService.Infrastructure.csproj \
  --startup-project app/src/LibraryService.Api/LibraryService.Api.csproj \
  --context LibraryDbContext \
  -o app/src/LibraryService.Infrastructure/Database/ManualScripts/<NNN>-<MigrationName>_Revert.sql \
  -i
```

### Step 2: Domain Layer Changes

#### 2.1 Create/Update Entity
File: `app/src/LibraryService.Domain/Entities/<EntityName>.cs`

```csharp
namespace LibraryService.Domain.Entities;

public class <EntityName>
{
    public Guid Id { get; set; }
    // Add properties based on requirements
}
```

#### 2.2 Create Configuration
File: `app/src/LibraryService.Infrastructure/Database/Configurations/<EntityName>Configuration.cs`

```csharp
using LibraryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryService.Infrastructure.Database.Configurations;

public class <EntityName>Configuration : IEntityTypeConfiguration<<EntityName>>
{
    public void Configure(EntityTypeBuilder<<EntityName>> builder)
    {
        builder.ToTable("<entity_name>");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        // Add property configurations
    }
}
```

### Step 3: Application Layer Changes

#### 3.1 Update Repository Interface
File: `app/src/LibraryService.Application/Abstractions/Repositories/I<EntityName>Repository.cs`

```csharp
using LibraryService.Domain.Entities;

namespace LibraryService.Application.Abstractions.Repositories;

public interface I<EntityName>Repository
{
    Task<IReadOnlyCollection<<EntityName>>> GetAllAsync(CancellationToken cancellationToken);
    Task<<EntityName>?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<<EntityName>> AddAsync(<EntityName> entity, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(<EntityName> entity, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

#### 3.2 Create Commands/Queries
File: `app/src/LibraryService.Application/<EntityName>/Commands/<CommandName>.cs`

```csharp
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using MediatR;

namespace LibraryService.Application.<EntityName>.Commands;

public record <CommandName>(/* parameters */) : IRequest<<EntityName>Dto>;

public class <CommandName>Handler : IRequestHandler<<CommandName>, <EntityName>Dto>
{
    private readonly I<EntityName>Repository _repository;

    public <CommandName>Handler(I<EntityName>Repository repository)
    {
        _repository = repository;
    }

    public async Task<<EntityName>Dto> Handle(<CommandName> request, CancellationToken cancellationToken)
    {
        // Business logic
    }
}
```

#### 3.3 Create DTO
File: `app/src/LibraryService.Application/<EntityName>/<EntityName>Dto.cs`

```csharp
namespace LibraryService.Application.<EntityName>;

public record <EntityName>Dto(
    Guid Id,
    // Properties
);
```

### Step 4: Infrastructure Layer Changes

#### 4.1 Update DbContext
File: `app/src/LibraryService.Infrastructure/Database/LibraryDbContext.cs`

```csharp
public class LibraryDbContext : DbContext
{
    // Existing DbSets...
    public DbSet<<EntityName>> <EntityName>s => Set<<EntityName>>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
```

#### 4.2 Implement Repository
File: `app/src/LibraryService.Infrastructure/Repositories/<EntityName>Repository.cs`

```csharp
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Domain.Entities;
using LibraryService.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LibraryService.Infrastructure.Repositories;

public class <EntityName>Repository : I<EntityName>Repository
{
    private readonly LibraryDbContext _dbContext;

    public <EntityName>Repository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<<EntityName>>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.<EntityName>s
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<<EntityName>?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.<EntityName>s
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<<EntityName>> AddAsync(<EntityName> entity, CancellationToken cancellationToken)
    {
        _dbContext.<EntityName>s.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(<EntityName> entity, CancellationToken cancellationToken)
    {
        _dbContext.<EntityName>s.Update(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.<EntityName>s.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _dbContext.<EntityName>s.Remove(entity);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
```

#### 4.3 Register Services
File: `app/src/LibraryService.Infrastructure/ServiceCollectionExtensions.cs`

```csharp
services.AddScoped<I<EntityName>Repository, <EntityName>Repository>();
```

### Step 5: API Layer Changes

#### 5.1 Create Controller
File: `app/src/LibraryService.Api/Controllers/<EntityName>Controller.cs`

```csharp
using LibraryService.Application.<EntityName>;
using LibraryService.Application.<EntityName>.Commands;
using LibraryService.Application.<EntityName>.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryService.Api.Controllers;

[ApiController]
[Route("api/<entity_name_plural>")]
public class <EntityName>Controller : ControllerBase
{
    private readonly IMediator _mediator;

    public <EntityName>Controller(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<<EntityName>Dto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _mediator.Send(new Get<EntityName>sQuery(), cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<<EntityName>Dto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var item = await _mediator.Send(new Get<EntityName>ByIdQuery(id), cancellationToken);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<<EntityName>Dto>> Create(<EntityName>Request request, CancellationToken cancellationToken)
    {
        var command = new Create<EntityName>Command(request.<Property1>, request.<Property2>);
        var created = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, <EntityName>Request request, CancellationToken cancellationToken)
    {
        var command = new Update<EntityName>Command(id, request.<Property1>, request.<Property2>);
        var updated = await _mediator.Send(command, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new Delete<EntityName>Command(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
```

#### 5.2 Update API Documentation
File: `app/src/LibraryService.Api/ApiDocs/<EntityName>/<MethodName>.md`

Follow the existing documentation pattern in `app/src/LibraryService.Api/ApiDocs/`.

#### 5.3 Update HTTP Test File
File: `app/src/LibraryService.Api/LibraryService.Api.http`

Add test requests following the existing pattern.

---

## Testing

### Unit Tests
Add tests under `app/test/<TestProject>/`.

### Integration Tests
Run database migrations and test endpoints.

### Build & Run
```bash
# Restore dependencies
dotnet restore app/LibraryService.sln

# Build
dotnet build app/LibraryService.sln

# Run API
dotnet run --project app/src/LibraryService.Api/LibraryService.Api.csproj
```

---

## Deployment Checklist

- [ ] Database migration created and tested
- [ ] SQL scripts generated for forward and rollback
- [ ] API documentation updated
- [ ] Tests added/updated
- [ ] Build succeeds without errors
- [ ] Local testing completed
- [ ] Docker compose configuration updated (if applicable)

---

## Notes

1. All .cs files should use UTF-8 BOM encoding
2. Follow the existing naming conventions (PascalCase for public members)
3. Keep controllers thin - business logic in Application layer
4. Update API docs for any endpoint changes
5. Ensure migrations are generated using `dotnet ef` (not manually written)



# Implementation Plan for DEMO-19: Subscription Payment API

## Issue Summary
**Issue Key:** DEMO-19  
**Summary:** Добавить api для оплаты подписки в Library Service (Add API for subscription payment in Library Service)  
**Status:** Backlog  
**Priority:** Medium  
**Labels:** db-write, ebooks, integration, payments, saga

## Requirements (from Confluence documentation)
The payment process for subscriptions involves:
1. Validate client and subscription type
2. Calculate subscription price
3. Create new subscription record
4. Create payment record
5. Send payment to PaymentService
6. Update subscription and payment based on response

## Implementation Plan

### 1. New Application Layer Components

#### 1.1 DTOs (LibraryService.Application)
| File | Description |
|------|-------------|
| `Subscriptions/SubscriptionCheckoutResult.cs` | Result DTO with SubscriptionId and PaymentStatus |
| `Subscriptions/CheckoutSubscriptionRequest.cs` | Request DTO for checkout endpoint |
| `Subscriptions/CheckoutSubscriptionCommand.cs` | MediatR command for checkout |
| `Subscriptions/CheckoutSubscriptionCommandHandler.cs` | Handler implementing checkout logic |

### 2. New Domain Service

#### 2.1 ISubscriptionPaymentService (LibraryService.Application.Abstractions.Services)
| Method | Description |
|--------|-------------|
| `CheckoutSubscriptionAsync()` | Process subscription payment with saga pattern |

### 3. New API Endpoint

#### 3.1 SubscriptionsController
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/subscriptions/checkout` | POST | Process subscription payment |

### 4. Database Changes
No new tables needed - existing schema supports subscriptions and payments.

### 5. Documentation (LibraryService.Api/ApiDocs/Subscriptions/CheckoutSubscription)
| File | Description |
|------|-------------|
| `CheckoutSubscription.md` | API documentation |
| `Diagrams/CheckoutSubscription/Algorithm.puml/svg` | Algorithm diagram |
| `Examples/CheckoutSubscription/Input.md` | Request example |
| `Examples/CheckoutSubscription/Output.md` | Response example |

### 6. Implementation Steps

1. **Create DTOs** - Define request/response types
2. **Create Command & Handler** - Implement MediatR pattern
3. **Create Service** - Implement business logic with saga pattern
4. **Add Controller Endpoint** - Expose checkout API
5. **Update API Docs** - Document the new endpoint
6. **Add Tests** - Unit and integration tests
7. **Database Migration** - Verify existing schema is sufficient

### 7. Key Design Decisions

- **SAGA Pattern:** The payment process follows a saga pattern with compensating actions
- **Idempotency:** Use `uniqueId` for idempotent payment requests
- **Transaction Scope:** Database operations wrapped in transaction
- **External Integration:** PaymentService integration point for external payment processing

### 8. Dependencies
- Existing: `LibraryDbContext` for database operations
- Existing: Payment and Subscription repositories
- External: PaymentService (to be integrated)


### CASE RESULTS
- Partially/Failed.
- Comments: Agent created plan, but failed to follow it. Some required logic is missing, tests are not updated. 

