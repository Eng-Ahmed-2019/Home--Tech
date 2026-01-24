using FluentValidation;
using HomeTech.Application.DTOs;

namespace HomeTech.Application.Validator
{
    public class RegisterUserValidator
        : AbstractValidator<RegisterUserDto>
    {
        private static readonly HashSet<string> CommonPasswords = new(StringComparer.OrdinalIgnoreCase)
        {
            "123456",
            "password",
            "12345678",
            "qwerty",
            "admin",
            "letmein",
            "welcome",
            "iloveyou"
        };

        public RegisterUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("اسم المستخدم مطلوب")
                .MinimumLength(3).WithMessage("اسم المستخدم يجب أن يكون 3 أحرف على الأقل")
                .MaximumLength(20).WithMessage("اسم المستخدم يجب أن لا يتجاوز 20 حرفًا");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("البريد الإلكتروني غير صالح");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(12).WithMessage("كلمة المرور يجب أن تكون 12 حرفًا على الأقل")
                .Matches("[A-Z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل")
                .Matches("[a-z]").WithMessage("كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل")
                .Matches("[0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رقم واحد على الأقل")
                .Matches("[^a-zA-Z0-9]").WithMessage("كلمة المرور يجب أن تحتوي على رمز خاص واحد على الأقل")
                .Must(HaveAtLeastUniqueCharacters)
                    .WithMessage("كلمة المرور يجب أن تحتوي على 4 أحرف مختلفة على الأقل")
                .Must(NotContainEmail)
                    .WithMessage("كلمة المرور يجب ألا تحتوي على البريد الإلكتروني أو جزء منه")
                .Must(NotBeCommonPassword)
                    .WithMessage("كلمة المرور ضعيفة أو شائعة جدًا");
        }

        private bool HaveAtLeastUniqueCharacters(string password)
        {
            return password.Distinct().Count() >= 4;
        }

        private bool NotContainEmail(RegisterUserDto model, string password)
        {
            if (string.IsNullOrWhiteSpace(model.Email)) return true;

            var emailParts = model.Email
                .Split('@', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.ToLower());

            var lowerPassword = password.ToLower();

            return !emailParts.Any(part => part.Length >= 3 && lowerPassword.Contains(part));
        }

        private bool NotBeCommonPassword(string password)
        {
            return !CommonPasswords.Contains(password);
        }
    }
}