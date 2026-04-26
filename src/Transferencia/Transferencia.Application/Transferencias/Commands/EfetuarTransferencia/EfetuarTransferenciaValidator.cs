using FluentValidation;

namespace Transferencia.Application.Transferencias.Commands.EfetuarTransferencia;

public class EfetuarTransferenciaValidator : AbstractValidator<EfetuarTransferenciaCommand>
{
    public EfetuarTransferenciaValidator()
    {
        RuleFor(x => x.IdRequisicao)
            .NotEmpty()
            .WithErrorCode("INVALID_REQUEST")
            .WithMessage("Identificação da requisição é obrigatória.");

        RuleFor(x => x.NumeroContaDestino)
            .GreaterThan(0)
            .WithErrorCode("INVALID_ACCOUNT")
            .WithMessage("Conta destino inválida.");

        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithErrorCode("INVALID_VALUE")
            .WithMessage("Valor deve ser positivo.");
    }
}