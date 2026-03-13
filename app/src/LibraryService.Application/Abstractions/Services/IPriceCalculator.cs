namespace LibraryService.Application.Abstractions.Services;

public interface IPriceCalculator
{
    Task<PriceCalculationResult> CalculatePriceAsync(PriceCalculationInput input, CancellationToken cancellationToken);
}
