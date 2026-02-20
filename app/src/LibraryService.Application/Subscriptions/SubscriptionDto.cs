namespace LibraryService.Application.Subscriptions;

public record SubscriptionTypeDto(Guid Id, string Name, int Period, decimal Price);

public record SubscriptionDto(
    Guid Id,
    string Name,
    Guid SubscriptionTypeId,
    bool IsActive,
    DateTime StartDateUtc,
    SubscriptionTypeDto Type,
    IReadOnlyCollection<Guid> ClientIds);

public record CreateSubscriptionRequest(
    string Name,
    Guid SubscriptionTypeId,
    bool IsActive,
    DateTime StartDateUtc,
    IReadOnlyCollection<Guid> ClientIds);

public record UpdateSubscriptionRequest(
    string Name,
    Guid SubscriptionTypeId,
    bool IsActive,
    DateTime StartDateUtc,
    IReadOnlyCollection<Guid> ClientIds);

public record CreateSubscriptionTypeRequest(string Name, int Period, decimal Price);

public record UpdateSubscriptionTypeRequest(string Name, int Period, decimal Price);
