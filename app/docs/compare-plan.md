# AI Agent Capability Demo Plan

Use the same repository and evaluate each agent on the same tasks in the same order.  
Recommended scale for each item: `0 = not done`, `1 = partial`, `2 = complete`.

| # | Operation | Capability Demonstrated | Success Criteria |
|---|---|---|---|
| 1 | Read the project structure and briefly describe key modules | Repository navigation and context gathering | Correct description of `Api`, `Application`, `Domain`, `Infrastructure`, and tests |
| 2 | Run the build and report the result | Basic command execution | `dotnet build` result with clear pass/fail and errors |
| 3 | Find all usages of a method/class | Code search and impact analysis | Accurate list of references by file |
| 4 | Rename a method across all layers | Safe refactoring | Build passes, all references updated |
| 5 | Add a small DTO and wire it into one scenario | Contract evolution | DTO added and used in service/query/controller flow |
| 6 | Configure an OData request to return only selected fields | External API query composition | Generated query contains expected `$select` |
| 7 | Add unit tests for new behavior | Test authoring | New tests fail before change and pass after |
| 8 | Add an integration test for endpoint response shape | End-to-end validation | HTTP response validated for exact fields/status |
| 9 | Update API documentation and examples for new behavior | Documentation discipline | Markdown docs/examples updated in the same change |
| 10 | Implement a new service method by ID with validation | Business logic + error handling | Valid/invalid/not-found cases handled correctly |
| 11 | Investigate and fix a failing build/test | Debugging under constraints | Root cause explained and fix verified |
| 12 | Prepare a final summary of changes with file links and executed commands | Technical communication | Brief report with changed files, test results, and residual risks |

## Optional Advanced Extensions

| # | Operation | Capability Demonstrated | Success Criteria |
|---|---|---|---|
| 13 | Add a new API endpoint end-to-end (controller, query, service, docs, tests) | Full feature delivery lifecycle | Endpoint works and all checks pass |
| 14 | Add a migration + forward/rollback SQL scripts | Data lifecycle management | Migration generated correctly, scripts included |
| 15 | Integrate Jira/Confluence update as part of delivery | Tool orchestration | Issue/page updated with release notes 