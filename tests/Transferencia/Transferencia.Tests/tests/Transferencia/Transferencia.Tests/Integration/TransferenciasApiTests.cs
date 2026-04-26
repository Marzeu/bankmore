using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Transferencia.Tests.Integration;

public class TransferenciasApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TransferenciasApiTests(WebApplicationFactory<Program> factory)
    {
        var dbPath = Path.Combine(
            Path.GetTempPath(),
            $"transferencia-test-{Guid.NewGuid()}.db"
        );

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["DatabaseSettings:ConnectionString"] = $"Data Source={dbPath}"
                });
            });
        });
    }

    [Fact]
    public async Task PostTransferencia_DeveRetornar403_QuandoTokenNaoForEnviado()
    {
        using var client = _factory.CreateClient();

        var request = new
        {
            idRequisicao = "REQ-INT-001",
            numeroContaDestino = 2002,
            valor = 100
        };

        var response = await client.PostAsJsonAsync("/api/transferencias", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PostTransferencia_DeveRetornar400_QuandoValorForInvalido()
    {
        using var client = CriarClientAutenticado();

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token-teste");

        var request = new
        {
            idRequisicao = "REQ-INT-002",
            numeroContaDestino = 2002,
            valor = 0
        };

        var response = await client.PostAsJsonAsync("/api/transferencias", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("INVALID_VALUE", content);
    }

    private HttpClient CriarClientAutenticado()
    {
        return _factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication("Test")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            "Test",
                            options => { }
                        );
                });
            })
            .CreateClient();
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("numeroConta", "1001")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}