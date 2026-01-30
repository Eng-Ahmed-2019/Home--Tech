using FluentValidation;
using HomeTech.Application.CQRS.Queries;

namespace HomeTech.Application.Validator
{
    public class LoginValidator : AbstractValidator<LoginQuery>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("الإيميل الخاص بك مطلوب")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صالح");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("الرقم السري الخاص بك مطلوب")
                .MinimumLength(12).WithMessage("يجب أن يكون الرقم السري علي ألاقل 12 رقم أو حرف");
        }
    }
}