using ContaCorrente.Application.Common;
using ContaCorrente.Application.Common.Errors;
using ContaCorrente.Application.Common.Exceptions;
using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Application.Movimentos.Commands.MovimentarConta;
using Moq;

namespace ContaCorrente.Tests.Application.Movimentos;

public class MovimentarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly MovimentarContaHandler _handler;

    public MovimentarContaHandlerTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _handler = new MovimentarContaHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveInserirMovimentoERegistrarIdempotencia_QuandoMovimentacaoForValida()
    {
        var command = CriarCommand();

        var conta = CriarConta(ativo: true);

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(command.NumeroConta!.Value))
            .ReturnsAsync(conta);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.InserirMovimento(
            Guid.Parse(conta.IdContaCorrente),
            command.Tipo,
            command.Valor
        ), Times.Once);

        _repositoryMock.Verify(x => x.RegistrarIdempotencia(
            command.IdRequisicao,
            $"Movimento {command.Tipo} - Valor {command.Valor}",
            "Movimento processado com sucesso"
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoDeveProcessar_QuandoRequisicaoJaFoiProcessada()
    {
        var command = CriarCommand();

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(x => x.InserirMovimento(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<decimal>()
        ), Times.Never);

        _repositoryMock.Verify(x => x.RegistrarIdempotencia(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarInvalidAccount_QuandoContaNaoExistir()
    {
        var command = CriarCommand();

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(command.NumeroConta!.Value))
            .ReturnsAsync((ContaCorrenteDto?)null);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal(ErrorTypes.InvalidAccount, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Handle_DeveRetornarInactiveAccount_QuandoContaEstiverInativa()
    {
        var command = CriarCommand();

        var conta = CriarConta(ativo: false);

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(command.NumeroConta!.Value))
            .ReturnsAsync(conta);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal(ErrorTypes.InactiveAccount, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Handle_DeveRetornarInvalidType_QuandoDebitoEmContaDeOutroUsuario()
    {
        var command = CriarCommand(
            numeroConta: 2002,
            numeroContaLogada: 1001,
            tipo: "D"
        );

        var conta = CriarConta(numero: 2002, ativo: true);

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(It.IsAny<int>()))
            .ReturnsAsync(conta);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal(ErrorTypes.InvalidType, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);
    }

    private static MovimentarContaCommand CriarCommand(
        int numeroConta = 1001,
        int numeroContaLogada = 1001,
        string tipo = "C",
        decimal valor = 150)
    {
        return new MovimentarContaCommand
        {
            IdRequisicao = "REQ-001",
            NumeroConta = numeroConta,
            NumeroContaLogada = numeroContaLogada,
            Tipo = tipo,
            Valor = valor
        };
    }

    private static ContaCorrenteDto CriarConta(
        int numero = 1001,
        bool ativo = true)
    {
        return new ContaCorrenteDto
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            Numero = numero,
            Nome = "Cliente Teste",
            Cpf = "12345678909",
            Ativo = ativo
        };
    }
}