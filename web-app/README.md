# Library Service Web App

Minimal static UI for `LibraryService.Api`.

## Run

1. Start API:
   `dotnet run --project app/src/LibraryService.Api/LibraryService.Api.csproj`
2. Start web app static server with Node (from repo root):
   `node web-app/server.js 5500`
3. Open:
   `http://127.0.0.1:5500`
4. Use interactive commands in the same terminal:
   `help`, `status`, `open`, `quit`
5. Use the top menu to navigate to entity pages:
   `Books`, `Ebooks`, `Subscription Types`, `Subscriptions`, `Payments`

## Stop

- In PowerShell:
  run `quit` in the interactive prompt, or `Ctrl+C`

Default API base URL in the UI is `http://localhost:5149`.
