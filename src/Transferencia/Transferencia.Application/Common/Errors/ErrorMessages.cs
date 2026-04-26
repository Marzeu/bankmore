namespace Transferencia.Application.Common.Errors;

public static class ErrorMessages
{
    public const string SameAccount = "Conta de origem e destino não podem ser iguais.";
    public const string TransferFailed = "Falha ao processar movimento na conta corrente.";
    public const string TransferOperationFailed = "Falha ao efetuar transferência.";
    public const string InternalServerError = "Erro interno no servidor";
    public const string UserUnauthorized = "Usuário não autorizado.";
}