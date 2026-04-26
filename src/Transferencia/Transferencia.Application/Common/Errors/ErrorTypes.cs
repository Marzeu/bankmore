namespace Transferencia.Application.Common.Errors;

public static class ErrorTypes
{
    public const string InvalidAccount = "INVALID_ACCOUNT";
    public const string InactiveAccount = "INACTIVE_ACCOUNT";
    public const string InvalidValue = "INVALID_VALUE";
    public const string InvalidRequest = "INVALID_REQUEST";

    public const string UserUnauthorized = "USER_UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string TransferOperationFailed = "TRANSFER_OPERATION_FAILED";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
}