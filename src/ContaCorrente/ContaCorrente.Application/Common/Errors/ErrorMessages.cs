namespace ContaCorrente.Application.Common.Errors;

public static class ErrorMessages
{
    public const string InvalidAccount = "Conta corrente não encontrada.";
    public const string InactiveAccount = "Conta corrente inativa.";
    public const string InvalidValue = "O valor da movimentação deve ser positivo.";
    public const string InvalidType = "Tipo de movimento inválido.";
    public const string OnlyCreditAllowedForDifferentAccount = "Só é permitido crédito em conta diferente da conta logada.";
    public const string UserUnauthorized = "Usuário não autorizado.";
    public const string InternalServerError = "Erro interno no servidor";

    public const string TokenInvalidOrExpired = "Token inválido ou expirado.";    
    public const string InvalidCredentials = "Usuário ou senha inválidos.";

    public const string DocumentOrAccountRequired = "Documento ou número da conta é obrigatório.";    
    public const string PasswordRequired = "Senha é obrigatória.";  
}