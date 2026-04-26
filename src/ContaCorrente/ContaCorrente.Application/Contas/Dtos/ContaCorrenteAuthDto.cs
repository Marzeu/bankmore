namespace ContaCorrente.Application.Contas.Dtos;

public class ContaCorrenteAuthDto
{
    public string IdContaCorrente { get; set; } = default!;
    public int Numero { get; set; }
    public string Cpf { get; set; } = default!;
    public string SenhaHash { get; set; } = default!;
    public string Salt { get; set; } = default!;
    public bool Ativo { get; set; }
}