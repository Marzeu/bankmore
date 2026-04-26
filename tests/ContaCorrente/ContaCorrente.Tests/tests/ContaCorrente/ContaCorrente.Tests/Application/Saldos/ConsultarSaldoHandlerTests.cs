using ContaCorrente.Application.Common;
using ContaCorrente.Application.Common.Errors;
using ContaCorrente.Application.Common.Exceptions;
using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Application.Saldos.Dtos;
using ContaCorrente.Application.Saldos.Queries.ConsultarSaldo;
using Moq;

namespace ContaCorrente.Tests.Application.Saldos;

public class ConsultarSaldoHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _repositoryMock;
    private readonly ConsultarSaldoHandler _handler;

    public ConsultarSaldoHandlerTests()
    {
        _repositoryMock = new Mock<IContaCorrenteRepository>();
        _handler = new ConsultarSaldoHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSaldo_QuandoContaForValida()
    {
        var query = new ConsultarSaldoQuery
        {
            NumeroConta = 1001
        };

        var conta = CriarConta(ativo: true);

        var saldoEsperado = new SaldoDto
        {
            NumeroConta = 1001,
            NomeTitular = "Cliente Teste",
            DataConsulta = DateTime.Now,
            Saldo = 250
        };

        _repositoryMock
            .Setup(x => x.ObterPorNumero(query.NumeroConta))
            .ReturnsAsync(conta);

        _repositoryMock
            .Setup(x => x.ConsultarSaldo(query.NumeroConta))
            .ReturnsAsync(saldoEsperado);

        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(saldoEsperado.NumeroConta, resultado.NumeroConta);
        Assert.Equal(saldoEsperado.NomeTitular, resultado.NomeTitular);
        Assert.Equal(saldoEsperado.Saldo, resultado.Saldo);
    }

    [Fact]
    public async Task Handle_DeveRetornarSaldoZero_QuandoContaNaoTiverMovimentacoes()
    {
        var query = new ConsultarSaldoQuery
        {
            NumeroConta = 1001
        };

        var conta = CriarConta(ativo: true);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(query.NumeroConta))
            .ReturnsAsync(conta);

        _repositoryMock
            .Setup(x => x.ConsultarSaldo(query.NumeroConta))
            .ReturnsAsync((SaldoDto?)null);

        var resultado = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(conta.Numero, resultado.NumeroConta);
        Assert.Equal(conta.Nome, resultado.NomeTitular);
        Assert.Equal(0m, resultado.Saldo);
    }

    [Fact]
    public async Task Handle_DeveRetornarInvalidAccount_QuandoContaNaoExistir()
    {
        var query = new ConsultarSaldoQuery
        {
            NumeroConta = 1001
        };

        _repositoryMock
            .Setup(x => x.ObterPorNumero(query.NumeroConta))
            .ReturnsAsync((ContaCorrenteDto?)null);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal(ErrorTypes.InvalidAccount, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task Handle_DeveRetornarInactiveAccount_QuandoContaEstiverInativa()
    {
        var query = new ConsultarSaldoQuery
        {
            NumeroConta = 1001
        };

        var conta = CriarConta(ativo: false);

        _repositoryMock
            .Setup(x => x.ObterPorNumero(query.NumeroConta))
            .ReturnsAsync(conta);

        var exception = await Assert.ThrowsAsync<AppException>(() =>
            _handler.Handle(query, CancellationToken.None)
        );

        Assert.Equal(ErrorTypes.InactiveAccount, exception.Type);
        Assert.Equal(HttpStatus.BadRequest, exception.StatusCode);
    }

    private static ContaCorrenteDto CriarConta(bool ativo)
    {
        return new ContaCorrenteDto
        {
            IdContaCorrente = Guid.NewGuid().ToString(),
            Numero = 1001,
            Nome = "Cliente Teste",
            Cpf = "12345678909",
            Ativo = ativo
        };
    }
}