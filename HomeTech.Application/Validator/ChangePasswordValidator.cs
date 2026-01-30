using FluentValidation;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.Validator
{
    public class ChangePasswordValidator
        : AbstractValidator<ChangePasswordCommand>
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

        public ChangePasswordValidator()
        {
            RuleFor(x => x.Dto.CurrentPassword)
                .NotEmpty().WithMessage("كلمة المرور الحالية مطلوبة");

            RuleFor(x => x.Dto.NewPassword)
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

        private bool NotContainEmail(ChangePasswordCommand model, string password)
        {
            if (string.IsNullOrWhiteSpace(model.Dto.NewPassword)) return true;

            var email = model.Dto.NewPassword.ToLower();
            var lowerPassword = password.ToLower();

            return !email.Split('@')
                .Any(part => part.Length >= 3 && lowerPassword.Contains(part));
        }

        private bool NotBeCommonPassword(string password)
        {
            return !CommonPasswords.Contains(password);
        }
    }
}