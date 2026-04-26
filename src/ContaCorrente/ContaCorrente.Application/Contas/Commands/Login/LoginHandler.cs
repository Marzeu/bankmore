using ContaCorrente.Application.Auth;
using ContaCorrente.Application.Contas.Repositories;
using MediatR;

namespace ContaCorrente.Application.Contas.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly ITokenService _tokenService;

    public LoginHandler(IContaCorrenteRepository repository, ITokenService tokenService)
    {
        _repository = repository;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var conta = await _repository.ObterParaLogin(request.DocumentoOuNumero);

        if (conta is null)
            throw new UnauthorizedAccessException("Usuário não autorizado.");

        if (conta.SenhaHash != request.Senha)
            throw new UnauthorizedAccessException("Usuário não autorizado.");

        return _tokenService.Generate(Guid.Parse(conta.IdContaCorrente), conta.Numero);
    }
}