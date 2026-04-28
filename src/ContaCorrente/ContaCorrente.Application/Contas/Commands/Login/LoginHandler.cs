using ContaCorrente.Application.Auth;
using ContaCorrente.Application.Common.Errors;
using ContaCorrente.Application.Common.Security;
using ContaCorrente.Application.Contas.Repositories;
using MediatR;

namespace ContaCorrente.Application.Contas.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly ITokenService _tokenService;
    private readonly PasswordService _passwordService;

    public LoginHandler(
        IContaCorrenteRepository repository,
        ITokenService tokenService,
        PasswordService passwordService)
    {
        _repository = repository;
        _tokenService = tokenService;
        _passwordService = passwordService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var conta = await _repository.ObterParaLogin(request.DocumentoOuNumero);

        if (conta is null)
            throw new UnauthorizedAccessException(ErrorMessages.UserUnauthorized);

        var senhaValida = _passwordService.VerifyPassword(
            conta.SenhaHash,
            request.Senha
        );

        if (!senhaValida)
            throw new UnauthorizedAccessException(ErrorMessages.UserUnauthorized);

        return _tokenService.Generate(Guid.Parse(conta.IdContaCorrente), conta.Numero);
    }
}