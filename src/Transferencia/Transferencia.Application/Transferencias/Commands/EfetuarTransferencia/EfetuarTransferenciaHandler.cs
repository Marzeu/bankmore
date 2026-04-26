using Transferencia.Application.Common.Exceptions;
using MediatR;
using Transferencia.Application.ContaCorrente;
using Transferencia.Application.Transferencias.Repositories;
using Transferencia.Application.Common.Errors;
using Transferencia.Application.Common;

namespace Transferencia.Application.Transferencias.Commands.EfetuarTransferencia;

public class EfetuarTransferenciaHandler : IRequestHandler<EfetuarTransferenciaCommand>
{
    private readonly IContaCorrenteClient _contaCorrenteClient;
    private readonly ITransferenciaRepository _repository;

    public EfetuarTransferenciaHandler(
        IContaCorrenteClient contaCorrenteClient,
        ITransferenciaRepository repository)
    {
        _contaCorrenteClient = contaCorrenteClient;
        _repository = repository;
    }

    public async Task Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
    {
        if (request.NumeroContaDestino == request.NumeroContaOrigem)
            throw new AppException(ErrorMessages.SameAccount, ErrorTypes.InvalidAccount, HttpStatus.BadRequest);

        var jaProcessada = await _repository.IdempotenciaExiste(request.IdRequisicao);

        if (jaProcessada)
            return;

        await _contaCorrenteClient.Debitar(
            request.Token!,
            $"{request.IdRequisicao}-DEBITO",
            request.Valor
        );

        try
        {
            await _contaCorrenteClient.Creditar(
                request.Token!,
                $"{request.IdRequisicao}-CREDITO",
                request.NumeroContaDestino,
                request.Valor
            );
        }
        catch (AppException)
        {
            await _contaCorrenteClient.Creditar(
                request.Token!,
                $"{request.IdRequisicao}-ESTORNO",
                request.NumeroContaOrigem,
                request.Valor
            );

            throw;
        }
        catch
        {
            await _contaCorrenteClient.Creditar(
                request.Token!,
                $"{request.IdRequisicao}-ESTORNO",
                request.NumeroContaOrigem,
                request.Valor
            );

            throw new AppException(ErrorMessages.TransferOperationFailed, ErrorTypes.TransferOperationFailed, HttpStatus.BadRequest);
        }

        await _repository.Registrar(
            request.IdRequisicao,
            request.NumeroContaDestino,
            request.Valor
        );
    }
}