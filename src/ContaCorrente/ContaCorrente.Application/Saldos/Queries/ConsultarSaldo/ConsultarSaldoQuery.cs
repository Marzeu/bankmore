using ContaCorrente.Application.Saldos.Dtos;
using MediatR;

namespace ContaCorrente.Application.Saldos.Queries.ConsultarSaldo;

public class ConsultarSaldoQuery : IRequest<SaldoDto>
{
    public int NumeroConta { get; set; }
}