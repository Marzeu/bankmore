using MediatR;
using System.Text.Json.Serialization;

namespace Transferencia.Application.Transferencias.Commands.EfetuarTransferencia;

public class EfetuarTransferenciaCommand : IRequest
{
    public string IdRequisicao { get; set; } = default!;
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }

    [JsonIgnore]
    public string? Token { get; set; } = default!;

    [JsonIgnore]
    public int NumeroContaOrigem { get; set; }
}