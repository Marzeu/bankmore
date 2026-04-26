using ContaCorrente.Application.Common;
using ContaCorrente.Application.Common.Errors;
using ContaCorrente.Application.Common.Exceptions;
using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Domain.Entities;
using MediatR;

namespace ContaCorrente.Application.Movimentos.Commands.MovimentarConta;

public class MovimentarContaHandler : IRequestHandler<MovimentarContaCommand>
{
    private readonly IContaCorrenteRepository _repository;

    public MovimentarContaHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(MovimentarContaCommand request, CancellationToken cancellationToken)
    {
        var jaProcessada = await _repository.IdempotenciaExiste(request.IdRequisicao);

        if (jaProcessada)
            return;

        var numeroContaMovimentada = request.NumeroConta ?? request.NumeroContaLogada;

        var conta = await _repository.ObterPorNumero(numeroContaMovimentada);

        conta = ValidarMovimentacao(conta, request, numeroContaMovimentada);        

        await _repository.InserirMovimento(Guid.Parse(conta.IdContaCorrente), request.Tipo, request.Valor);

        await _repository.RegistrarIdempotencia(request.IdRequisicao, $"Movimento {request.Tipo} - Valor {request.Valor}", "Movimento processado com sucesso");
    }

    private static ContaCorrenteDto ValidarMovimentacao(ContaCorrenteDto? conta, MovimentarContaCommand request, int numeroContaMovimentada)
    {
        if (conta is null)
            throw new AppException(ErrorMessages.InvalidAccount, ErrorTypes.InvalidAccount, HttpStatus.BadRequest);

        if (!conta.Ativo)
            throw new AppException(ErrorMessages.InactiveAccount, ErrorTypes.InactiveAccount, HttpStatus.BadRequest);        

        var movimentandoContaDeOutroUsuario = numeroContaMovimentada != request.NumeroContaLogada;

        if (movimentandoContaDeOutroUsuario && request.Tipo == "D")
            throw new AppException(ErrorMessages.OnlyCreditAllowedForDifferentAccount, ErrorTypes.InvalidType, HttpStatus.BadRequest);

        return conta;
    }
}