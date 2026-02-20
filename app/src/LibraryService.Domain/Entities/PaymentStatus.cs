namespace LibraryService.Domain.Entities;

public enum PaymentStatus
{
    New = 1,
    Processing = 2,
    Paid = 3,
    Cancelled = 4,
    Failed = 5,
}
