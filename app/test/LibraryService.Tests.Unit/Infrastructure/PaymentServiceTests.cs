using FluentAssertions;
using LibraryService.Infrastructure.Services;
using Moq;
using PaymentService.Client;

namespace LibraryService.Tests.Unit.Infrastructure;

public class PaymentServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnPayments_WhenClientReturnsData()
    {
        var expected = new List<PaymentDto>
        {
            new()
            {
                ClientId = "client-1",
                Amount = 10.50m,
                Currency = "USD",
                Description = "Payment 1",
                Status = PaymentStatus.Accepted,
                CreatedAtUtc = DateTimeOffset.UtcNow
            }
        };

        var client = new Mock<IPaymentServiceClient>();
        client
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new LibraryService.Infrastructure.Services.PaymentService(client.Object);

        var result = await service.GetAllAsync(CancellationToken.None);

        result.Should().BeSameAs(expected);
        client.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPayment_WhenClientReturnsData()
    {
        var id = Guid.NewGuid();
        var expected = new PaymentDto
        {
            ClientId = "client-42",
            Amount = 99.99m,
            Currency = "EUR",
            Description = "Payment 42",
            Status = PaymentStatus.Accepted,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var client = new Mock<IPaymentServiceClient>();
        client
            .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new LibraryService.Infrastructure.Services.PaymentService(client.Object);

        var result = await service.GetByIdAsync(id, CancellationToken.None);

        result.Should().BeSameAs(expected);
        client.Verify(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldDelegateToClient_WhenRequestIsProvided()
    {
        var request = new CreatePaymentRequest
        {
            ClientId = "new-client",
            Amount = 15.75m,
            Currency = "USD",
            Description = "New payment"
        };

        var expected = new PaymentDto
        {
            ClientId = request.ClientId,
            Amount = request.Amount,
            Currency = request.Currency,
            Description = request.Description,
            Status = PaymentStatus.Accepted,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var client = new Mock<IPaymentServiceClient>();
        client
            .Setup(x => x.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var service = new LibraryService.Infrastructure.Services.PaymentService(client.Object);

        var result = await service.CreateAsync(request, CancellationToken.None);

        result.Should().BeSameAs(expected);
        client.Verify(x => x.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
