Implemented explicit Arrange/Act/Assert structure across all test methods in the test suite.

Updated files:
- [LibraryControllersIntegrationTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Integration/Controllers/LibraryControllersIntegrationTests.cs)
- [BookHandlersTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Books/BookHandlersTests.cs)
- [ClientHandlersTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Clients/ClientHandlersTests.cs)
- [EbookCatalogServiceTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Infrastructure/EbookCatalogServiceTests.cs)
- [PaymentServiceTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Infrastructure/PaymentServiceTests.cs)
- [JournalHandlersTests.cs](c:/Users/vagrant/dev/git-markul/demo-library-service/app/test/LibraryService.Tests.Unit/Journals/JournalHandlersTests.cs)

Validation:
- Ran `dotnet test app/LibraryService.sln`
- Result: all tests passed (`18` unit + `9` integration)
- Note: existing NuGet vulnerability warnings (`NU1903`) were reported for `System.Net.Http` and `System.Text.RegularExpressions` packages.


### CASE RESULTS
- Passed.
- Comments: Agent updated tests, verified build and tests afterwards.