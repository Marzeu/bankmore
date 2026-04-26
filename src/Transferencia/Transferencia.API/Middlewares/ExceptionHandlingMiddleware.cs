using FluentValidation;
using System.Text.Json;
using Transferencia.Application.Common;
using Transferencia.Application.Common.Errors;
using Transferencia.Application.Common.Exceptions;

namespace Transferencia.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var error = MapException(ex);

            await WriteErrorAsync(
                context,
                error.StatusCode,
                error.Message,
                error.Type
            );
        }
    }

    private static ErrorResponse MapException(Exception ex)
    {
        return ex switch
        {
            ValidationException validationException =>
                FromValidationException(validationException),

            AppException appException =>
                new ErrorResponse(
                    appException.StatusCode,
                    appException.Message,
                    appException.Type
                ),

            UnauthorizedAccessException unauthorizedAccessException =>
                new ErrorResponse(
                    HttpStatus.Unauthorized,
                    ErrorMessages.UserUnauthorized,
                    ErrorTypes.UserUnauthorized
                ),

            _ =>
                new ErrorResponse(
                    HttpStatus.InternalServerError,
                    ErrorMessages.InternalServerError,
                    ErrorTypes.InternalServerError
                )
        };
    }

    private static ErrorResponse FromValidationException(ValidationException ex)
    {
        var firstError = ex.Errors.First();

        return new ErrorResponse(
            HttpStatus.BadRequest,
            firstError.ErrorMessage,
            firstError.ErrorCode
        );
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string message,
        string type)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            message,
            type
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private sealed record ErrorResponse(
        int StatusCode,
        string Message,
        string Type
    );
}