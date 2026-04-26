using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Transferencia.Infrastructure.Configurations;

namespace Transferencia.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public void Initialize()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var sql = @"
            CREATE TABLE IF NOT EXISTS transferencia (
                idtransferencia TEXT PRIMARY KEY,
                idrequisicao TEXT NOT NULL UNIQUE,
                numerocontadestino INTEGER NOT NULL,
                valor REAL NOT NULL,
                datatransferencia TEXT NOT NULL
            );
        ";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}