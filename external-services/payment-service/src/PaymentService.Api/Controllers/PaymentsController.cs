using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Api.Contracts;
using PaymentService.Infrastructure.Database;
using PaymentService.Infrastructure.Models;

namespace PaymentService.Api.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(PaymentDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<PaymentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var payments = await dbContext.Payments
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(200)
            .Select(x => new PaymentDto(
                x.Id,
                x.ClientId,
                x.Amount,
                x.Currency,
                x.Description,
                x.Status,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return Ok(payments);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var payment = await dbContext.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (payment is null)
        {
            return NotFound();
        }

        return Ok(ToDto(payment));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> Create(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ClientId = request.ClientId.Trim(),
            Amount = decimal.Round(request.Amount, 2, MidpointRounding.AwayFromZero),
            Currency = request.Currency.Trim().ToUpperInvariant(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Status = PaymentStatus.Accepted,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Payments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = ToDto(payment);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    private static PaymentDto ToDto(Payment payment) =>
        new(
            payment.Id,
            payment.ClientId,
            payment.Amount,
            payment.Currency,
            payment.Description,
            payment.Status,
            payment.CreatedAtUtc);
}
