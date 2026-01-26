using FluentValidation;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.Validator
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("الإيميل الخاص بك مطلوب")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صالح");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("الرقم السري الخاص بك مطلوب")
                .MinimumLength(12).WithMessage("يجب أن يكون الرقم السري علي ألاقل 12 رقم أو حرف");
        }
    }
}