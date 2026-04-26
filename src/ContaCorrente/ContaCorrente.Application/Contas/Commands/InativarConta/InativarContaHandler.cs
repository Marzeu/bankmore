using ContaCorrente.Application.Common.Exceptions;
using ContaCorrente.Application.Contas.Repositories;
using MediatR;
using ContaCorrente.Application.Common;
using ContaCorrente.Application.Common.Errors;

namespace ContaCorrente.Application.Contas.Commands.InativarConta;

public class InativarContaHandler : IRequestHandler<InativarContaCommand>
{
    private readonly IContaCorrenteRepository _repository;

    public InativarContaHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        InativarContaCommand request,
        CancellationToken cancellationToken)
    {
        var conta = await _repository
            .ObterPorNumeroParaLogin(request.NumeroContaLogada);

        if (conta is null)
            throw new AppException(ErrorMessages.InvalidAccount, ErrorTypes.InvalidAccount, HttpStatus.BadRequest);

        if (conta.SenhaHash != request.Senha)
            throw new UnauthorizedAccessException(ErrorMessages.UserUnauthorized);

        await _repository.InativarConta(
            Guid.Parse(conta.IdContaCorrente)
        );
    }
}