using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Transferencia.Application.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Transferencia.Application.ContaCorrente;
using Transferencia.Application.Common.Errors;
using Transferencia.Application.Common;

namespace Transferencia.Infrastructure.ContaCorrente;

public class ContaCorrenteClient : IContaCorrenteClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ContaCorrenteClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["ContaCorrenteApi:BaseUrl"]!;
    }

    public async Task Debitar(string token, string idRequisicao, decimal valor)
    {
        var request = new
        {
            idRequisicao,
            valor,
            tipo = "D"
        };

        await EnviarMovimento(token, request);
    }

    public async Task Creditar(
        string token,
        string idRequisicao,
        int numeroContaDestino,
        decimal valor)
    {
        var request = new
        {
            idRequisicao,
            numeroConta = numeroContaDestino,
            valor,
            tipo = "C"
        };

        await EnviarMovimento(token, request);
    }

    private async Task EnviarMovimento(string token, object body)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_baseUrl}/api/contas-correntes/movimentos"
        );

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(body);

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
            return;

        var errorBody = await response.Content.ReadAsStringAsync();

        var error = JsonSerializer.Deserialize<ApiErrorResponse>(
            errorBody,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        throw new AppException(error?.Message ?? ErrorMessages.TransferFailed, error?.Type ?? ErrorTypes.TransferOperationFailed, HttpStatus.BadRequest);
    }

    private sealed class ApiErrorResponse
    {
        public string Message { get; set; } = default!;
        public string Type { get; set; } = default!;
    }
}