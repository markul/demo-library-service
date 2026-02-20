using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Database;
using PaymentService.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Payment Service API";
});
builder.Services.AddPaymentInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    dbContext.Database.Migrate();
}

app.UseOpenApi();
app.UseSwaggerUi();

app.MapControllers();

app.Run();
