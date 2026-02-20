using LibraryService.Application.Abstractions.Repositories;
using MediatR;

namespace LibraryService.Application.Payments.Commands;

public record DeletePaymentCommand(Guid Id) : IRequest<bool>;

public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, bool>
{
    private readonly IPaymentRepository _repository;

    public DeletePaymentCommandHandler(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        return _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
