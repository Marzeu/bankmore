using MediatR;

namespace ContaCorrente.Application.Contas.Commands.CadastrarContaCorrente;

public class CadastrarContaCorrenteCommand : IRequest<int>
{
    public string Nome { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public string Senha { get; set; } = default!;
}