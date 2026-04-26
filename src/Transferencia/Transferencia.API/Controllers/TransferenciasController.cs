using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transferencia.Application.Transferencias.Commands.EfetuarTransferencia;

namespace Transferencia.API.Controllers;

[ApiController]
[Route("api/transferencias")]
public class TransferenciasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransferenciasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Transferir(EfetuarTransferenciaCommand command)
    {
        var authorization = Request.Headers.Authorization.ToString();

        var token = authorization.Replace("Bearer ", "");

        var numeroConta = User.FindFirst("numeroConta")?.Value;

        if (string.IsNullOrWhiteSpace(token) || numeroConta is null)
            return Forbid();

        command.Token = token;
        command.NumeroContaOrigem = int.Parse(numeroConta);

        await _mediator.Send(command);

        return NoContent();
    }
}