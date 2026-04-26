using MediatR;

namespace ContaCorrente.Application.Contas.Commands.InativarConta;

public class InativarContaCommand : IRequest
{
    public string Senha { get; set; } = default!;
    public int NumeroContaLogada { get; set; }
}