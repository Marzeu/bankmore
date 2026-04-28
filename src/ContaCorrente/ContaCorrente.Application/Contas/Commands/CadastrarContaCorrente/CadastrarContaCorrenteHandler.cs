using ContaCorrente.Application.Common.Security;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Domain.Entities;
using MediatR;

namespace ContaCorrente.Application.Contas.Commands.CadastrarContaCorrente;

public class CadastrarContaCorrenteHandler : IRequestHandler<CadastrarContaCorrenteCommand, int>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly PasswordService _passwordService;

    public CadastrarContaCorrenteHandler(IContaCorrenteRepository repository, PasswordService passwordService)
    {
        _repository = repository;
        _passwordService = passwordService;
    }

    public async Task<int> Handle(
        CadastrarContaCorrenteCommand request,
        CancellationToken cancellationToken)
    {        
        var salt = Guid.NewGuid().ToString();
        var senhaHash = _passwordService.HashPassword(request.Senha);

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