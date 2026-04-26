using ContaCorrente.Application.Common;
using ContaCorrente.Application.Common.Errors;
using ContaCorrente.Application.Common.Exceptions;
using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Application.Saldos.Dtos;
using MediatR;

namespace ContaCorrente.Application.Saldos.Queries.ConsultarSaldo;

public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, SaldoDto>
{
    private readonly IContaCorrenteRepository _repository;

    public ConsultarSaldoHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<SaldoDto> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
    {
        var conta = await _repository.ObterPorNumero(request.NumeroConta);

        conta = ValidarContaParaConsultaSaldo(conta);

        var saldo = await _repository.ConsultarSaldo(conta.Numero);

        return saldo ?? new SaldoDto
        {
            NumeroConta = conta.Numero,
            NomeTitular = conta.Nome,
            DataConsulta = DateTime.Now,
            Saldo = 0
        };
    }

    private static ContaCorrenteDto ValidarContaParaConsultaSaldo(ContaCorrenteDto? conta)
    {
        if (conta is null)
            throw new AppException(ErrorMessages.InvalidAccount, ErrorTypes.InvalidAccount, HttpStatus.BadRequest);

        if (!conta.Ativo)
            throw new AppException(ErrorMessages.InactiveAccount, ErrorTypes.InactiveAccount, HttpStatus.BadRequest);

        return conta;
    }
}