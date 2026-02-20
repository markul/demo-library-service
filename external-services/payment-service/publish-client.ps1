param(
    [string]$Configuration = "Release",
    [string]$Output = "..\\local-nuget"
)

$ErrorActionPreference = "Stop"

dotnet pack .\src\PaymentService.Client\PaymentService.Client.csproj `
    -c $Configuration `
    -p:GenerateClient=true `
    -o $Output
