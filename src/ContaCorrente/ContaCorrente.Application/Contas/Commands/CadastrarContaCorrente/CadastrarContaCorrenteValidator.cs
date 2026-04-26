using FluentValidation;
using ContaCorrente.Domain.Services;

namespace ContaCorrente.Application.Contas.Commands.CadastrarContaCorrente;

public class CadastrarContaCorrenteValidator : AbstractValidator<CadastrarContaCorrenteCommand>
{
    public CadastrarContaCorrenteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(3);

        RuleFor(x => x.Cpf)
            .Must(CpfValidator.IsValid)
            .WithMessage("CPF inválido.")
            .WithErrorCode("INVALID_DOCUMENT");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .MinimumLength(6);
    }
}