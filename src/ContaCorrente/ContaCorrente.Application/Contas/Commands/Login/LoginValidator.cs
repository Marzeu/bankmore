using ContaCorrente.Application.Common.Errors;
using FluentValidation;

namespace ContaCorrente.Application.Contas.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.DocumentoOuNumero)
            .NotEmpty()
            .WithErrorCode(ErrorTypes.InvalidRequest)
            .WithMessage(ErrorMessages.DocumentOrAccountRequired);

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithErrorCode(ErrorTypes.InvalidRequest)
            .WithMessage(ErrorMessages.PasswordRequired);
    }
}