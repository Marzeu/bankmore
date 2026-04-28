using ContaCorrente.Application.Auth;
using ContaCorrente.Application.Common.Security;
using ContaCorrente.Application.Contas.Commands.Login;
using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using Moq;

namespace ContaCorrente.Tests.Application.Contas;

public class LoginHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly PasswordService _passwordService;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordService = new PasswordService();

        _handler = new LoginHandler(
            _repositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordService
        );
    }

    [Fact]
    public async Task Handle_DeveRetornarToken_QuandoSenhaForValida()
    {
        var senha = "123456";
        var senhaHash = _passwordService.HashPassword(senha);

        var conta = new ContaCorrenteAuthDto
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            Numero = 1001,
            SenhaHash = senhaHash
        };

        _repositoryMock
            .Setup(x => x.ObterParaLogin("12345678909"))
            .ReturnsAsync(conta);

        _tokenServiceMock
            .Setup(x => x.Generate(Guid.Parse(conta.IdContaCorrente), conta.Numero))
            .Returns("token-gerado");

        var token = await _handler.Handle(
            new LoginCommand
            {
                DocumentoOuNumero = "12345678909",
                Senha = senha
            },
            CancellationToken.None
        );

        Assert.Equal("token-gerado", token);
    }

    [Fact]
    public async Task Handle_DeveLancarUnauthorized_QuandoSenhaForInvalida()
    {
        var senhaHash = _passwordService.HashPassword("senha-correta");

        var conta = new ContaCorrenteAuthDto
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            Numero = 1001,
            SenhaHash = senhaHash
        };

        _repositoryMock
            .Setup(x => x.ObterParaLogin("12345678909"))
            .ReturnsAsync(conta);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(
                new LoginCommand
                {
                    DocumentoOuNumero = "12345678909",
                    Senha = "senha-errada"
                },
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task Handle_DeveLancarUnauthorized_QuandoContaNaoExistir()
    {
        _repositoryMock
            .Setup(x => x.ObterParaLogin("12345678909"))
            .ReturnsAsync((ContaCorrenteAuthDto?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _handler.Handle(
                new LoginCommand
                {
                    DocumentoOuNumero = "12345678909",
                    Senha = "123456"
                },
                CancellationToken.None
            )
        );
    }
}