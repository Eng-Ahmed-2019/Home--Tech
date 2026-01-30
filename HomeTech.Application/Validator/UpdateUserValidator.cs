using FluentValidation;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.Validator
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Dto.FullName)
                .NotEmpty().WithMessage("اسم المستخدم مطلوب")
                .MinimumLength(3).WithMessage("اسم المستخدم يجب أن يكون 3 أحرف على الأقل")
                .MaximumLength(20).WithMessage("اسم المستخدم يجب أن لا يتجاوز 20 حرفًا");

            RuleFor(x => x.Dto.Gender)
                .IsInEnum()
                .WithMessage("النوع غير صالح");
        }
    }
}