FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/PaymentService.Api/PaymentService.Api.csproj", "src/PaymentService.Api/"]
COPY ["src/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "src/PaymentService.Infrastructure/"]
RUN dotnet restore "src/PaymentService.Api/PaymentService.Api.csproj"
COPY . .
WORKDIR "/src/src/PaymentService.Api"
RUN dotnet publish "PaymentService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.Api.dll"]
