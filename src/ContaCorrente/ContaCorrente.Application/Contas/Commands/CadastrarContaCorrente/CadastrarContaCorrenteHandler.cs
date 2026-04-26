using MediatR;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Application.Contas.Repositories;

namespace ContaCorrente.Application.Contas.Commands.CadastrarContaCorrente;

public class CadastrarContaCorrenteHandler : IRequestHandler<CadastrarContaCorrenteCommand, int>
{
    private readonly IContaCorrenteRepository _repository;

    public CadastrarContaCorrenteHandler(IContaCorrenteRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(
        CadastrarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: usar hash real (SHA256 ou BCrypt)
        var salt = Guid.NewGuid().ToString();
        var senhaHash = request.Senha;

        var conta = new Domain.Entities.ContaCorrente(
            request.Nome,
            request.Cpf,
            senhaHash,
            salt
        );

        await _repository.Criar(conta);

        return conta.Numero;
    }
}