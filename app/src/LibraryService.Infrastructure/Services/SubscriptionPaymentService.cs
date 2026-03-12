using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using LibraryService.Application.Abstractions.Services;

namespace LibraryService.Infrastructure.Services;

public class SubscriptionPaymentService : ISubscriptionPaymentService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public SubscriptionPaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentProcessingResult> ProcessPaymentAsync(
        Guid clientId,
        Guid subscriptionId,
        Guid paymentId,
        decimal amount,
        string uniqueId,
        CancellationToken cancellationToken)
    {
        // TODO: Replace with actual PaymentService URL from configuration
        var paymentServiceUrl = "http://localhost:5001/api/payments/process";

        var request = new
        {
            ClientId = clientId,
            SubscriptionId = subscriptionId,
            PaymentId = paymentId,
            Amount = amount,
            UniqueId = uniqueId
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(paymentServiceUrl, request, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>(cancellationToken);
                return new PaymentProcessingResult(true, result?.ExternalId?.ToString());
            }
            
            return new PaymentProcessingResult(false, null);
        }
        catch
        {
            return new PaymentProcessingResult(false, null);
        }
    }
}
