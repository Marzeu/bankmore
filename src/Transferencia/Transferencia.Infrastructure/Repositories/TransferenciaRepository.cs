using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Transferencia.Application.Transferencias.Repositories;
using Transferencia.Infrastructure.Configurations;

namespace Transferencia.Infrastructure.Repositories;

public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly string _connectionString;

    public TransferenciaRepository(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task Registrar(string idRequisicao, int numeroContaDestino, decimal valor)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
            INSERT INTO transferencia
            (
                idtransferencia,
                idrequisicao,
                numerocontadestino,
                valor,
                datatransferencia
            )
            VALUES
            (
                @IdTransferencia,
                @IdRequisicao,
                @NumeroContaDestino,
                @Valor,
                @DataTransferencia
            );
        ";

        await connection.ExecuteAsync(sql, new
        {
            IdTransferencia = Guid.NewGuid().ToString(),
            IdRequisicao = idRequisicao,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            DataTransferencia = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
        });
    }

    public async Task<bool> IdempotenciaExiste(string idRequisicao)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        SELECT COUNT(1)
        FROM transferencia
        WHERE idrequisicao = @IdRequisicao;
    ";

        var count = await connection.ExecuteScalarAsync<int>(
            sql,
            new { IdRequisicao = idRequisicao }
        );

        return count > 0;
    }
}