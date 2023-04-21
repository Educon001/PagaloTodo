using UCABPagaloTodoMS.Application.Commands;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Infrastructure.Utils;
using FluentValidation;

namespace UCABPagaloTodoMS.Application.Validators
{
    public class AddTaxPayerValidator : AbstractValidator<AddTaxPayerCommand>
    {
        public AddTaxPayerValidator()
        {
            RuleFor(c => c.Object.QuotationId)
                .NotEmpty().WithMessage("The quote id is required.");

            RuleFor(c => c.Object.IdentificationType)
                .NotEmpty().WithMessage("The identification type of the tax payer is required.");

            RuleFor(c => c.Object.IdentificationNumber)
                .NotEmpty().WithMessage("The identification number of the tax payer is required.");

            RuleFor(c => c.Object.FirstName)
                .NotEmpty().WithMessage("The name of the tax payer is required.");

            RuleFor(c => c.Object.LastName)
              .NotEmpty().WithMessage("The last name of the tax payer is required.");

            RuleFor(c => c.Object.BirthDate)
            .NotEmpty().WithMessage("The birthdate of the tax payer is required.");

            RuleFor(c => c.Object.Gender)
            .NotEmpty().WithMessage("The gender of the tax payer is required.");

            RuleFor(c => c.Object.Address)
                .NotEmpty().WithMessage("The address of the tax payer is required.");

            RuleFor(c => c.Object.Email)
                .NotEmpty().WithMessage("The email addres of the tax payer is required.");

            RuleFor(c => c.Object.Region)
                .NotEmpty().WithMessage("The addres region of the tax payer is required.");

            RuleFor(c => c.Object.Country)
                .NotEmpty().WithMessage("The addres country of the tax payer is required.");

            RuleFor(c => c.Object)
              .Must(ValidateEmail).WithMessage("The email addres of the tax payer is invalid.");
        }

        private static bool ValidateEmail(TaxPayerRequest request)
        {
            if (request.Email == null)
                return false;

            return Utilities.IsValidEmail(request.Email);
        }
    }
}
