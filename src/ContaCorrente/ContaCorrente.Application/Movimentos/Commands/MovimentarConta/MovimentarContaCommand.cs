using MediatR;

namespace ContaCorrente.Application.Movimentos.Commands.MovimentarConta;

public class MovimentarContaCommand : IRequest
{
    public string IdRequisicao { get; set; } = default!;
    public int? NumeroConta { get; set; }
    public int NumeroContaLogada { get; set; }
    public decimal Valor { get; set; }
    public string Tipo { get; set; } = default!; // C ou D
}