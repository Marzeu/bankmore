using FluentValidation;

namespace ContaCorrente.Application.Movimentos.Commands.MovimentarConta;

public class MovimentarContaValidator : AbstractValidator<MovimentarContaCommand>
{
    public MovimentarContaValidator()
    {
        RuleFor(x => x.IdRequisicao)
            .NotEmpty()
            .WithErrorCode("INVALID_REQUEST")
            .WithMessage("Identificação da requisição é obrigatória.");

        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithErrorCode("INVALID_VALUE")
            .WithMessage("Valor deve ser positivo.");

        RuleFor(x => x.Tipo)
            .Must(x => x == "C" || x == "D")
            .WithErrorCode("INVALID_TYPE")
            .WithMessage("Tipo deve ser C ou D.");
    }
}