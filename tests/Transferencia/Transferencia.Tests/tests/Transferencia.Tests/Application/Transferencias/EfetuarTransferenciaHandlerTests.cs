using Moq;
using Transferencia.Application.ContaCorrente;
using Transferencia.Application.Transferencias.Commands.EfetuarTransferencia;
using Transferencia.Application.Transferencias.Repositories;
using Transferencia.Application.Common;
using Transferencia.Application.Common.Errors;
using Transferencia.Application.Common.Exceptions;

namespace Transferencia.Tests.Application.Transferencias;

public class EfetuarTransferenciaHandlerTests
{
    private readonly Mock<IContaCorrenteClient> _contaCorrenteClientMock;
    private readonly Mock<ITransferenciaRepository> _repositoryMock;
    private readonly EfetuarTransferenciaHandler _handler;

    public EfetuarTransferenciaHandlerTests()
    {
        _contaCorrenteClientMock = new Mock<IContaCorrenteClient>();
        _repositoryMock = new Mock<ITransferenciaRepository>();

        _handler = new EfetuarTransferenciaHandler(
            _contaCorrenteClientMock.Object,
            _repositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_DeveDebitarCreditarERegistrarTransferencia_QuandoTransferenciaForValida()
    {
        // Arrange
        var command = new EfetuarTransferenciaCommand
        {
            IdRequisicao = "REQ-001",
            Token = "token-teste",
            NumeroContaOrigem = 1001,
            NumeroContaDestino = 2002,
            Valor = 150
        };

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contaCorrenteClientMock.Verify(x => x.Debitar(
            command.Token,
            $"{command.IdRequisicao}-DEBITO",
            command.Valor
        ), Times.Once);

        _contaCorrenteClientMock.Verify(x => x.Creditar(
            command.Token,
            $"{command.IdRequisicao}-CREDITO",
            command.NumeroContaDestino,
            command.Valor
        ), Times.Once);

        _repositoryMock.Verify(x => x.Registrar(
            command.IdRequisicao,
            command.NumeroContaDestino,
            command.Valor
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_NaoDeveProcessarTransferencia_QuandoRequisicaoJaFoiProcessada()
    {
        // Arrange
        var command = new EfetuarTransferenciaCommand
        {
            IdRequisicao = "REQ-001",
            Token = "token-teste",
            NumeroContaOrigem = 1001,
            NumeroContaDestino = 2002,
            Valor = 150
        };

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contaCorrenteClientMock.Verify(x => x.Debitar(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<decimal>()
        ), Times.Never);

        _contaCorrenteClientMock.Verify(x => x.Creditar(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);

        _repositoryMock.Verify(x => x.Registrar(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveFazerEstorno_QuandoCreditoFalhar()
    {
        // Arrange
        var command = new EfetuarTransferenciaCommand
        {
            IdRequisicao = "REQ-001",
            Token = "token-teste",
            NumeroContaOrigem = 1001,
            NumeroContaDestino = 2002,
            Valor = 150
        };

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        // Crédito vai falhar
        _contaCorrenteClientMock
            .Setup(x => x.Creditar(
                command.Token,
                $"{command.IdRequisicao}-CREDITO",
                command.NumeroContaDestino,
                command.Valor
            ))
            .ThrowsAsync(new Exception("Erro no crédito"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AppException>(() =>
                _handler.Handle(command, CancellationToken.None));

        Assert.Equal(ErrorTypes.TransferOperationFailed, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);

        // Verifica se o débito foi feito
        _contaCorrenteClientMock.Verify(x => x.Debitar(
            command.Token,
            $"{command.IdRequisicao}-DEBITO",
            command.Valor
        ), Times.Once);

        // Verifica se tentou creditar
        _contaCorrenteClientMock.Verify(x => x.Creditar(
            command.Token,
            $"{command.IdRequisicao}-CREDITO",
            command.NumeroContaDestino,
            command.Valor
        ), Times.Once);

        // Verifica se fez estorno (crédito na conta origem)
        _contaCorrenteClientMock.Verify(x => x.Creditar(
            command.Token,
            $"{command.IdRequisicao}-ESTORNO",
            command.NumeroContaOrigem,
            command.Valor
        ), Times.Once);

        // Não deve registrar transferência
        _repositoryMock.Verify(x => x.Registrar(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarInvalidAccount_QuandoContaOrigemForIgualDestino()
    {
        // Arrange
        var command = new EfetuarTransferenciaCommand
        {
            IdRequisicao = "REQ-001",
            Token = "token-teste",
            NumeroContaOrigem = 1001,
            NumeroContaDestino = 1001, // mesma conta
            Valor = 150
        };

        // Act
        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        // Assert
        Assert.Equal(ErrorTypes.InvalidAccount, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);

        // Não deve chamar nada externo
        _contaCorrenteClientMock.Verify(x => x.Debitar(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<decimal>()
        ), Times.Never);

        _contaCorrenteClientMock.Verify(x => x.Creditar(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);

        _repositoryMock.Verify(x => x.Registrar(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);
    }

    [Fact]
    public async Task Handle_DevePropagarErroDaContaCorrente_QuandoCreditoFalharComAppException()
    {
        // Arrange
        var command = new EfetuarTransferenciaCommand
        {
            IdRequisicao = "REQ-001",
            Token = "token-teste",
            NumeroContaOrigem = 1001,
            NumeroContaDestino = 2002,
            Valor = 150
        };

        _repositoryMock
            .Setup(x => x.IdempotenciaExiste(command.IdRequisicao))
            .ReturnsAsync(false);

        var erroContaCorrente = new AppException(
            "Conta destino inválida.",
            ErrorTypes.InvalidAccount,
            HttpStatus.BadRequest
        );

        _contaCorrenteClientMock
            .Setup(x => x.Creditar(
                command.Token,
                $"{command.IdRequisicao}-CREDITO",
                command.NumeroContaDestino,
                command.Valor
            ))
            .ThrowsAsync(erroContaCorrente);

        // Act
        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(command, CancellationToken.None)
        );

        // Assert — erro deve ser o mesmo
        Assert.Equal(erroContaCorrente.Type, exception.Type);
        Assert.Equal(erroContaCorrente.StatusCode, exception.StatusCode);
        Assert.Equal(erroContaCorrente.Message, exception.Message);

        // Deve ter feito estorno
        _contaCorrenteClientMock.Verify(x => x.Creditar(
            command.Token,
            $"{command.IdRequisicao}-ESTORNO",
            command.NumeroContaOrigem,
            command.Valor
        ), Times.Once);

        // Não deve registrar transferência
        _repositoryMock.Verify(x => x.Registrar(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()
        ), Times.Never);
    }
}