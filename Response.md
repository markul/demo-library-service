Here is a concise overview of the **LibraryService** project structure based on the repository guidelines:

---

### 🏗️ High-Level Layout

```
LibraryService/
├── app/                     # Main application code
│   ├── LibraryService.sln   # Solution file
│   ├── src/                 # Source code
│   │   ├── LibraryService.Domain/       # Core domain model & business logic
│   │   ├── LibraryService.Application/  # Application services, CQRS, DTOs
│   │   ├── LibraryService.Infrastructure/ # Data access, EF migrations, external integrations
│   │   └── LibraryService.Api/          # Web API (controllers, middleware, configuration)
│   └── test/                # Test projects (currently placeholder)
│
├── infrastructure/          # Docker-based local infrastructure
│   └── docker-compose.yml   # Defines app-db, jira, confluence, etc.
│
├── external-services/       # Placeholder for 3rd-party service integrations
│
└── skills/                  # reusable automation tooling
    ├── atlassian/SKILL.md
    └── utf8-bom-converter/SKILL.md
```

---

### 📦 Layer Responsibilities

| Layer | Purpose | Key Components |
|------|---------|----------------|
| **Domain** | Core business entities, value objects, domain rules | `Book`, `Journal`, `Client`, `Subscription`, aggregates |
| **Application** | Use cases, command/query handlers, service interfaces | MediatR handlers, DTOs, interfaces for repositories, external APIs |
| **Infrastructure** | EF Core `LibraryDbContext`, migrations, file/queue/email services | Migrations, repository implementations, message queues |
| **Api** | HTTP layer: controllers, middleware, routing, config | Controllers (`/api/books`, `/api/journals`, etc.), Swagger, `Program.cs`, `appsettings*.json` |

---

### 🗄️ Database & Migrations

- **Primary DB**: `app-db` PostgreSQL (Docker service).
- **Connection strings**:
  - Default (Docker): `Host=app-db;...`
  - Local dev override: `Host=localhost;...`
- **Migrations**:
  - Stored under `app/src/LibraryService.Infrastructure/Database/Migrations`.
  - Generated via `dotnet ef` (never manually written).
  - Forward & rollback SQL scripts go to `ManualScripts/`.

---

### 🧪 Testing

- **Location**: `app/test/<ProjectName>`
- **Framework**: `xUnit`
- **Run**:  
  ```bash
  dotnet test app/LibraryService.sln
  ```
- **Naming**: `{MethodName}_{ExpectedResult}_{Condition}`  
- **Requirement**: Every API endpoint change must include corresponding test updates.

---

### 🌐 API

- **Endpoints**: `/api/books`, `/api/journals`, `/api/clients`, `/api/subscriptions`, `/api/subscriptions/types`, `/api/payments`
- **Docs**: `app/src/LibraryService.Api/ApiDocs/<Area>/{MethodName}.md` + diagrams + examples.
- **Http file**: `app/src/LibraryService.Api/LibraryService.Api.http`

---

### 📜 Style & Standards

- Target: `.NET 8.0`, nullable enabled, implicit usings.
- Encoding: **UTF-8 BOM** for `.cs` files.
- Namespace/structure: Matches folder layout.
- Controllers: Thin layer → delegate all business to **Application**.

---

Let me know if you’d like to explore any specific part in detail (e.g., folder layout of `Infrastructure`, migration workflow, or API docs template).



### CASE RESULTS
- Passed.
- Comments: Detailed response, covers most important aspects of the project.