using ContaCorrente.Application.Contas.Commands.CadastrarContaCorrente;
using ContaCorrente.Application.Movimentos.Commands.MovimentarConta;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContaCorrente.Application.Saldos.Queries.ConsultarSaldo;
using ContaCorrente.Application.Contas.Commands.InativarConta;

namespace ContaCorrente.API.Controllers;

[ApiController]
[Route("api/contas-correntes")]
public class ContasCorrentesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContasCorrentesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Cadastrar(
        CadastrarContaCorrenteCommand command)
    {
        var numeroConta = await _mediator.Send(command);

        return Ok(new
        {
            numero = numeroConta
        });
    }

    [Authorize]
    [HttpPost("movimentos")]
    public async Task<IActionResult> Movimentar(MovimentarContaCommand command)
    {
        var numeroContaToken = User.FindFirst("numeroConta")?.Value;

        if (numeroContaToken is null)
            return Forbid();

        command.NumeroContaLogada = int.Parse(numeroContaToken);

        if (command.NumeroConta is null)
            command.NumeroConta = command.NumeroContaLogada;

        await _mediator.Send(command);

        return NoContent();
    }

    [Authorize]
    [HttpGet("saldo")]
    public async Task<IActionResult> ConsultarSaldo()
    {
        var numeroContaToken = User.FindFirst("numeroConta")?.Value;

        if (numeroContaToken is null)
            return Forbid();

        var saldo = await _mediator.Send(new ConsultarSaldoQuery
        {
            NumeroConta = int.Parse(numeroContaToken)
        });

        return Ok(saldo);
    }

    [Authorize]
    [HttpPatch("inativar")]
    public async Task<IActionResult> InativarConta(InativarContaCommand command)
    {
        var numeroConta = User.FindFirst("numeroConta")?.Value;

        if (numeroConta is null)
            return Forbid();

        command.NumeroContaLogada = int.Parse(numeroConta);

        await _mediator.Send(command);

        return NoContent();
    }
}