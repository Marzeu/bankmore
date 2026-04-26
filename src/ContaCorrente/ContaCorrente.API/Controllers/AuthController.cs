using ContaCorrente.Application.Contas.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContaCorrente.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command);

        return Ok(new
        {
            accessToken = token,
            tokenType = "Bearer"
        });
    }
}