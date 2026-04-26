namespace ContaCorrente.Domain.Entities;

public class ContaCorrente
{
    public Guid IdContaCorrente { get; private set; }
    public int Numero { get; private set; }
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public bool Ativo { get; private set; }
    public string SenhaHash { get; private set; }
    public string Salt { get; private set; }

    public ContaCorrente(
        string nome,
        string cpf,
        string senhaHash,
        string salt)
    {
        IdContaCorrente = Guid.NewGuid();
        Numero = GerarNumeroConta();
        Nome = nome;
        Cpf = cpf;
        Ativo = true;
        SenhaHash = senhaHash;
        Salt = salt;
    }

    private static int GerarNumeroConta()
    {
        return Random.Shared.Next(100000, 999999);
    }

    public void Inativar()
    {
        Ativo = false;
    }
}