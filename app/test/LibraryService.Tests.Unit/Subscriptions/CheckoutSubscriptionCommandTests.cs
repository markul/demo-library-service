using FluentAssertions;
using LibraryService.Application.Abstractions.Repositories;
using LibraryService.Application.Abstractions.Services;
using LibraryService.Application.Subscriptions.Commands;
using LibraryService.Domain.Entities;
using Moq;

namespace LibraryService.Tests.Unit.Subscriptions;

public class CheckoutSubscriptionCommandTests
{
    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenClientOrSubscriptionTypeDoesNotExist()
    {
        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCheckoutExistingPayment?)null);
        repository
            .Setup(x => x.GetCheckoutInputDataAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCheckoutInputData?)null);

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(Guid.NewGuid(), Guid.NewGuid(), "key-1"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.NotFound);
        result.Response.Should().BeNull();
        repository.Verify(
            x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        paymentGateway.Verify(
            x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldApplyDiscountAndReturnCreated_WhenPaymentIsAccepted()
    {
        var clientId = Guid.NewGuid();
        var subscriptionTypeId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();
        SubscriptionCheckoutPaymentRequest? capturedPaymentRequest = null;

        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCheckoutExistingPayment?)null);
        repository
            .Setup(x => x.GetCheckoutInputDataAsync(clientId, subscriptionTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutInputData(99.99m, "Premium", 6));
        repository
            .Setup(x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutPendingResult(subscriptionId, paymentId));
        repository
            .Setup(x => x.UpdateCheckoutResultAsync(
                subscriptionId,
                paymentId,
                PaymentStatus.Paid,
                true,
                "external-1",
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        paymentGateway
            .Setup(x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()))
            .Callback<SubscriptionCheckoutPaymentRequest, CancellationToken>((request, _) => capturedPaymentRequest = request)
            .ReturnsAsync(new SubscriptionCheckoutPaymentResult(
                SubscriptionCheckoutPaymentStatus.Accepted,
                "external-1"));

        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(clientId, subscriptionTypeId, "key-2"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.Created);
        result.Response.Should().NotBeNull();
        result.Response!.SubscriptionId.Should().Be(subscriptionId);
        result.Response.PaymentStatus.Should().Be("Paid");

        capturedPaymentRequest.Should().NotBeNull();
        capturedPaymentRequest!.Amount.Should().Be(94.99m);
        repository.Verify(
            x => x.UpdateCheckoutResultAsync(
                subscriptionId,
                paymentId,
                PaymentStatus.Paid,
                true,
                "external-1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaymentRequired_WhenPaymentIsRejected()
    {
        var clientId = Guid.NewGuid();
        var subscriptionTypeId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-3", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCheckoutExistingPayment?)null);
        repository
            .Setup(x => x.GetCheckoutInputDataAsync(clientId, subscriptionTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutInputData(10m, "Standard", 2));
        repository
            .Setup(x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutPendingResult(subscriptionId, paymentId));
        repository
            .Setup(x => x.UpdateCheckoutResultAsync(
                subscriptionId,
                paymentId,
                PaymentStatus.Failed,
                false,
                "external-2",
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        paymentGateway
            .Setup(x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutPaymentResult(
                SubscriptionCheckoutPaymentStatus.Rejected,
                "external-2"));

        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(clientId, subscriptionTypeId, "key-3"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.PaymentRequired);
        result.Response.Should().NotBeNull();
        result.Response!.PaymentStatus.Should().Be("Failed");
    }

    [Fact]
    public async Task Handle_ShouldReturnAccepted_WhenPaymentGatewayReturnsTechnicalFailure()
    {
        var clientId = Guid.NewGuid();
        var subscriptionTypeId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var paymentId = Guid.NewGuid();

        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-4", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionCheckoutExistingPayment?)null);
        repository
            .Setup(x => x.GetCheckoutInputDataAsync(clientId, subscriptionTypeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutInputData(12m, "Standard", 0));
        repository
            .Setup(x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutPendingResult(subscriptionId, paymentId));
        repository
            .Setup(x => x.UpdateCheckoutResultAsync(
                subscriptionId,
                paymentId,
                PaymentStatus.Processing,
                false,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        paymentGateway
            .Setup(x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutPaymentResult(
                SubscriptionCheckoutPaymentStatus.TechnicalFailure,
                null));

        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(clientId, subscriptionTypeId, "key-4"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.Accepted);
        result.Response.Should().NotBeNull();
        result.Response!.PaymentStatus.Should().Be("Processing");
    }

    [Fact]
    public async Task Handle_ShouldReturnOkWithoutCreatingNewCheckout_WhenIdempotentPaidRecordExists()
    {
        var clientId = Guid.NewGuid();
        var subscriptionTypeId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();

        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-5", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutExistingPayment(
                clientId,
                subscriptionTypeId,
                subscriptionId,
                PaymentStatus.Paid));

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(clientId, subscriptionTypeId, "key-5"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.Ok);
        result.Response.Should().NotBeNull();
        result.Response!.PaymentStatus.Should().Be("Paid");

        repository.Verify(
            x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        paymentGateway.Verify(
            x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenIdempotencyKeyIsReusedWithDifferentPayload()
    {
        var repository = new Mock<ISubscriptionCheckoutRepository>();
        repository
            .Setup(x => x.GetByIdempotencyKeyAsync("key-6", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionCheckoutExistingPayment(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                PaymentStatus.Paid));

        var paymentGateway = new Mock<ISubscriptionCheckoutPaymentGateway>();
        var handler = new CheckoutSubscriptionCommandHandler(repository.Object, paymentGateway.Object);

        var result = await handler.Handle(
            new CheckoutSubscriptionCommand(Guid.NewGuid(), Guid.NewGuid(), "key-6"),
            CancellationToken.None);

        result.Outcome.Should().Be(CheckoutSubscriptionOutcome.Conflict);
        result.Response.Should().BeNull();

        repository.Verify(
            x => x.CreatePendingCheckoutAsync(It.IsAny<SubscriptionCheckoutPendingCreateRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        paymentGateway.Verify(
            x => x.CreatePaymentAsync(It.IsAny<SubscriptionCheckoutPaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
