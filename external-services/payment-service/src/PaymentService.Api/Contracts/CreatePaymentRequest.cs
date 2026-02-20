using System.ComponentModel.DataAnnotations;

namespace PaymentService.Api.Contracts;

public sealed class CreatePaymentRequest
{
    [Required]
    [StringLength(128, MinimumLength = 1)]
    public string ClientId { get; set; } = string.Empty;

    [Range(0.01, 1000000000)]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression("^[A-Z]{3}$")]
    public string Currency { get; set; } = "USD";

    [StringLength(1024)]
    public string? Description { get; set; }
}
