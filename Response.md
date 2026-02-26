Plan

I have enough context and will implement this in one pass with the existing architecture:

1. Add domain + EF model for ClientAddress (Entity, DbSet, Configuration, relationship on Client).
2. Add application flow for create (DTO/request, CreateClientAddressCommand, repository abstraction).
3. Add infrastructure repository + DI registration and expose a new API action on ClientsController.
4. Generate EF migration + forward/rollback SQL scripts under Database/ManualScripts.
5. Update API docs (ApiDocs/Clients) and LibraryService.Api.http.
6. Add unit + integration tests for the new endpoint and run dotnet build and dotnet test.



Implemented `ClientAddress` end-to-end with create endpoint.

- Added new domain entity and client relationship:  
[ClientAddress.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Domain/Entities/ClientAddress.cs:3), [Client.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Domain/Entities/Client.cs:15)
- Wired EF Core model/table mapping + DbSet + DI/repository:  
[LibraryDbContext.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Database/LibraryDbContext.cs:19), [ClientAddressConfiguration.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Database/Configurations/ClientAddressConfiguration.cs:7), [IClientAddressRepository.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Abstractions/Repositories/IClientAddressRepository.cs:5), [ClientAddressRepository.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Repositories/ClientAddressRepository.cs:8), [ServiceCollectionExtensions.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/ServiceCollectionExtensions.cs:26)
- Added application command + request/DTO contracts:  
[CreateClientAddressCommand.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Clients/Commands/CreateClientAddressCommand.cs:7), [ClientAddressDto.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Application/Clients/ClientAddressDto.cs:3)
- Added endpoint `POST /api/clients/addresses`:  
[ClientsController.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/Controllers/ClientsController.cs:42), sample request in [LibraryService.Api.http](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/LibraryService.Api.http:83)
- Added migration + SQL scripts:  
[20260226095147_AddClientAddresses.cs](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Database/Migrations/20260226095147_AddClientAddresses.cs:9), [006-AddClientAddresses.sql](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Database/ManualScripts/006-AddClientAddresses.sql), [006-AddClientAddresses_Revert.sql](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Infrastructure/Database/ManualScripts/006-AddClientAddresses_Revert.sql)
- Added API docs + examples + diagram:  
[CreateClientAddress.md](C:/Users/vagrant/dev/git-markul/demo-library-service/app/src/LibraryService.Api/ApiDocs/Clients/CreateClientAddress.md:1)

Tests and build:

- `dotnet build app/LibraryService.sln` passed.
- `dotnet test app/LibraryService.sln` passed (Unit: 20, Integration: 11).
- Existing NU1903 dependency vulnerability warnings are still present (pre-existing, unchanged by this feature).


### CASE RESULTS
- Passed.
- Comments: Agent added entity, all corresponding application and infrastructure logic. Added docs, migrations, tests. Verified that all tests pass.