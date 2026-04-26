namespace Transferencia.Application.Transferencias.Repositories;

public interface ITransferenciaRepository
{
    Task Registrar(string idRequisicao, int numeroContaDestino, decimal valor);
    Task<bool> IdempotenciaExiste(string idRequisicao);
}