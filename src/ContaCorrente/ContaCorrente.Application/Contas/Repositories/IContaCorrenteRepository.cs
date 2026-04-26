using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Saldos.Dtos;
using ContaCorrente.Domain.Entities;


namespace ContaCorrente.Application.Contas.Repositories;

public interface IContaCorrenteRepository
{
    Task Criar(Domain.Entities.ContaCorrente conta);
    Task<ContaCorrenteAuthDto?> ObterParaLogin(string documentoOuNumero);
    Task<ContaCorrenteDto?> ObterPorNumero(int numero);
    Task InserirMovimento(Guid idContaCorrente, string tipoMovimento, decimal valor);
    Task<bool> IdempotenciaExiste(string chaveIdempotencia);
    Task RegistrarIdempotencia(string chaveIdempotencia, string requisicao, string resultado);
    Task<SaldoDto?> ConsultarSaldo(int numeroConta);
    Task<ContaCorrenteAuthDto?> ObterPorNumeroParaLogin(int numero);
    Task InativarConta(Guid idContaCorrente);
}