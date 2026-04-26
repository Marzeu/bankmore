namespace ContaCorrente.Application.Auth;

public interface ITokenService
{
    string Generate(Guid idContaCorrente, int numeroConta);
}