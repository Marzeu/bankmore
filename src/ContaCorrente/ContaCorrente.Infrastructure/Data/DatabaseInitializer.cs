using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using ContaCorrente.Infrastructure.Configurations;

namespace ContaCorrente.Infrastructure.Data;

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
        CREATE TABLE IF NOT EXISTS contacorrente (
            idcontacorrente TEXT PRIMARY KEY,
            numero INTEGER NOT NULL UNIQUE,
            nome TEXT NOT NULL,
            cpf TEXT NOT NULL UNIQUE,
            ativo INTEGER NOT NULL DEFAULT 1,
            senha TEXT NOT NULL,
            salt TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS movimento (
            idmovimento TEXT PRIMARY KEY,
            idcontacorrente TEXT NOT NULL,
            datamovimento TEXT NOT NULL,
            tipomovimento TEXT NOT NULL,
            valor REAL NOT NULL,
            CHECK (tipomovimento IN ('C','D')),
            FOREIGN KEY(idcontacorrente) REFERENCES contacorrente(idcontacorrente)
        );

        CREATE TABLE IF NOT EXISTS idempotencia (
            chave_idempotencia TEXT PRIMARY KEY,
            requisicao TEXT,
            resultado TEXT
        );
        ";

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}