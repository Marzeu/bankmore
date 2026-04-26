namespace ContaCorrente.Application.Common.Exceptions;

public class AppException : Exception
{
    public string Type { get; }
    public int StatusCode { get; }

    public AppException(string message, string type, int statusCode) : base(message)
    {
        Type = type;
        StatusCode = statusCode;
    }
}