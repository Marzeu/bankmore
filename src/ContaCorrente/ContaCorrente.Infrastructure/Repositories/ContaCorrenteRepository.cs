using ContaCorrente.Application.Contas.Dtos;
using ContaCorrente.Application.Contas.Repositories;
using ContaCorrente.Application.Saldos.Dtos;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Infrastructure.Configurations;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace ContaCorrente.Infrastructure.Repositories;

public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly string _connectionString;

    public ContaCorrenteRepository(IOptions<DatabaseSettings> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task Criar(Domain.Entities.ContaCorrente conta)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
            INSERT INTO contacorrente 
            (idcontacorrente, numero, nome, cpf, ativo, senha, salt)
            VALUES
            (@Id, @Numero, @Nome, @Cpf, @Ativo, @Senha, @Salt)
        ";

        var parameters = new
        {
            Id = conta.IdContaCorrente.ToString(),
            conta.Numero,
            conta.Nome,
            conta.Cpf,
            Ativo = conta.Ativo ? 1 : 0,
            Senha = conta.SenhaHash,
            conta.Salt
        };

        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<ContaCorrenteAuthDto?> ObterParaLogin(string documentoOuNumero)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
            SELECT 
                idcontacorrente AS IdContaCorrente,
                numero AS Numero,
                cpf AS Cpf,
                senha AS SenhaHash,
                salt AS Salt,
                ativo AS Ativo
            FROM contacorrente
            WHERE cpf = @Documento
               OR numero = @Numero;
        ";

        var numero = int.TryParse(documentoOuNumero, out var n) ? n : -1;

        return await connection.QueryFirstOrDefaultAsync<ContaCorrenteAuthDto>(
            sql,
            new
            {
                Documento = documentoOuNumero,
                Numero = numero
            }
        );
    }

    public async Task<ContaCorrenteDto?> ObterPorNumero(int numero)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        SELECT
            idcontacorrente AS IdContaCorrente,
            numero AS Numero,
            nome AS Nome,
            cpf AS Cpf,
            ativo AS Ativo,
            senha AS SenhaHash,
            salt AS Salt
        FROM contacorrente
        WHERE numero = @Numero;
    ";

        return await connection.QueryFirstOrDefaultAsync<ContaCorrenteDto>(
            sql,
            new { Numero = numero }
        );
    }

    public async Task InserirMovimento(
        Guid idContaCorrente,
        string tipoMovimento,
        decimal valor)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        INSERT INTO movimento
        (
            idmovimento,
            idcontacorrente,
            datamovimento,
            tipomovimento,
            valor
        )
        VALUES
        (
            @IdMovimento,
            @IdContaCorrente,
            @DataMovimento,
            @TipoMovimento,
            @Valor
        );
    ";

        var parameters = new
        {
            IdMovimento = Guid.NewGuid().ToString(),
            IdContaCorrente = idContaCorrente.ToString(),
            DataMovimento = DateTime.Now.ToString("dd/MM/yyyy"),
            TipoMovimento = tipoMovimento,
            Valor = valor
        };

        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task<bool> IdempotenciaExiste(string chaveIdempotencia)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        SELECT COUNT(1)
        FROM idempotencia
        WHERE chave_idempotencia = @Chave;
    ";

        var count = await connection.ExecuteScalarAsync<int>(
            sql,
            new { Chave = chaveIdempotencia }
        );

        return count > 0;
    }

    public async Task RegistrarIdempotencia(string chaveIdempotencia, string requisicao, string resultado)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        INSERT INTO idempotencia
        (
            chave_idempotencia,
            requisicao,
            resultado
        )
        VALUES
        (
            @Chave,
            @Requisicao,
            @Resultado
        );
    ";

        await connection.ExecuteAsync(sql, new
        {
            Chave = chaveIdempotencia,
            Requisicao = requisicao,
            Resultado = resultado
        });
    }

    public async Task<SaldoDto?> ConsultarSaldo(int numeroConta)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        SELECT
            c.numero AS NumeroConta,
            c.nome AS NomeTitular,
            COALESCE(SUM(
                CASE
                    WHEN m.tipomovimento = 'C' THEN m.valor
                    WHEN m.tipomovimento = 'D' THEN -m.valor
                    ELSE 0
                END
            ), 0) AS Saldo
        FROM contacorrente c
        LEFT JOIN movimento m 
            ON m.idcontacorrente = c.idcontacorrente
        WHERE c.numero = @NumeroConta
        GROUP BY c.numero, c.nome;
    ";

        var saldo = await connection.QueryFirstOrDefaultAsync<SaldoDto>(
            sql,
            new { NumeroConta = numeroConta }
        );

        if (saldo is not null)
            saldo.DataConsulta = DateTime.Now;

        return saldo;
    }

    public async Task<ContaCorrenteAuthDto?> ObterPorNumeroParaLogin(int numero)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        SELECT
            idcontacorrente AS IdContaCorrente,
            numero AS Numero,
            cpf AS Cpf,
            senha AS SenhaHash,
            salt AS Salt,
            ativo AS Ativo
        FROM contacorrente
        WHERE numero = @Numero;
    ";

        return await connection.QueryFirstOrDefaultAsync<ContaCorrenteAuthDto>(
            sql,
            new { Numero = numero }
        );
    }

    public async Task InativarConta(Guid idContaCorrente)
    {
        using var connection = new SqliteConnection(_connectionString);

        var sql = @"
        UPDATE contacorrente
        SET ativo = 0
        WHERE idcontacorrente = @Id;
    ";

        await connection.ExecuteAsync(sql, new
        {
            Id = idContaCorrente.ToString()
        });
    }
}