using FluentValidation;
using HomeTech.Application.Interfaces;
using HomeTech.Application.CQRS.Commands;

namespace HomeTech.Application.Validator
{
    public class CreateHomeTechServiceValidator : AbstractValidator<CreateHomeTechServiceCommand>
    {
        private readonly IHomeTechServiceRepository _repository;

        public CreateHomeTechServiceValidator(IHomeTechServiceRepository repository)
        {
            _repository = repository;

            RuleFor(s => s.Dto.Name)
                .NotEmpty().WithMessage("اسم الخدمة مطلوب")
                .MinimumLength(3).WithMessage("يجب ان يكون اسم الخدمة اكبر من 3 احرف")
                .MaximumLength(100).WithMessage("يجب ان يكون اسم الخدمة اقل من 100 حرف");

            RuleFor(s => s)
                .MustAsync(async (dto, ct) =>
                    !await _repository.ExistsByNameAsync(dto.Dto.Name.Trim(), null, ct)
                )
                .WithMessage("هذا الاسم مستخدم بالفعل");

            RuleFor(l => l.Dto.IsActive)
                .NotNull()
                .WithMessage("حالة الخدمة مطلوبة");

            RuleFor(x => x.Dto.Price).GreaterThan(0).WithMessage("سعر الخدمة يجب ان يكون اكبر من صفر");
        }
    }
}