using FluentAssertions;
using LibraryService.Application.Payments;
using LibraryService.Application.Subscriptions;
using LibraryService.Domain.Entities;
using LibraryService.Tests.Integration.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace LibraryService.Tests.Integration.Controllers;

public class SubscriptionCheckoutIntegrationTests : IClassFixture<LibraryApiFactory>
{
    private readonly HttpClient _client;

    public SubscriptionCheckoutIntegrationTests(LibraryApiFactory factory)
    {
        _client = factory.CreateClient();
        factory.ResetAndSeed();
    }

    [Fact]
    public async Task Checkout_ShouldReturnCreatedThenOk_ForIdempotentSuccessfulRetry()
    {
        var request = new CheckoutSubscriptionRequest
        {
            ClientId = LibraryApiFactory.AliceClientId,
            SubscriptionTypeId = LibraryApiFactory.StandardSubscriptionTypeId,
            IdempotencyKey = "checkout-ok-1",
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);
        var firstBody = await firstResponse.Content.ReadFromJsonAsync<CheckoutSubscriptionResponse>();

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        firstBody.Should().NotBeNull();
        firstBody!.PaymentStatus.Should().Be("Paid");

        var secondResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);
        var secondBody = await secondResponse.Content.ReadFromJsonAsync<CheckoutSubscriptionResponse>();

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        secondBody.Should().NotBeNull();
        secondBody!.SubscriptionId.Should().Be(firstBody.SubscriptionId);
        secondBody.PaymentStatus.Should().Be("Paid");

        var subscriptionResponse = await _client.GetAsync($"/api/subscriptions/{firstBody.SubscriptionId}");
        var subscription = await subscriptionResponse.Content.ReadFromJsonAsync<SubscriptionDto>();
        subscriptionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        subscription.Should().NotBeNull();
        subscription!.IsActive.Should().BeTrue();
        subscription.Name.Should().Be("Checkout subscription");

        var paymentsResponse = await _client.GetAsync("/api/payments");
        var payments = await paymentsResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<PaymentDto>>();
        paymentsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        payments.Should().NotBeNull();
        var payment = payments!.Single(x => x.UniqueId == request.IdempotencyKey);
        payment.Status.Should().Be(PaymentStatus.Paid);
    }

    [Fact]
    public async Task Checkout_ShouldReturnPaymentRequired_ForRejectedPaymentAndRetry()
    {
        var request = new CheckoutSubscriptionRequest
        {
            ClientId = LibraryApiFactory.AliceClientId,
            SubscriptionTypeId = LibraryApiFactory.StandardSubscriptionTypeId,
            IdempotencyKey = "checkout-reject-1",
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);
        var firstBody = await firstResponse.Content.ReadFromJsonAsync<CheckoutSubscriptionResponse>();

        firstResponse.StatusCode.Should().Be(HttpStatusCode.PaymentRequired);
        firstBody.Should().NotBeNull();
        firstBody!.PaymentStatus.Should().Be("Failed");

        var secondResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);
        var secondBody = await secondResponse.Content.ReadFromJsonAsync<CheckoutSubscriptionResponse>();

        secondResponse.StatusCode.Should().Be(HttpStatusCode.PaymentRequired);
        secondBody.Should().NotBeNull();
        secondBody!.SubscriptionId.Should().Be(firstBody.SubscriptionId);
        secondBody.PaymentStatus.Should().Be("Failed");
    }

    [Fact]
    public async Task Checkout_ShouldReturnAccepted_WhenPaymentServiceFailsTechnically()
    {
        var request = new CheckoutSubscriptionRequest
        {
            ClientId = LibraryApiFactory.BobClientId,
            SubscriptionTypeId = LibraryApiFactory.PremiumSubscriptionTypeId,
            IdempotencyKey = "checkout-error-1",
        };

        var response = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);
        var body = await response.Content.ReadFromJsonAsync<CheckoutSubscriptionResponse>();

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        body.Should().NotBeNull();
        body!.PaymentStatus.Should().Be("Processing");

        var subscriptionResponse = await _client.GetAsync($"/api/subscriptions/{body.SubscriptionId}");
        var subscription = await subscriptionResponse.Content.ReadFromJsonAsync<SubscriptionDto>();
        subscriptionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        subscription.Should().NotBeNull();
        subscription!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Checkout_ShouldReturnNotFound_WhenClientOrSubscriptionTypeDoesNotExist()
    {
        var request = new CheckoutSubscriptionRequest
        {
            ClientId = Guid.NewGuid(),
            SubscriptionTypeId = LibraryApiFactory.StandardSubscriptionTypeId,
            IdempotencyKey = "checkout-notfound-1",
        };

        var response = await _client.PostAsJsonAsync("/api/subscriptions/checkout", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Checkout_ShouldReturnConflict_WhenIdempotencyKeyIsReusedWithDifferentPayload()
    {
        var firstRequest = new CheckoutSubscriptionRequest
        {
            ClientId = LibraryApiFactory.AliceClientId,
            SubscriptionTypeId = LibraryApiFactory.StandardSubscriptionTypeId,
            IdempotencyKey = "checkout-conflict-1",
        };
        var secondRequest = new CheckoutSubscriptionRequest
        {
            ClientId = LibraryApiFactory.AliceClientId,
            SubscriptionTypeId = LibraryApiFactory.PremiumSubscriptionTypeId,
            IdempotencyKey = "checkout-conflict-1",
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", firstRequest);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondResponse = await _client.PostAsJsonAsync("/api/subscriptions/checkout", secondRequest);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
