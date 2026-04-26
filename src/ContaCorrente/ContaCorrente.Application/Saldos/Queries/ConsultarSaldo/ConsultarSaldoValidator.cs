using FluentValidation;

namespace ContaCorrente.Application.Saldos.Queries.ConsultarSaldo;

public class ConsultarSaldoValidator : AbstractValidator<ConsultarSaldoQuery>
{
    public ConsultarSaldoValidator()
    {
        RuleFor(x => x.NumeroConta)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ACCOUNT")
            .WithMessage("Número da conta corrente inválido.");
    }
}