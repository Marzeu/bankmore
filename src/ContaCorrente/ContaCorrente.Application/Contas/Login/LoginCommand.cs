using MediatR;

namespace ContaCorrente.Application.Contas.Commands.Login;

public class LoginCommand : IRequest<string>
{
    public string DocumentoOuNumero { get; set; } = default!;
    public string Senha { get; set; } = default!;
}