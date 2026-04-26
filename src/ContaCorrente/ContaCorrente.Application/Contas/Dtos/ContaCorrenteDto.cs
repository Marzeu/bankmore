namespace ContaCorrente.Application.Contas.Dtos;

public class ContaCorrenteDto
{
    public string IdContaCorrente { get; set; } = default!;
    public int Numero { get; set; }
    public string Nome { get; set; } = default!;
    public string Cpf { get; set; } = default!;
    public bool Ativo { get; set; }
}