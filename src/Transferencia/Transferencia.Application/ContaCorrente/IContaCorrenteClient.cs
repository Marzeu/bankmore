namespace Transferencia.Application.ContaCorrente;

public interface IContaCorrenteClient
{
    Task Debitar(string token, string idRequisicao, decimal valor);
    Task Creditar(string token, string idRequisicao, int numeroContaDestino, decimal valor);
}