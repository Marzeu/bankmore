namespace ContaCorrente.Application.Saldos.Dtos;

public class SaldoDto
{
    public int NumeroConta { get; set; }
    public string NomeTitular { get; set; } = default!;
    public DateTime DataConsulta { get; set; }
    public decimal Saldo { get; set; }
}